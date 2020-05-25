﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DubsAnalyzer
{
    public static class InternalMethodUtility
    {
        private static MethodInfo AnalyzerStartMeth = AccessTools.Method(typeof(InternalMethodUtility), nameof(AnalyzerStart));
        private static MethodInfo AnalyzerEndMeth = AccessTools.Method(typeof(InternalMethodUtility), nameof(AnalyzerEnd));

        private static FieldInfo AnalyzerKeyDict = AccessTools.Field(typeof(InternalMethods), "KeyMethods");
        private static MethodInfo AnalyzerGetValue = AccessTools.Method(typeof(Dictionary<string, MethodInfo>), "get_Item");


        public static bool IsFunctionCall(OpCode instruction)
        {
            return (instruction == OpCodes.Call || instruction == OpCodes.Callvirt);// || instruction == OpCodes.Calli);
        }

        public static void LogInstruction(CodeInstruction instruction)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Instruction Opcode: {instruction.opcode}");
            if(IsFunctionCall(instruction.opcode))
            {
                MethodInfo m = instruction.operand as MethodInfo;
                builder.Append($" function: {m.Name}, with the return type of: {m.ReturnType.Name}");
                if (m.GetParameters().Count() != 0)
                {
                    builder.Append(" With the parameters;");
                    foreach (var p in m.GetParameters())
                    {
                        builder.Append($" Type: {p.ParameterType.ToString()}, ");
                    }
                }
                else
                {
                    builder.Append(" With no parameters");
                }
            } 
            if(instruction.labels?.Count != 0)
            {
                foreach (var l in instruction.labels)
                    builder.Append($" with the label: {l.ToString()}");
            }
            Log.Message(builder.ToString());
        }


        public static void InsertStartIL(ILGenerator ilGen, string key)
        {
            ilGen.Emit(OpCodes.Ldstr, key); 
            // load our string to memory

            ilGen.Emit(OpCodes.Ldsfld, AnalyzerKeyDict); // KeyMethods
            ilGen.Emit(OpCodes.Ldstr, key);
            ilGen.Emit(OpCodes.Call, AnalyzerGetValue); // KeyMethods.get_Item(key) or KeyMethods[key]
            // KeyMethods[key]

            ilGen.Emit(OpCodes.Call, AnalyzerStartMeth);
            // AnalyzerStart(key, KeyMethods[key]);
        }

        public static void AnalyzerStart(string key, MethodInfo meth = null)
        {
            Analyzer.Start(key, null, null, null , null, meth);
        }

        public static void InsertEndIL(ILGenerator ilGen, string key)
        {
            ilGen.Emit(OpCodes.Ldstr, key);
            ilGen.Emit(OpCodes.Call, AnalyzerEndMeth);

            ilGen.Emit(OpCodes.Ret);
        }

        private static void AnalyzerEnd(string key)
        {
            Analyzer.Stop(key);
        }
    }
}