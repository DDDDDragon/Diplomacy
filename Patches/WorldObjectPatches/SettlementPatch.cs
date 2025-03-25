using System;
using Diplomacy.Patches.FactionPatches;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld.Planet;
using Verse;
using System.Linq;
using Diplomacy.Utils;

namespace Diplomacy.Patches.WorldObjectPatches
{
    [StaticConstructorOnStartup]
    public static class SettlementPatch
    {
        static SettlementPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(Settlement), nameof(Settlement.GetFloatMenuOptions)),
                postfix: new HarmonyMethod(typeof(SettlementPatch), nameof(PostGetFloatMenuOptions)));
        }

        public static void PostGetFloatMenuOptions(ref IEnumerable<FloatMenuOption> __result, Settlement __instance, Caravan caravan)
        {
            var faction = __instance.Faction;

            var relation = faction.RelationKindWith(caravan.Faction);

            if (FactionRelationUtils.TryGetCustomFactionRelationKind(relation, out var custom))
            {
                foreach (var option in custom.GetSettlementCaravanFloatMenuOptions(__instance, caravan))
                {
                    __result = __result.Append(option);
                }
            }
        }
    }
}
