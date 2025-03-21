using Diplomacy.Patches.FactionPatches;
using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using Diplomacy.Patches.FactionPatches.Custom;

namespace Diplomacy.Patches
{
    [StaticConstructorOnStartup]
    public static class FactionDialogMakerPatch
    {
        static FactionDialogMakerPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(FactionDialogMaker), nameof(FactionDialogMaker.FactionDialogFor)), 
                transpiler: new HarmonyMethod(typeof(FactionDialogMakerPatch), nameof(FactionDialogForTranspiler)));

            harmony.Patch(typeof(FactionDialogMaker).GetMethod("RequestMilitaryAidOption", BindingFlags.NonPublic | BindingFlags.Static),
                transpiler: new HarmonyMethod(typeof(FactionDialogMakerPatch), nameof(RequestMilitaryAidOptionTranspiler)));

            harmony.Patch(typeof(FactionDialogMaker).GetMethod("RequestTraderOption", BindingFlags.NonPublic | BindingFlags.Static),
                transpiler: new HarmonyMethod(typeof(FactionDialogMakerPatch), nameof(RequestTraderOptionTranspiler)));
        }

        public static IEnumerable<CodeInstruction> FactionDialogForTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            var codes = new List<CodeInstruction>(instructions);

            bool added = false;

            foreach (var code in codes)
            {
                if (code.opcode == OpCodes.Br)
                {
                    Label skip = ilg.DefineLabel();

                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    yield return new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Faction).GetProperty(nameof(Faction.PlayerRelationKind)).GetGetMethod()
                    );

                    yield return new CodeInstruction(
                        OpCodes.Call,
                        typeof(CustomFactionRelationKindManager).GetMethod(nameof(CustomFactionRelationKindManager.CustomRelationExist), [typeof(FactionRelationKind)])
                    );

                    yield return new CodeInstruction(OpCodes.Brfalse, skip);

                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);

                    yield return new CodeInstruction(OpCodes.Ldflda, typeof(FactionDialogMaker).GetNestedType("<>c__DisplayClass0_0", BindingFlags.NonPublic).GetField("root"));

                    yield return new CodeInstruction(OpCodes.Ldarg_0);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionDialogMakerPatch).GetMethod(nameof(FactionDialogFor)));

                    code.labels = [skip];

                    yield return code;
                }

                else if (code.Calls(typeof(FactionDialogMaker).GetMethod("<FactionDialogFor>g__AddAndDecorateOption|0_0", BindingFlags.Static | BindingFlags.NonPublic)) && !added)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    yield return new CodeInstruction(OpCodes.Ldloc_0);

                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(FactionDialogMaker).GetNestedType("<>c__DisplayClass0_0", BindingFlags.NonPublic).GetField("negotiator"));

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionDialogMakerPatch).GetMethod("RequestCustomOption"));

                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);

                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionDialogMaker).GetMethod("<FactionDialogFor>g__AddAndDecorateOption|0_0", BindingFlags.Static | BindingFlags.NonPublic));

                    yield return code;

                    added = true;
                }

                else yield return code;
            }
        }

        public static IEnumerable<CodeInstruction> RequestMilitaryAidOptionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            bool added = false;

            foreach (var code in codes)
            {
                if (code.Calls(typeof(DiaOption).GetMethod(nameof(DiaOption.Disable))) && !added)
                {
                    added = true;

                    yield return code;

                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    yield return new CodeInstruction(OpCodes.Ldloca, 0);

                    yield return new CodeInstruction(OpCodes.Ldarg_0);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    yield return new CodeInstruction(OpCodes.Ldarg_2);

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionDialogMakerPatch).GetMethod(nameof(RequestMilitaryAidOption)));
                }
                else yield return code;
            }
        }

        public static IEnumerable<CodeInstruction> RequestTraderOptionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            bool added = false;

            foreach (var code in codes)
            {
                if (code.Calls(typeof(DiaOption).GetMethod(nameof(DiaOption.Disable))) && !added)
                {
                    added = true;

                    yield return code;

                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    yield return new CodeInstruction(OpCodes.Ldloca, 0);

                    yield return new CodeInstruction(OpCodes.Ldarg_0);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    yield return new CodeInstruction(OpCodes.Ldarg_2);

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionDialogMakerPatch).GetMethod(nameof(RequestTraderOption)));
                }
                else yield return code;
            }
        }

        public static void FactionDialogFor(ref DiaNode root, Pawn negotiator, Faction faction)
        {
            var relationKind = faction.PlayerRelationKind;

            if (CustomFactionRelationKindManager.CustomRelationExist(relationKind))
                CustomFactionRelationKindManager.GetCustomFactionRelation(relationKind).FactionDialogFor(ref root, negotiator, faction);
        }

        public static void RequestMilitaryAidOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator)
        {
            var relationKind = faction.PlayerRelationKind;

            if (CustomFactionRelationKindManager.CustomRelationExist(relationKind))
                CustomFactionRelationKindManager.GetCustomFactionRelation(relationKind).RequestMilitaryAidOption(ref diaOption, map, faction, negotiator);
        }

        public static void RequestTraderOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator)
        {
            var relationKind = faction.PlayerRelationKind;

            if (CustomFactionRelationKindManager.CustomRelationExist(relationKind))
                CustomFactionRelationKindManager.GetCustomFactionRelation(relationKind).RequestTraderOption(ref diaOption, map, faction, negotiator);
        }

        public static DiaOption RequestCustomOption(Map map, Faction faction, Pawn negotiator)
        {
            var relationKind = faction.PlayerRelationKind;

            if (CustomFactionRelationKindManager.CustomRelationExist(relationKind))
                return CustomFactionRelationKindManager.GetCustomFactionRelation(relationKind).RequestCustomOption(map, faction, negotiator);

            return new();
        }
    }
}
