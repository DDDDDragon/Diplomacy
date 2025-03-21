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

        public virtual Color GetColor() => Color.white;

        public virtual void RelationKindChangedFrom(FactionRelationKind kind, ref TaggedString text, string reason = null) { }

        public virtual void RelationKindChangedTo(FactionRelationKind kind, ref TaggedString text, string reason = null) { }

        public virtual void FactionDialogFor(ref DiaNode root, Pawn negotiator, Faction faction) { }

        public virtual void RequestMilitaryAidOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator) { }

        public virtual void RequestTraderOption(ref DiaOption diaOption, Map map, Faction faction, Pawn negotiator) { }

        public virtual DiaOption RequestCustomOption(Map map, Faction faction, Pawn negotiator) { return new(); }

        public virtual string AnotherFactionRelationKindChangedReason(Faction other, bool canSendHostilityLetter, GlobalTargetInfo? lookTarget)
        {
            return "";
        }

        public virtual IEnumerable<FloatMenuOption> GetSiteCaravanFloatMenuOptions(Site site, Caravan caravan)
        {
            return new List<FloatMenuOption>();
        }

        public virtual IEnumerable<FloatMenuOption> GetSettlementCaravanFloatMenuOptions(Settlement settlement, Caravan caravan)
        {
            return new List<FloatMenuOption>();
        }
    }
}
