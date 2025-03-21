using Diplomacy.Patches.FactionPatches.Custom;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomacy.Content.Faction
{
    public class TestFaction : CustomFaction
    {
        public override bool PreTryAffectGoodwillWith(RimWorld.Faction other, int goodwillChange, bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null, GlobalTargetInfo? lookTarget = null)
        {
            return false;
        }

        public override void CheckKindThresholds(bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
        {
            base.CheckKindThresholds(canSendLetter, reason, lookTarget, out sentLetter);
        }
    }
}
