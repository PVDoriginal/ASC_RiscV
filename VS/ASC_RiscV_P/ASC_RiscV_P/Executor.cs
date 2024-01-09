using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC_RiscV_P {
    internal class Executor {

        static Dictionary<byte, List<int>> LabelPositions = new();
        static int[] RegisterValues = new int[32];
        static float[] FRegisterValues = new float[32];

        public static void Execute(){

            BinaryReader bin = new BinaryReader(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/bin.idk", FileMode.Open));

            bool[] bits = GetBytes(bin);

            foreach (bool bit in bits)
                Console.Write(bit ? 1 : 0);
            Console.WriteLine("\n");

            RecordLabels(bits);

            Console.WriteLine(RegisterValues[5]);
            Execute(LabelPositions[0][0], bits); // execute from main
            Console.WriteLine(RegisterValues[5]);
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

                if (MainClass.Instructions[s] == "li"){
                    bool rtype = bits[curr++];
                    byte reg = GetByte(ref curr, bits);

                    if (!rtype) 
                        RegisterValues[reg] = GetConstant(ref curr, bits);
                    else 
                        FRegisterValues[reg] = GetConstantFloat(ref curr, bits);
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

                if (MainClass.Instructions[s] == "label")
                    RegisterLabel(GetByte(ref curr, bits), curr);
            }

        }

        static void RegisterLabel(byte label, int pos){

            if (!LabelPositions.Keys.Contains(label))
                LabelPositions[label] = new List<int>();

            LabelPositions[label].Add(pos);
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
