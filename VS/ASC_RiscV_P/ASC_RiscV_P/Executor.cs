using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC_RiscV_P {
    internal class Executor {

        static Dictionary<byte, List<int>> LabelPositions = new();
        static Dictionary<byte, int> VariablePositions = new();
        static int[] RegisterValues = new int[32];
        static float[] FRegisterValues = new float[32];

        public static void Execute(){

            BinaryReader bin = new BinaryReader(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/bin.idk", FileMode.Open));

            bool[] bits = GetBytes(bin);

            foreach (bool bit in bits)
                Console.Write(bit ? 1 : 0);
            Console.WriteLine("\n");

            RecordLabels(bits);
            Execute(LabelPositions[0][0], bits); // execute main
            Console.WriteLine(Convert.ToChar(RegisterValues[6])); 
        }
        
        static void Execute(int curr, bool[] bits) {

            while (curr < bits.Length) {

                string s = "";
                while (curr < bits.Length && !MainClass.Instructions.Keys.Contains(s))
                    s += bits[curr++] ? "1" : "0";

                if (curr == bits.Length) break;

                if (MainClass.Instructions[s] == "label") {
                    GetByte(ref curr, bits);
                    continue;
                }

                if (MainClass.Instructions[s] == "li") {
                    bool rtype = bits[curr++];
                    int reg = GetRegister(curr, bits); curr += 5;

                    if (!rtype) 
                        RegisterValues[reg] = GetConstant(ref curr, bits);
                    else 
                        FRegisterValues[reg] = GetConstantFloat(ref curr, bits);
                    continue;
                }

                if (MainClass.Instructions[s] == "la") {
                    bool rtype = bits[curr++];
                    int reg = GetRegister(curr, bits); curr += 5;

                    byte variable = GetByte(ref curr, bits);
                    if (!rtype)
                        RegisterValues[reg] = VariablePositions[variable] / 8;
                    else
                        FRegisterValues[reg] = VariablePositions[variable] / 8;
                    continue;
                }

                if (MainClass.Instructions[s] == "add") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    if (!rtype)
                        RegisterValues[reg1] = RegisterValues[reg2] + RegisterValues[reg3];
                    else
                        FRegisterValues[reg1] = FRegisterValues[reg2] + FRegisterValues[reg3];
                    continue;
                }

                if (MainClass.Instructions[s] == "lb") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    int constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    int address = constant + RegisterValues[reg_op];

                    if(!rtype)
                        RegisterValues[reg_dest] = GetByte(address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetByte(address * 8, bits);
                    continue;
                }
            }

        }

        static void RecordLabels(bool[] bits){

            int curr = 0;
            while (curr < bits.Length) {

                string s = "";
                while (curr < bits.Length && !MainClass.Instructions.Keys.Contains(s))
                    s += bits[curr++] ? "1" : "0";

                if (curr == bits.Length) break;

                if (MainClass.Instructions[s] == "label") {
                    byte label = GetByte(ref curr, bits);
                    RegisterLabel(label, curr);
                    continue;
                }

                if (MainClass.Instructions[s] == "var") {
                    byte varId = GetByte(ref curr, bits);
                    byte type = GetByte(ref curr, bits);

                    VariablePositions[varId] = curr;

                    switch (type) {

                        //asciz 
                        case 1:
                            byte b = 1;
                            while(b != 0)
                                b = GetByte(ref curr, bits);    

                            break;

                    }
                    continue;
                }

                bool foundInstruction = false;
                foreach (string instruction in MainClass.Codes.Keys)
                    if (instruction != "var" && instruction != "label" && MainClass.Instructions[s] == instruction) {
                        curr += MainClass.InstructionSizes[instruction];
                        foundInstruction = true; break; 
                    }
                if (foundInstruction) continue;
                break;
            }

        }

        static void RegisterLabel(byte label, int pos){

            if (!LabelPositions.Keys.Contains(label))
                LabelPositions[label] = new List<int>();

            LabelPositions[label].Add(pos);
        }

        static bool IsInstruction(string t, int curr, bool[] bits){
            string s = "";
            while(s.Length < MainClass.Codes[t].Length && curr < bits.Length)
                s += bits[curr++] ? "1" : "0";
            return s == MainClass.Codes[t] && curr < bits.Length;
        }

        static byte GetRegister(int curr, bool[] bits){
            byte b = 0, a = 16;
            for (int i = curr; i < curr + 5; i++, a >>= 1)
                if (bits[i])
                    b += a;
            return b;
        }

        static byte GetByte(int curr, bool[] bits){
            byte b = 0;
            for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                b += bits[curr++] ? a : (byte)0;
            return b;
        }
        static byte GetByte(ref int curr, bool[] bits) {
            byte b = 0;
            for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                b += bits[curr++] ? a : (byte)0;
            return b;
        }
        static int GetConstant(ref int curr, bool[] bits) {
            string t = "";
            for (int i = 0; i < 32; ++i)
                t += bits[curr++] ? "1" : "0";
            return Convert.ToInt32(t, 2);  
        }

        static float GetConstantFloat(ref int curr, bool[] bits){
            string t = "";
            for (int i = 0; i < 32; ++i)
                t += bits[curr++] ? "1" : "0";
            Console.WriteLine(t);
            return BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(t, 2)), 0);
        }

        static bool[] GetBytes(BinaryReader bin){
            long count = bin.BaseStream.Length;
            bool[] bits = new bool[count * 8];
            int curr = 0;
            while(count-- != 0) {

                byte b = bin.ReadByte();

                for (int k = 7, j = 128; k >= 0; k--, j >>= 1) 
                    bits[curr++] = ((b & j) != 0 ? true : false);
                
            }

            return bits;
        }
    }
}
