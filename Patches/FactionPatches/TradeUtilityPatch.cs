using System;
using Diplomacy.Patches.FactionPatches.Custom;
using Diplomacy.Utils;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class TradeUtilityPatch
    {
        static TradeUtilityPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(TradeUtility), nameof(TradeUtility.GetPricePlayerBuy)),
                postfix: new HarmonyMethod(typeof(TradeUtilityPatch), nameof(PostGetPricePlayerBuy)));
        }

        public static void PostGetPricePlayerBuy(ref float __result, Thing thing, float priceFactorBuy_TraderPriceType, float priceFactorBuy_JoinAs, float priceGain_PlayerNegotiator, float priceGain_FactionBase)
        {
            var faction = ThingUtils.GetThingFaction(thing); 

            if (faction != null)
            {
                var relation = faction.PlayerRelationKind;

                if (FactionRelationUtils.TryGetCustomFactionRelationKind(relation, out var custom))
                {
                    custom.PostGetPricePlayerBuy(ref __result, thing, priceFactorBuy_TraderPriceType, priceFactorBuy_JoinAs, priceGain_PlayerNegotiator, priceGain_FactionBase);
                }
            }
        }
    }
}
