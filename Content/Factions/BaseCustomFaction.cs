using Diplomacy.Patches.FactionPatches.Custom;
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

namespace Diplomacy.Content.Factions
{
    public class BaseCustomFaction : CustomFaction
    {
        public override bool PreCheckKindThresholds(ref FactionRelation relation, bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
        {
            int num = GoodwillWith(relation.other);
            FactionRelationKind previousKind = relation.kind;

            if (FactionRelationUtils.TryGetCustomFactionRelationKind(previousKind, out var custom))
            {
                if(custom.CheckChangeTo(ref relation, canSendLetter, reason, lookTarget, out sentLetter, out var kind))
                {
                    relation.kind = kind;
                    Notify_CustomRelationKindChanged(relation.other, previousKind, kind, canSendLetter, reason, lookTarget, out sentLetter);
                }

                return false;
            }

            sentLetter = false;
            if (relation.kind != 0 && num <= -75)
            {
                relation.kind = FactionRelationKind.Hostile;
                Notify_RelationKindChanged(relation.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
            }

            if (relation.kind != FactionRelationKind.Ally && num >= 75)
            {
                relation.kind = FactionRelationKind.Ally;
                Notify_RelationKindChanged(relation.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
            }

            if (relation.kind == FactionRelationKind.Hostile && num >= 0)
            {
                relation.kind = FactionRelationKind.Neutral;
                Notify_RelationKindChanged(relation.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
            }

            if (relation.kind == FactionRelationKind.Ally && num <= 0)
            {
                relation.kind = FactionRelationKind.Neutral;
                Notify_RelationKindChanged(relation.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
            }

            return false;
        }

        public override bool PreTryAffectGoodwillWith(ref bool __result, Faction other, int goodwillChange, bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null, GlobalTargetInfo? lookTarget = null)
        {
            if (FactionRelationUtils.TryGetCustomFactionRelationKind(RelationWith(other).kind, out var custom))
            {
                if (!custom.AffectedByGoodwill)
                {
                    __result = false;

                    return false;
                }
            }

            if (!CanChangeGoodwillFor(other, goodwillChange))
            {
                __result = false;
            }

            if (goodwillChange == 0)
            {
                __result = true;
            }

            int num = GoodwillWith(other);
            goodwillChange = CalculateAdjustedGoodwillChange(other, goodwillChange);
            int num2 = BaseGoodwillWith(other);
            int num3 = Mathf.Clamp(num2 + goodwillChange, -100, 100);
            if (num2 == num3)
            {
                __result = true;
            }

            if (reason != null && (IsPlayer || other.IsPlayer))
            {
                Faction arg = (IsPlayer ? other : this);
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(reason, arg.Named(HistoryEventArgsNames.AffectedFaction), goodwillChange.Named(HistoryEventArgsNames.CustomGoodwill)));
            }

            FactionRelation factionRelation = RelationWith(other);
            factionRelation.baseGoodwill = num3;
            factionRelation.CheckKindThresholds(this, canSendHostilityLetter, reason?.LabelCap ?? ((TaggedString)null), lookTarget ?? GlobalTargetInfo.Invalid, out var sentLetter);
            FactionRelation factionRelation2 = other.RelationWith(this);
            FactionRelationKind kind = factionRelation2.kind;
            factionRelation2.baseGoodwill = factionRelation.baseGoodwill;
            factionRelation2.kind = factionRelation.kind;
            bool sentLetter2;
            if (kind != factionRelation2.kind)
            {
                other.Notify_RelationKindChanged(this, kind, canSendHostilityLetter, reason?.LabelCap ?? ((TaggedString)null), lookTarget ?? GlobalTargetInfo.Invalid, out sentLetter2);
            }
            else
            {
                sentLetter2 = false;
            }

            int num4 = GoodwillWith(other);
            if (canSendMessage && num != num4 && !sentLetter && !sentLetter2 && Current.ProgramState == ProgramState.Playing && (IsPlayer || other.IsPlayer))
            {
                Faction faction = (IsPlayer ? other : this);
                string text = ((reason == null) ? ((string)"MessageGoodwillChanged".Translate(faction.Name, num.ToString("F0"), num4.ToString("F0"))) : ((string)"MessageGoodwillChangedWithReason".Translate(faction.Name, num.ToString("F0"), num4.ToString("F0"), reason.label)));
                Messages.Message(text, lookTarget ?? GlobalTargetInfo.Invalid, ((float)goodwillChange > 0f) ? MessageTypeDefOf.PositiveEvent : MessageTypeDefOf.NegativeEvent);
            }

            return false;
        }
    }
}
