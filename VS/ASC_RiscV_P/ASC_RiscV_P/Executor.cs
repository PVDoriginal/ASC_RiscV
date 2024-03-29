﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC_RiscV_P {
    internal class Executor {
        [DllImport("msvcr120.dll")]
        public static extern int printf(string format, __arglist);

        [DllImport("msvcr120.dll")]
        public static extern int scanf(string format, __arglist);

        static Dictionary<byte, List<int>> LabelPositions = new();
        static Dictionary<byte, int> VariablePositions = new();
        static long[] RegisterValues = new long[32];
        static double[] FRegisterValues = new double[32];

        public static void Execute(){

            BinaryReader bin = new BinaryReader(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Binaries/" + MainClass.Code + ".bin", FileMode.Open));

            List<bool> bits = GetBytes(bin);

            /*foreach (bool bit in bits)
                Console.Write(bit ? 1 : 0);
            Console.WriteLine("\n");*/

            // allocate stack
            for (int i = 0; i < 5000; ++i)
                bits.Add(false);
            while (bits.Count % 8 != 0)
                bits.Add(false);

            RegisterValues[2] = bits.Count / 8;

            RecordVars(bits, 32, GetInt(0, bits));
            RecordLabels(bits, GetInt(0, bits));

            Execute(LabelPositions[0][0], bits); // execute main

            WriteEndStatus();
        }
        
        static void Execute(int curr, List<bool> bits) {

            while (curr < bits.Count) {

                string s = "";
                while (curr < bits.Count && !MainClass.Instructions.Keys.Contains(s))
                    s += bits[curr++] ? "1" : "0";

                //Console.WriteLine(s);

                if (curr == bits.Count) break;
                if(MainClass.Instructions[s] == "eof"){ return; }
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
                        FRegisterValues[reg] = GetConstantDouble(ref curr, bits);
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
                if (MainClass.Instructions[s] == "sub") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    if (!rtype)
                        RegisterValues[reg1] = RegisterValues[reg2] - RegisterValues[reg3];
                    else
                        FRegisterValues[reg1] = FRegisterValues[reg2] - FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fsub.d") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = FRegisterValues[reg2] - FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fadd.d") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = FRegisterValues[reg2] + FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fadd.s") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = FRegisterValues[reg2] + FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fmul.d") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = FRegisterValues[reg2] * FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fmul.s") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = FRegisterValues[reg2] * FRegisterValues[reg3];
                    continue;
                }
                if (MainClass.Instructions[s] == "fsqrt.d") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;
                    curr++;
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg1] = Math.Sqrt(FRegisterValues[reg2]);
                    continue;
                }
                if (MainClass.Instructions[s] == "lb") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_op];

                    if (!rtype)
                        RegisterValues[reg_dest] = GetByte((int)address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetByte((int)address * 8, bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "lw") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_op];

                    if (!rtype)
                        RegisterValues[reg_dest] = GetInt((int)address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetInt((int)address * 8, bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "flw") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_op];

                    if (!rtype)
                        RegisterValues[reg_dest] = (int)GetFloatSingle((int)address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetFloatSingle((int)address * 8, bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "ld") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_op];
                    
                    if (!rtype)
                        RegisterValues[reg_dest] = GetDouble((int)address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetDouble((int)address * 8, bits);

                    continue;
                }
                if (MainClass.Instructions[s] == "fld") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);

                    bool rtype2 = bits[curr++];
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    
                    long address = RegisterValues[reg_op];

                    if (!rtype)
                        RegisterValues[reg_dest] = GetDouble((int)address * 8, bits);
                    else
                        FRegisterValues[reg_dest] = GetFloatDouble((int)address * 8, bits);

                    continue;
                }
                if (MainClass.Instructions[s] == "sb") {

                    bool rtype = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_dest];

                    if (!rtype)
                        WriteByteAt((int)address * 8, (byte)RegisterValues[reg_source], bits);
                    else
                        WriteByteAt((int)address * 8, (byte)FRegisterValues[reg_source], bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "sd") {

                    bool rtype = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_dest];

                    if (!rtype)
                        WriteDoubleAt((int)address * 8, (long)RegisterValues[reg_source], bits);
                    else
                        WriteDoubleAt((int)address * 8, (long)FRegisterValues[reg_source], bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "fsw") {

                    bool rtype = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);
                    curr++;
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    long address = constant + RegisterValues[reg_dest];


                    if (!rtype)
                        WriteSingleAt((int)address * 8, (float)FRegisterValues[reg_source], bits);
                    else
                        WriteSingleAt((int)address * 8, (float)FRegisterValues[reg_source], bits);
                    continue;
                }
                if (MainClass.Instructions[s] == "srai") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    curr++;
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);

                    if (!rtype)
                        RegisterValues[reg_dest] = (int)RegisterValues[reg_source] >> (int)constant;
                    else
                        FRegisterValues[reg_dest] = (int)RegisterValues[reg_source] >> (int)constant;
                    continue;
                }
                if (MainClass.Instructions[s] == "slli") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    curr++;
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);

                    if (!rtype)
                        RegisterValues[reg_dest] = (int)RegisterValues[reg_source] << (int)constant;
                    else
                        FRegisterValues[reg_dest] = (int)RegisterValues[reg_source] << (int)constant;
                    continue;
                }
                if (MainClass.Instructions[s] == "beqz") {

                    bool rtype = bits[curr++];
                    int reg = GetRegister(curr, bits); curr += 5;

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    if ((!rtype && RegisterValues[reg] == 0) || (rtype && FRegisterValues[reg] == 0)) {
                        Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                        return;
                    }
                    continue;
                }
                if (MainClass.Instructions[s] == "bnez") {

                    bool rtype = bits[curr++];
                    int reg = GetRegister(curr, bits); curr += 5;

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    if ((!rtype && RegisterValues[reg] != 0) || (rtype && FRegisterValues[reg] != 0)) {
                        Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                        return;
                    }
                    continue;
                }
                if (MainClass.Instructions[s] == "bge") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    var val1 = (!rtype ? RegisterValues[reg1] : FRegisterValues[reg1]);
                    var val2 = (!rtype ? RegisterValues[reg2] : FRegisterValues[reg2]);

                    if (val1 >= val2) {
                        Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                        return;
                    }
                    continue;
                }
                if (MainClass.Instructions[s] == "ble") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    var val1 = (!rtype ? RegisterValues[reg1] : FRegisterValues[reg1]);
                    var val2 = (!rtype ? RegisterValues[reg2] : FRegisterValues[reg2]);

                    if (val1 <= val2) {
                        Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                        return;
                    }
                    continue;
                }
                if (MainClass.Instructions[s] == "bgt") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    var val1 = (!rtype ? RegisterValues[reg1] : FRegisterValues[reg1]);
                    var val2 = (!rtype ? RegisterValues[reg2] : FRegisterValues[reg2]);

                    if (val1 > val2) {
                        Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                        return;
                    }
                    continue;
                }
                if (MainClass.Instructions[s] == "fgt.s") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    bool rtype3 = bits[curr++];
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    if (!(FRegisterValues[reg2] > FRegisterValues[reg3]))
                        FRegisterValues[reg1] = 0;

                    continue;
                }
                if (MainClass.Instructions[s] == "flt.s") {

                    bool rtype = bits[curr++];
                    int reg1 = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg2 = GetRegister(curr, bits); curr += 5;

                    bool rtype3 = bits[curr++];
                    int reg3 = GetRegister(curr, bits); curr += 5;

                    if (!(FRegisterValues[reg2] < FRegisterValues[reg3]))
                        FRegisterValues[reg1] = 0;

                    continue;
                }
                if (MainClass.Instructions[s] == "addi") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    curr++;
                    int reg_op = GetRegister(curr, bits); curr += 5;

                    long constant = GetConstant(ref curr, bits);

                    if (!rtype)
                        RegisterValues[reg_dest] = RegisterValues[reg_op] + constant;
                    else
                        FRegisterValues[reg_dest] = RegisterValues[reg_op] + constant;

                    continue;
                }
                if (MainClass.Instructions[s] == "j") {

                    bool jumpType = bits[curr++];
                    byte label = GetByte(ref curr, bits);

                    Execute(GetLabelJumpPos(label, curr, jumpType), bits);
                    return;
                }
                if (MainClass.Instructions[s] == "mv") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    if (rtype && rtype2)
                        FRegisterValues[reg_dest] = FRegisterValues[reg_source];
                    else if (!rtype && rtype2)
                        RegisterValues[reg_dest] = (int)FRegisterValues[reg_source];
                    else if (rtype && !rtype2)
                        FRegisterValues[reg_dest] = RegisterValues[reg_source];
                    else
                        RegisterValues[reg_dest] = RegisterValues[reg_source];

                    continue;
                }
                if (MainClass.Instructions[s] == "fmv.s.x") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;
                    FRegisterValues[reg_dest] = (float)RegisterValues[reg_source];

                    continue;
                }
                if (MainClass.Instructions[s] == "fmv.s") {

                    bool rtype = bits[curr++];
                    int reg_dest = GetRegister(curr, bits); curr += 5;

                    bool rtype2 = bits[curr++];
                    int reg_source = GetRegister(curr, bits); curr += 5;

                    FRegisterValues[reg_dest] = (float)FRegisterValues[reg_source];

                    continue;
                }
                if (MainClass.Instructions[s] == "call") {
                    byte label = GetByte(ref curr, bits);

                    if (label == 1) { PrintF(bits); continue; }
                    if(label == 2) { ScanF(bits); continue; }
                    if(label == 3) { Cfunc(bits); continue; }
                    if(label == 4) { FPrintF(bits); continue; }

                    RegisterValues[1] = curr;
                    Execute(LabelPositions[label][0], bits);
                    return;
                }
                if (MainClass.Instructions[s] == "ret") {
                    Execute((int)RegisterValues[1], bits);
                    return;
                }
            }
        }

        static void RecordVars(List<bool> bits, int start, int end){
            while(true){
                byte name = GetByte(ref start, bits);
                if (name == Convert.ToInt32(MainClass.Codes["eof"], 2)) break;
                int pos = GetInt(start, bits); start += 32;
                VariablePositions[name] = pos;
            }
        }

        static void RecordLabels(List<bool> bits, int curr){

            while (curr < bits.Count) {

                string s = "";
                while (curr < bits.Count && !MainClass.Instructions.Keys.Contains(s))
                    s += bits[curr++] ? "1" : "0";

                if (curr == bits.Count) break;

                //Console.WriteLine(s);

                if (MainClass.Instructions[s] == "label") {
                    byte label = GetByte(ref curr, bits);
                    RegisterLabel(label, curr);
                    continue;
                }

                bool foundInstruction = false;
                foreach (string instruction in MainClass.Codes.Keys)
                    if (instruction != "eof" && MainClass.Instructions[s] == instruction) {
                        curr += MainClass.InstructionSizes[instruction];
                        foundInstruction = true; break; 
                    }
                if (foundInstruction) continue;
                break;
            }

        }

        static void WriteEndStatus() {

            StreamWriter text = new StreamWriter(File.Open(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/status.txt", FileMode.Create));
            foreach(string s in MainClass.RegisterCodes.Keys)
                if (s[0] == 'f') 
                    text.WriteLine(s + ": " + FRegisterValues[GetRegisterId(MainClass.RegisterCodes[s])]);
                else
                    text.WriteLine(s + ": " + RegisterValues[GetRegisterId(MainClass.RegisterCodes[s])]);

            text.Flush();
        }

        static byte GetRegisterId(bool[] bits){
            byte b = 0;
            for (byte i = 0, a = 16; i < 5; ++i, a >>= 1)
                if (bits[i])
                    b += a;
            return b;
        }

        static void PrintF(List<bool> bits){
            int address = (int)RegisterValues[10] * 8; 
            string s = "";
            byte b = 1;
            while(b != 0){
                b = GetByte(ref address, bits);
                s += Convert.ToChar(b);
            }
            printf(s, __arglist(RegisterValues[11], RegisterValues[12], RegisterValues[13], RegisterValues[14], RegisterValues[15], RegisterValues[16], RegisterValues[17]));
        }
        static void FPrintF(List<bool> bits) {
            int address = (int)RegisterValues[10] * 8;
            string s = "";
            byte b = 1;
            while (b != 0) {
                b = GetByte(ref address, bits);
                s += Convert.ToChar(b);
            }
            printf(s, __arglist(FRegisterValues[10], FRegisterValues[11], FRegisterValues[12], FRegisterValues[13], FRegisterValues[14], RegisterValues[15], FRegisterValues[16]));
        }

        static void ScanF(List<bool> bits) {
            int address = (int)RegisterValues[10] * 8;
            string s = "";
            byte b = 1;
            while (b != 0) {
                b = GetByte(ref address, bits);
                s += Convert.ToChar(b);
            }

            int x = -1, y = -1, w = -1, z = -1, t = -1;

            scanf(s, __arglist(ref x, ref y, ref w, ref z, ref t));

            if (x != -1)
                WriteWordAt((int)RegisterValues[11]*8, x, bits);
            if (y != -1)
                WriteWordAt((int)RegisterValues[12]*8, y, bits);
            if (w != -1)
                WriteWordAt((int)RegisterValues[13]*8, w, bits);
            if (z != -1)
                WriteWordAt((int)RegisterValues[14]*8, z, bits);
            if (t != -1)
                WriteWordAt((int)RegisterValues[15]*8, t, bits);
        }

        static void Cfunc(List<bool> bits) => RegisterValues[10] = RegisterValues[10] + RegisterValues[11] * RegisterValues[12];
        
        static int GetLabelJumpPos(byte label, int curr, bool jumpType){

            if (jumpType) {
                int index = curr;
                for (int i = 0; i < LabelPositions[label].Count; ++i)
                    if (LabelPositions[label][i] > curr) {
                        index = i;
                        break;
                    }
                return LabelPositions[label][index];
            }
            else {
                int index = curr;
                for (int i = LabelPositions[label].Count-1; i >= 0; --i)
                    if (LabelPositions[label][i] < curr) {
                        index = i;
                        break;
                    }
                return LabelPositions[label][index];
            }
        }
        static void RegisterLabel(byte label, int pos){

            if (!LabelPositions.Keys.Contains(label))
                LabelPositions[label] = new List<int>();

            LabelPositions[label].Add(pos);
        }

        static bool IsInstruction(string t, int curr, List<bool> bits){
            string s = "";
            while(s.Length < MainClass.Codes[t].Length && curr < bits.Count)
                s += bits[curr++] ? "1" : "0";
            return s == MainClass.Codes[t] && curr < bits.Count;
        }

        static byte GetRegister(int curr, List<bool> bits){
            byte b = 0, a = 16;
            for (int i = curr; i < curr + 5; i++, a >>= 1)
                if (bits[i])
                    b += a;
            return b;
        }

        static void WriteWordAt(int address, int d, List<bool> bits) {

            string binary = Convert.ToString(d, 2);

            while (binary.Length < 32)
                binary = '0' + binary;

            for (int i = 0; i < 32; ++i)
                bits[address + i] = (binary[i] == '1');
        }

        static void WriteDoubleAt(int address, long d, List<bool> bits){

            string binary = Convert.ToString(d, 2);

            while (binary.Length < 64)
                binary = '0' + binary;

            for (int i = 0; i < 64; ++i)
                bits[address + i] = (binary[i] == '1');
        }

        static void WriteSingleAt(int address, float f, List<bool> bits) {

            string binary = FloatToBinary(f);

            for (int i = 0; i < 32; ++i)
                bits[address + i] = (binary[i] == '1');
        }
        static string FloatToBinary(float f) {
            StringBuilder sb = new StringBuilder();
            Byte[] ba = BitConverter.GetBytes(f);
            foreach (Byte b in ba)
                for (int i = 0; i < 8; i++) {
                    sb.Insert(0, ((b >> i) & 1) == 1 ? "1" : "0");
                }
            string s = sb.ToString();
            return s;
        }
        static void WriteByteAt(int address, byte b, List<bool> bits) {
            string binary = Convert.ToString(b, 2);
            while (binary.Length < 8)
                binary = '0' + binary;

            for (int i = 0; i < 8; ++i)
                bits[address + i] = binary[i] == '1';
        }

        static long GetDouble(int curr, List<bool> bits){
            string bin = "";
            for (int i = 0; i < 64; ++i)
                bin += (bits[curr + i] ? "1" : "0");
            return Convert.ToInt64(bin, 2);
        }

        static double GetFloatDouble(int curr, List<bool> bits) {
            string bin = "";
            for (int i = 0; i < 64; ++i)
                bin += (bits[curr + i] ? "1" : "0");
            return BitConverter.ToDouble(BitConverter.GetBytes(Convert.ToInt64(bin, 2)));
        }

        static float GetFloatSingle(int curr, List<bool> bits) {
            string bin = "";
            for (int i = 0; i < 32; ++i)
                bin += (bits[curr + i] ? "1" : "0");
            return BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(bin, 2)));

        }

        static int GetInt(int curr, List<bool> bits){
            string bin = "";
            for (int i = 0; i < 32; ++i)
                bin += (bits[curr + i] ? "1" : "0");
            return Convert.ToInt32(bin, 2);
        }

        static byte GetByte(int curr, List<bool> bits){
            byte b = 0;
            for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                b += bits[curr++] ? a : (byte)0;
            return b;
        }
        static byte GetByte(ref int curr, List<bool> bits) {
            byte b = 0;
            for (byte i = 0, a = 128; i < 8; ++i, a >>= 1)
                b += bits[curr++] ? a : (byte)0;
            return b;
        }
        static long GetConstant(ref int curr, List<bool> bits) {
            string t = "";
            for (int i = 0; i < 64; ++i)
                t += bits[curr++] ? "1" : "0";
            return Convert.ToInt64(t, 2);  
        }

        static double GetConstantDouble(ref int curr, List<bool> bits){
            string t = "";
            for (int i = 0; i < 64; ++i)
                t += bits[curr++] ? "1" : "0";
            return BitConverter.ToDouble(BitConverter.GetBytes(Convert.ToInt64(t, 2)), 0);
        }

        static List<bool> GetBytes(BinaryReader bin){
            long count = bin.BaseStream.Length;
            List<bool> bits = new();

            while(count-- != 0) {

                byte b = bin.ReadByte();

                for (int k = 7, j = 128; k >= 0; k--, j >>= 1) 
                    bits.Add(((b & j) != 0 ? true : false));
            }

            return bits;
        }
    }
}
