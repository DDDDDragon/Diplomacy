using Diplomacy.Utils;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class TradeablePatch
    {
        static TradeablePatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(Tradeable), nameof(Tradeable.GetPriceTooltip)),
                prefix: new HarmonyMethod(typeof(TradeablePatch), nameof(PreGetPriceTooltip)),
                postfix: new HarmonyMethod(typeof(TradeablePatch), nameof(PostGetPriceTooltip)));
        }

        public static bool PreGetPriceTooltip(ref string __result, ref Tradeable __instance, TradeAction action)
        {
            var faction = TradeSession.trader.Faction;

            if (faction != null)
            {
                var relation = faction.PlayerRelationKind;

                if (FactionRelationUtils.CustomFactionRelationKindExist(relation))
                {
                    var custom = FactionRelationUtils.GetCustomFactionRelationKind(relation);

                    return custom.PreGetPriceTooltip(ref __result, ref __instance, action);
                }
            }

            return true;
        }

        public static void PostGetPriceTooltip(ref string __result, ref Tradeable __instance, TradeAction action)
        {
            var faction = TradeSession.trader.Faction;

            if (faction != null)
            {
                var relation = faction.PlayerRelationKind;

                if (FactionRelationUtils.CustomFactionRelationKindExist(relation))
                {
                    var custom = FactionRelationUtils.GetCustomFactionRelationKind(relation);

                    custom.PostGetPriceTooltip(ref __result, ref __instance, action);
                }
            }
        }
    }
}
