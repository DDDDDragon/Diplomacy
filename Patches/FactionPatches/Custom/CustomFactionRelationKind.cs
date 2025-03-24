using Diplomacy.Content.GameComponents.Vassal;
using Diplomacy.Utils;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{
    public abstract class CustomFactionRelationKind
    {
        public virtual string ID => "";

        public virtual string LegalAnotherFactionRelationKindID => "";

        public virtual int MaxGoodwill => int.MaxValue;

        public virtual int MinGoodwill => int.MinValue;

        public virtual string GetLabel() => $"{ID}Lower".Translate();

        public virtual string GetLabelCap() => $"{ID}".Translate();

        public virtual string Tip(Faction faction) => "";

        public virtual Color GetColor() => Color.white;

        public virtual void RelationKindChangedFrom(FactionRelationKind kind, ref TaggedString text, string reason = null) { }

        public virtual void RelationKindChangedTo(FactionRelationKind kind, ref TaggedString text, string reason = null) { }

        public virtual void FactionDialogFor(ref DiaNode root, Pawn negotiator, Faction faction) { }

        public virtual void RequestMilitaryAidOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator) { }

        public virtual void RequestTraderOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator) { }

        public virtual DiaOption RequestCustomOption(Map map, Faction faction, Pawn negotiator) { return new(); }

        public virtual string AnotherFactionRelationKindChangedReason(Faction other, bool canSendHostilityLetter, GlobalTargetInfo? lookTarget) => "";

        public virtual IEnumerable<FloatMenuOption> GetSiteCaravanFloatMenuOptions(Site site, Caravan caravan) => new List<FloatMenuOption>();

        public virtual IEnumerable<FloatMenuOption> GetSettlementCaravanFloatMenuOptions(Settlement settlement, Caravan caravan) => new List<FloatMenuOption>();

        public virtual float GetPlayerBuyPriceMultiplier(Faction faction) => 1f;

        public virtual float GetPlayerSellPriceMultiplier(Faction faction) => 1f;

        public virtual bool PreDrawFactionRow(ref float __result, Faction faction, float rowY, Rect fillRect) => true;

        public virtual void PostDrawFactionRow(ref float __result, Faction faction, float rowY, Rect fillRect) { }

        public virtual bool PreGetPriceTooltip(ref string __result, ref Tradeable __instance, TradeAction action) => true;

        public virtual void PostGetPriceTooltip(ref string __result, ref Tradeable __instance, TradeAction action) { }

        public virtual void PostGetPricePlayerBuy(ref float __result, Thing thing, float priceFactorBuy_TraderPriceType, float priceFactorBuy_JoinAs, float priceGain_PlayerNegotiator, float priceGain_FactionBase)
        {
            var faction = ThingUtils.GetThingFaction(thing);

            __result *= GetPlayerBuyPriceMultiplier(faction);
        }

        public virtual void SetRelation(FactionRelation relation)
        {
            relation.kind = FactionRelationUtils.GetFactionRelationKind(ID);

            relation.baseGoodwill = Math.Max(relation.baseGoodwill, MinGoodwill);

            if (MinGoodwill < MaxGoodwill)
                relation.baseGoodwill = Math.Min(relation.baseGoodwill, MaxGoodwill);
        }
    }
}
