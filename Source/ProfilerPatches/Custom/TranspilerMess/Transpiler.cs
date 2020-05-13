using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DubsAnalyzer
{
    public static class GenericTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            // We need to get all methods
            // Opcode of call, callVirt, calli
            List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
            List<CodeInstruction> modifiedList = new List<CodeInstruction>(instructions);


            for (int i = 0; i < instructionList.Count; i++)
            {
                if (TranspilerUtility.IsFunctionCall(instructionList[i].opcode))
                {
                    Log.Message("");

                    int index = TranspilerUtility.SafeBeforeFuncCall(instructionList, i-1);
                    Log.Error($"Index: {index} Opcode: {instructionList[index].opcode.ToString()} Operand: {instructionList[index].operand?.ToString()} Label Count { instructionList[index].labels?.Count }");

                    Log.Warning($"Index: {i} Opcode: {instructionList[i].opcode.ToString()} Operand: {instructionList[i].operand?.ToString()} Label Count { instructionList[i].labels?.Count }");

                    index = TranspilerUtility.SafeAfterFuncCall(instructionList, i+1);
                    Log.Error($"Index: {index} Opcode: {instructionList[index].opcode.ToString()} Operand: {instructionList[index].operand?.ToString()} Label Count { instructionList[index].labels?.Count }");
                }

            }

            return modifiedList.AsEnumerable();
        }

    }
}
