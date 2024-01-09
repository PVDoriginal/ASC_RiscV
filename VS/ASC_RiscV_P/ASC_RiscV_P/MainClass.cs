using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ASC_RiscV_P {
    internal class MainClass {

        public static Dictionary<string, string> Codes = new();
        public static Dictionary<string, string> Instructions = new();
        public static Dictionary<string, byte> RegisterCodes = new();

        static void Main(string[] args) {

            GetCodes();

            Console.WriteLine("Input 1 to Assemble, 2 to Execute");
            int x = int.Parse(Console.ReadLine());

            if (x == 1) { Assembler.Assemble(); return; }
            if (x == 2) { Executor.Execute(); return; }

            Console.WriteLine("Only Accepted Input is 1 or 2!");
        }

        static void GetCodes(){

            string[] lines = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Instructions.txt");
            foreach(string line in lines){
                string[] words = line.Split(' ');
                Codes[words[0]] = words[1]; // Convert.ToByte(words[1], 2);
                Instructions[words[1]] = words[0];
            }
            lines = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Registers.txt");
            foreach (string line in lines) {
                string[] words = line.Split(' ');
                RegisterCodes[words[0]] = Convert.ToByte(words[1]);
            }
        }
    }

}
