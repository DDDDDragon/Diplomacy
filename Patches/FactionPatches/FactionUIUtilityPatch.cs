using Diplomacy.Utils;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using static UnityEngine.Scripting.GarbageCollector;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class FactionUIUtilityPatch
    {
        static FactionUIUtilityPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(typeof(FactionUIUtility).GetMethod("DrawFactionRow", BindingFlags.NonPublic | BindingFlags.Static),
                prefix: new HarmonyMethod(typeof(FactionUIUtilityPatch), nameof(PreDrawFactionRow)), 
                postfix: new HarmonyMethod(typeof(FactionUIUtilityPatch), nameof(PostDrawFactionRow)),
                transpiler: new HarmonyMethod(typeof(FactionUIUtilityPatch), nameof(DrawFactionRowTranspiler)));
        }

        public static bool PreDrawFactionRow(ref float __result, Faction faction, float rowY, Rect fillRect)
        {
            var relationKind = faction.IsPlayer ? -1 : (int)faction.PlayerRelationKind;

            if(FactionRelationUtils.CustomFactionRelationKindExist(relationKind))
            {
                var custom = FactionRelationUtils.GetCustomFactionRelationKind(relationKind);

                return custom.PreDrawFactionRow(ref __result, faction, rowY, fillRect);
            }

            return true;
        }

        public static void PostDrawFactionRow(ref float __result, Faction faction, float rowY, Rect fillRect)
        {
            var relationKind = faction.IsPlayer ? -1 : (int)faction.PlayerRelationKind;

            if (FactionRelationUtils.CustomFactionRelationKindExist(relationKind))
            {
                var custom = FactionRelationUtils.GetCustomFactionRelationKind(relationKind);

                custom.PostDrawFactionRow(ref __result, faction, rowY, fillRect);
            }
        }

        public static IEnumerable<CodeInstruction> DrawFactionRowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            var codes = new List<CodeInstruction>(instructions);

            Label l = ilg.DefineLabel();

            Label endSwitch = ilg.DefineLabel();

            for(int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];

                if (code.opcode == OpCodes.Br && codes[i - 1].opcode == OpCodes.Switch)
                {
                    endSwitch = (Label)code.operand;

                    yield return new CodeInstruction(OpCodes.Br, l);
                }
                else if (code.opcode == OpCodes.Stloc_S && codes[i + 2].operand as string == "\n\n" && (code.operand as LocalBuilder).LocalIndex == 22)
                {
                    yield return code;

                    yield return new CodeInstruction(OpCodes.Br_S, endSwitch);

                    yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = [l] };

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionUIUtilityPatch).GetMethod(nameof(GetFactionRelationKindTip)));

                    yield return code;
                }
                else yield return code;
            }
        }

        public static string GetFactionRelationKindTip(Faction faction)
        {
            var relationKind = faction.IsPlayer ? -1 : (int)faction.PlayerRelationKind;

            if (FactionRelationUtils.CustomFactionRelationKindExist(relationKind))
            {
                var custom = FactionRelationUtils.GetCustomFactionRelationKind(relationKind);

                return custom.Tip(faction);
            }

            return "";
        }
    }
}
