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

            Labels["_start"] = 0;
            Labels["printf"] = 1;
            Labels["scanf"] = 2;

            string s = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Codes/" + MainClass.Code + ".txt");

            //BinaryWriter bin = new BinaryWriter(File.Open("D:/Github/ASC_P/VS/ASC_RiscV_P/ASC_RiscV_P/bin.idk", FileMode.Create));
            BinaryWriter bin = new BinaryWriter(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Binaries/" + MainClass.Code + ".bin", FileMode.Create));
            Queue<bool> bits = new();

            int curr = 0;

            while (curr < s.Length) {

                if (IsDataSectionStart(curr, s)) {
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
                        WriteCode(Convert.ToInt64(constant), ref bits);
                    else
                        WriteCode(Convert.ToSingle(constant), ref bits);

                    continue;
                }
                if (IsInstruction("la", curr, s)) {
                    WriteCode(MainClass.Codes["la"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string variable = GetWord(ref curr, s, true);
                    WriteCode(GetVariableCode(variable), ref bits);

                    continue;
                }
                if (IsInstruction("addi", curr, s)) {
                    WriteCode(MainClass.Codes["addi"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

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
                if (IsInstruction("sub", curr, s)) {
                    WriteCode(MainClass.Codes["sub"], ref bits);
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
                if (IsInstruction("lb", curr, s)) {
                    WriteCode(MainClass.Codes["lb"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }
                if (IsInstruction("ld", curr, s)) {
                    WriteCode(MainClass.Codes["ld"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }
                if (IsInstruction("srai", curr, s)) {
                    WriteCode(MainClass.Codes["srai"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

                    continue;
                }
                if (IsInstruction("sb", curr, s)) {
                    WriteCode(MainClass.Codes["sb"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }
                if (IsInstruction("sd", curr, s)) {
                    WriteCode(MainClass.Codes["sd"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string constant = GetWord(ref curr, s, true);
                    WriteCode(Convert.ToInt64(constant), ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }
                if (IsInstruction("beqz", curr, s)) {
                    WriteCode(MainClass.Codes["beqz"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string label = GetWord(ref curr, s, true);
                    WriteCode(label[label.Length - 1] == 'f' ? "1" : "0", ref bits);

                    label = label.Substring(0, label.Length - 1);
                    WriteCode(GetLabelCode(label), ref bits);

                    continue;
                }
                if (IsInstruction("bge", curr, s)) {
                    WriteCode(MainClass.Codes["bge"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    string label = GetWord(ref curr, s, true);
                    WriteCode(label[label.Length - 1] == 'f' ? "1" : "0", ref bits);

                    label = label.Substring(0, label.Length - 1);
                    WriteCode(GetLabelCode(label), ref bits);

                    continue;
                }
                if (IsInstruction("j", curr, s)) {

                    WriteCode(MainClass.Codes["j"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string label = GetWord(ref curr, s, true);
                    WriteCode(label[label.Length - 1] == 'f' ? "1" : "0", ref bits);

                    label = label.Substring(0, label.Length - 1);
                    WriteCode(GetLabelCode(label), ref bits);
                    continue;
                }
                if (IsInstruction("mv", curr, s)) {

                    WriteCode(MainClass.Codes["mv"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    reg = GetWord(ref curr, s, true);
                    WriteCode(reg[0] == 'f' ? "1" : "0", ref bits);
                    WriteCode(MainClass.RegisterCodes[reg], ref bits);

                    continue;
                }
                if (IsInstruction("call", curr, s)) {

                    WriteCode(MainClass.Codes["call"], ref bits);
                    JumpToNextWord(ref curr, s);

                    string label = GetWord(ref curr, s, true);
                    WriteCode(GetLabelCode(label), ref bits);

                    continue;
                }
                if (IsInstruction("ret", curr, s)) {
                    WriteCode(MainClass.Codes["ret"], ref bits);
                    JumpToNextWord(ref curr, s);
                    continue;
                }
            }

            WriteCode(MainClass.Codes["eof"], ref bits);

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

            Queue<bool> dataBits = new();
            Queue<Tuple<byte, int>> Vars = new();
            int localAddress = 0;

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

                Vars.Enqueue(new Tuple<byte, int>(GetVariableCode(GetLabel(curr, s)), localAddress));
                JumpToNextWord(ref curr, s);

                string type = GetWord(ref curr, s, true);
                switch (type){
                    case ".asciz":
                        byte[] bytes = Encoding.ASCII.GetBytes(GetString(ref curr, s, true));
                        WriteCode(bytes, ref dataBits, true);
                        localAddress += bytes.Length * 8;
                        break;

                    case ".word":
                        string word = ""; 
                        do {
                            word = GetWord(ref curr, s, true, false);
                            WriteCode(Convert.ToInt32(word.Replace(",", "")), ref dataBits);
                            localAddress += 32;
                        } while (word[word.Length - 1] == ',');
                        break;
                }
            }
            int textOffset = 32 + Vars.Count * 40 + MainClass.Codes["eof"].Length + dataBits.Count;
            int dataOffset = 32 + Vars.Count * 40 + MainClass.Codes["eof"].Length;
            WriteCode(textOffset, ref bits);
            
            while(Vars.Count != 0){
                Tuple<byte, int> t = Vars.Dequeue();
                WriteCode(t.Item1, ref bits); WriteCode(t.Item2 + dataOffset, ref bits);
            }

            WriteCode(MainClass.Codes["eof"], ref bits);

            while (dataBits.Count != 0)
                bits.Enqueue(dataBits.Dequeue());
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
        static void WriteCode(long code, ref Queue<bool> bits) {
            string rep = Convert.ToString(code, 2);
            while (rep.Length != 64)
                rep = '0' + rep;

            WriteCode(rep, ref bits);
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

        static string GetWord(ref int curr, string s, bool skipWhiteSpace = false, bool ignoreComma = true){
            string t = "";
            while(curr < s.Length && (!IsWhitespace(s[curr]) || (s[curr] == ',' && !ignoreComma))) {
                t += s[curr];
                curr++;
            }

            if (skipWhiteSpace) JumpPastWhitespace(ref curr, s);

            return t;
        }

        static string GetString(ref int curr, string s, bool skipWhiteSpace = false) {
            string t = "";

            curr++;
            while (curr < s.Length && s[curr] != '"') {
                if (s[curr] == 92 && s[curr + 1] == 110) {
                    t += '\n';
                    curr += 2;
                }
                else
                    t += s[curr++];
            }
            curr++;

            if (skipWhiteSpace) JumpPastWhitespace(ref curr, s);
            return t;
        }

        static string GetLabel(int curr, string s){
            string label = "";
            while (curr < s.Length && !IsWhitespace(s[curr]) && s[curr] != ':') { label += s[curr]; ++curr; }
            return label;
        }

        static byte currLabel = 3;
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
