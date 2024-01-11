using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ASC_RiscV_P {
    internal class MainClass {

        public static string Code = "strrev";

        public static Dictionary<string, string> Codes = new();
        public static Dictionary<string, string> Instructions = new();
        public static Dictionary<string, bool[]> RegisterCodes = new();
        public static Dictionary<string, int> InstructionSizes = new();

        static void Main(string[] args) {

            GetCodes();

            Console.WriteLine("Input 1 to Assemble, 2 to Execute, 3 for Both");
            int x = int.Parse(Console.ReadLine());

            if (x == 1) { Assembler.Assemble(); return; }
            if (x == 2) { Executor.Execute(); return; }
            if(x == 3) { Assembler.Assemble(); Executor.Execute(); return; }  

            Console.WriteLine("Only Accepted Input is 1 or 2!");
        }

        static void GetCodes(){

            string[] lines = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Instructions.txt");
            foreach(string line in lines){
                string[] words = line.Split(' ');
                Codes[words[0]] = words[1]; // Convert.ToByte(words[1], 2);
                Instructions[words[1]] = words[0];
                InstructionSizes[words[0]] = Convert.ToInt32(words[2]);
            }
            lines = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Registers.txt");
            foreach (string line in lines) {
                string[] words = line.Split(' ');

                RegisterCodes[words[0]] = new bool[5];
                string binaryString = Convert.ToString(Convert.ToInt32(words[1]), 2);
                while (binaryString.Length < 5)
                    binaryString = '0' + binaryString;

                for (int i = 0; i < 5; ++i)
                    RegisterCodes[words[0]][i] = binaryString[i] == '1';
            }
        }
    }

}
