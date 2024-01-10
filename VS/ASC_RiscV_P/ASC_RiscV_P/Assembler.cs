using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC_RiscV_P {
    internal class Assembler {

        static Dictionary<string, byte> Labels = new();
        static Dictionary<string, byte> Variables = new();
        public static void Assemble() {

            Labels["_start"] = (byte)0;
            string s = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/RVCode.txt");

            //BinaryWriter bin = new BinaryWriter(File.Open("D:/Github/ASC_P/VS/ASC_RiscV_P/ASC_RiscV_P/bin.idk", FileMode.Create));
            BinaryWriter bin = new BinaryWriter(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/bin.idk", FileMode.Create));
            Queue<bool> bits = new();

            int curr = 0;

            while (curr < s.Length) {

                if (s[curr] == 0) break;

                if(IsDataSectionStart(curr, s)){
                    JumpToNextLine(ref curr, s);
                    AssembleData(ref curr, s, ref bits);
                    continue;
                }

                if (s[curr] == '.' || s[curr] == '#') {
                    JumpToNextLine(ref curr, s);
                    continue;
                }

                if (IsWhitespace(s[curr])) {
                    JumpPastWhitespace(ref curr, s);
                    continue;
                }

                if (IsLabel(curr, s)) {
                    WriteCode(MainClass.Codes["label"], ref bits);
                    WriteCode(GetLabelCode(GetLabel(curr, s)), ref bits);
                    JumpToNextWord(ref curr, s);
                    continue;
                }

                if (IsInstruction("li", curr, s)) {
                    WriteCode(MainClass.Codes["li"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);

                    if (reg[0] != 'f') 
                        WriteCode(Convert.ToInt32(constant), ref bits);
                    else 
                        WriteCode(Convert.ToSingle(constant), ref bits);
                    
                    continue;
                }

                if(IsInstruction("la", curr, s)) {
                    WriteCode(MainClass.Codes["la"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string variable = GetWord(ref curr, s, true);
                    WriteCode(GetVariableCode(variable), ref bits);

                    continue;
                }

                if (IsInstruction("add", curr, s)) {
                    WriteCode(MainClass.Codes["add"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }

                if(IsInstruction("lb", curr, s)) {
                    WriteCode(MainClass.Codes["lb"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt32(constant), ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);
                }

                curr++;
            }

            while(bits.Count != 0){

                while (bits.Count < 8) bits.Enqueue(false);

                byte b = 0;

                for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                    b += bits.Dequeue() ? a : (byte)0;

                bin.Write(b);
            }

            bin.Flush();
            bin.Close();

        }

        static void AssembleData(ref int curr, string s, ref Queue<bool> bits){

            while (curr < s.Length) {

                if (s[curr] == 0) break;

                if (IsTextSectionStart(curr, s)){
                    JumpToNextLine(ref curr, s);
                    break;
                }

                if (s[curr] == '.' || s[curr] == '#') {
                    JumpToNextLine(ref curr, s);
                    continue;
                }

                if (IsWhitespace(s[curr])) {
                    JumpPastWhitespace(ref curr, s);
                    continue;
                }

                WriteCode(MainClass.Codes["var"], ref bits);
                WriteCode(GetVariableCode(GetLabel(curr, s)), ref bits);
                JumpToNextWord(ref curr, s);

                string type = GetWord(ref curr, s, true);
                switch (type){
                    case ".asciz":
                        WriteCode((byte)1, ref bits);
                        WriteCode(Encoding.ASCII.GetBytes(GetString(ref curr, s, true)), ref bits, true); 
                        break;
                }
            }

        }

        static void JumpToNextLine(ref int curr, string s) {
            while (curr < s.Length && s[curr] != '\n') curr++;
        }

        static void JumpPastWhitespace(ref int curr, string s) {
            while (curr < s.Length && IsWhitespace(s[curr])) curr++;
        }

        static bool IsInstruction(string t, int curr, string s) {
            int i;
            for (i = 0; curr + i < s.Length && i < t.Length && s[curr + i] == t[i]; ++i) ;
            return (!(curr + i > s.Length || i < t.Length));
        }
        static void JumpToNextWord(ref int curr, string s) {
            while (curr < s.Length && !IsWhitespace(s[curr])) ++curr;
            JumpPastWhitespace(ref curr, s);
        }

        static void WriteCode(string code, ref Queue<bool> bits){

            foreach (char c in code)
                if (c == '0')
                    bits.Enqueue(false);
                else if (c == '1')
                    bits.Enqueue(true);
        }
        static void WriteCode(bool[] code, ref Queue<bool> bits){
            foreach (bool b in code)
                bits.Enqueue(b);
        }

        static void WriteCode(byte code, ref Queue<bool> bits) {
            for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                bits.Enqueue((code & a) != 0 ? true : false);
        }
        static void WriteCode(int code, ref Queue<bool> bits) {
            string rep = Convert.ToString(code, 2);
            while (rep.Length != 32)
                rep = '0' + rep;

            WriteCode(rep, ref bits);
        }

        static void WriteCode(byte[] code, ref Queue<bool> bits, bool addNull = false) {
            foreach (byte b in code)
                WriteCode(b, ref bits);
            if(addNull)
                WriteCode((byte)0, ref bits);
        }

        static void WriteCode(float code, ref Queue<bool> bits) {
            string rep = Convert.ToString(BitConverter.ToInt32(BitConverter.GetBytes(code), 0), 2);
            while (rep.Length != 32)
                rep = '0' + rep;

            WriteCode(rep, ref bits);
        }

        static bool IsDataSectionStart(int curr, string s) => IsInstruction(".section .rodata", curr, s);
        static bool IsTextSectionStart(int curr, string s) => IsInstruction(".section .text", curr, s);


        static bool IsLabel(int curr, string s) {
            while (curr < s.Length && !IsWhitespace(s[curr]) && s[curr] != ':') ++curr;
            return curr < s.Length && s[curr] == ':';
        }

        static string GetWord(ref int curr, string s, bool skipWhiteSpace = false){
            string t = "";
            while(curr < s.Length && !IsWhitespace(s[curr])) {
                t += s[curr];
                curr++;
            }

            if (skipWhiteSpace) JumpPastWhitespace(ref curr, s);

            return t;
        }

        static string GetString(ref int curr, string s, bool skipWhiteSpace = false) {
            string t = "";

            curr++;
            while (curr < s.Length && s[curr] != '"') t += s[curr++];
            curr++;

            if (skipWhiteSpace) JumpPastWhitespace(ref curr, s);
            return t;
        }

        static string GetLabel(int curr, string s){
            string label = "";
            while (curr < s.Length && !IsWhitespace(s[curr]) && s[curr] != ':') { label += s[curr]; ++curr; }
            return label;
        }

        static byte currLabel = 1;
        static byte currVar = 0;

        static byte GetLabelCode(string label) {
            if(!Labels.Keys.Contains(label)) Labels[label] = currLabel++;
            return Labels[label];
        } 

        static byte GetVariableCode(string name){
            if (!Variables.Keys.Contains(name)) Variables[name] = currVar++;
            return Variables[name];
        }

        static bool IsWhitespace(char c) => Char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')';
    }
}
