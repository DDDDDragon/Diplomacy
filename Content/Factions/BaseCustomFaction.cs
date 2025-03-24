using Diplomacy.Patches.FactionPatches.Custom;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diplomacy.Content.Factions
{
    public class BaseCustomFaction : CustomFaction
    {
        public override void CheckKindThresholds(ref RimWorld.FactionRelation relation, bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
        {
            int num = GoodwillWith(relation.other);
            FactionRelationKind previousKind = relation.kind;
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
        }
    }
}
