using System.Collections.Generic;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace ExiledGaming.Patches
{
    using System.Reflection.Emit;

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AntiFly))]
    public class AntiFly173Fix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R4 && (float) i.operand == 2.35f) + offset;
            
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldc_R4, 6.75f),
                new CodeInstruction(OpCodes.Stloc_3)
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}