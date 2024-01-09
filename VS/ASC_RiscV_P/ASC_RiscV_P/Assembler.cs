using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC_RiscV_P {
    internal class Assembler {

        static Dictionary<string, byte> Labels = new();
        public static void Assemble() {

            Labels["_start"] = (byte)0;
            string s = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/RVCode.txt");

            //BinaryWriter bin = new BinaryWriter(File.Open("D:/Github/ASC_P/VS/ASC_RiscV_P/ASC_RiscV_P/bin.idk", FileMode.Create));
            BinaryWriter bin = new BinaryWriter(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/bin.idk", FileMode.Create));
            Queue<bool> bits = new();

            int curr = 0;

            while (curr < s.Length) {

                if (s[curr] == 0) break;

                if (s[curr] == '.' || s[curr] == '#') {
                    JumpToNextLine(ref curr, s);
                    continue;
                }

                if (s[curr] == ' ' || s[curr] == '\n') {
                    JumpPastWhitespace(ref curr, s);
                    continue;
                }

                if(IsLabel(curr, s)){
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

        }

        static void JumpToNextLine(ref int curr, string s) {
            while (curr < s.Length && s[curr] != '\n') curr++;
        }

        static void JumpPastWhitespace(ref int curr, string s) {
            while (curr < s.Length && (s[curr] == ' ' || s[curr] == '\n' || s[curr] == ',')) curr++;
        }

        static bool IsInstruction(string t, int curr, string s) {
            int i;
            for (i = 0; curr + i < s.Length && i < t.Length && s[curr + i] == t[i]; ++i) ;
            if (curr + i > s.Length || i < t.Length) return false;
            return true;
        }

        static void JumpToNextWord(ref int curr, string s) {
            while (curr < s.Length && s[curr] != ' ' && s[curr] != '\n' && s[curr] != ',') ++curr;
            JumpPastWhitespace(ref curr, s);
        }

        static void WriteCode(string code, ref Queue<bool> bits){

            foreach (char c in code)
                if (c == '0')
                    bits.Enqueue(false);
                else if (c == '1')
                    bits.Enqueue(true);
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
        static void WriteCode(float code, ref Queue<bool> bits) {
            string rep = Convert.ToString(BitConverter.ToInt32(BitConverter.GetBytes(code), 0), 2);
            Console.WriteLine(rep);
            while (rep.Length != 32)
                rep = '0' + rep;

            WriteCode(rep, ref bits);
        }



        static bool IsLabel(int curr, string s) {
            while (curr < s.Length && s[curr] != ' ' && s[curr] != '\n' && s[curr] != ':' && s[curr] != ',') ++curr;
            return curr < s.Length && s[curr] == ':';
        }

        static string GetWord(ref int curr, string s, bool skipWhiteSpace = false){
            string t = "";
            while(curr < s.Length && s[curr] != ' ' && s[curr] != '\n' && s[curr] != ','){
                t += s[curr];
                curr++;
            }

            if (skipWhiteSpace) JumpPastWhitespace(ref curr, s);

            return t;
        }

        static string GetLabel(int curr, string s){
            string label = "";
            while (curr < s.Length && s[curr] != ' ' && s[curr] != '\n' && s[curr] != ':' && s[curr] != ',') { label += s[curr]; ++curr; }
            return label;
        }

        static byte currLabel = 1; 
        
        static byte GetLabelCode(string label) {
            if(!Labels.Keys.Contains(label)) Labels[label] = currLabel++;
            return Labels[label];
        } 
    }
}
