using System;
using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Diplomacy.Utils;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class FactionRelationKindUtilityPatch
    {
        static FactionRelationKindUtilityPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(FactionRelationKindUtility), nameof(FactionRelationKindUtility.GetLabel)), 
                postfix: new HarmonyMethod(typeof(FactionRelationKindUtilityPatch), nameof(GetLabel)));
            harmony.Patch(AccessTools.Method(typeof(FactionRelationKindUtility), nameof(FactionRelationKindUtility.GetLabelCap)),
                postfix: new HarmonyMethod(typeof(FactionRelationKindUtilityPatch), nameof(GetLabelCap)));
            harmony.Patch(AccessTools.Method(typeof(FactionRelationKindUtility), nameof(FactionRelationKindUtility.GetColor)),
                postfix: new HarmonyMethod(typeof(FactionRelationKindUtilityPatch), nameof(GetColor)));
        }

        public static void GetLabel(this FactionRelationKind kind, ref string __result)
        {
            if (__result == "error")
                __result = FactionRelationUtils.TryGetCustomFactionRelationKind(kind, out var custom) ? custom.GetLabel() : "error";
        }

        public static void GetLabelCap(this FactionRelationKind kind, ref string __result)
        {
            if (__result == "error")
                __result = FactionRelationUtils.TryGetCustomFactionRelationKind(kind, out var custom) ? custom.GetLabelCap() : "error"; ;
        }

        public static void GetColor(this FactionRelationKind kind, ref Color __result)
        {
            if(__result == Color.white)
                __result = FactionRelationUtils.TryGetCustomFactionRelationKind(kind, out var custom) ? custom.GetColor() : Color.white;
        }
    }
}
