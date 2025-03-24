using Diplomacy.Patches.FactionPatches.Custom;
using RimWorld;
using RimWorld.Planet;

namespace Diplomacy.Content.Factions
{
    public class TestFaction : CustomFaction
    {
        public override bool PreTryAffectGoodwillWith(Faction other, int goodwillChange, bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null, GlobalTargetInfo? lookTarget = null)
        {
            return false;
        }

        public override void CheckKindThresholds(ref FactionRelation relation, bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
        {
            base.CheckKindThresholds(ref relation, canSendLetter, reason, lookTarget, out sentLetter);
        }
    }
}
