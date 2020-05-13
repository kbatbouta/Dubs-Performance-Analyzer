using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DubsAnalyzer
{
    public static class TranspilerUtility
    {
        public static bool IsFunctionCall(OpCode instruction)
        {
            return (instruction == OpCodes.Call || instruction == OpCodes.Callvirt || instruction == OpCodes.Calli);
        }

        public static bool IsLoadOperation(OpCode instruction)
        {
            // anything ld is related to loading to stack, anthing sts is static value manip
            return (instruction.Name.Contains("ld") || instruction.Name.Contains("sts"));
        }

        public static int SafeBeforeFuncCall(List<CodeInstruction> instructions, int index)
        {
            // Go backwards
            for(int i = index; i > 0; i--)
            {
                if(!(IsLoadOperation(instructions[i].opcode) || instructions[i].labels?.Count != 0))
                {
                    return i+1;
                }
            }

            return 0;
        }
        public static int SafeAfterFuncCall(List<CodeInstruction> instructions, int index)
        {
            // Go backwards
            for (int i = index; i < instructions.Count; i++)
            {
                if (!(IsLoadOperation(instructions[i].opcode) || instructions[i].labels?.Count != 0))
                {
                    return i;
                }
            }

            return instructions.Count - 1;
        }
    }
}
