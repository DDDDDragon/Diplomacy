using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diplomacy.Patches.FactionPatches.Custom;
using HarmonyLib;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class FactionRelationPatch
    {
        static FactionRelationPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(FactionRelation), nameof(FactionRelation.CheckKindThresholds)),
                prefix: new HarmonyMethod(typeof(FactionRelationPatch), nameof(PreCheckKindThresholds)));
        }

        public static bool PreCheckKindThresholds(FactionRelation __instance, Faction faction, bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
        {
            sentLetter = false;

            if (faction is CustomFaction customFaction)
            {
                customFaction.CheckKindThresholds(ref __instance, canSendLetter, reason, lookTarget, out var sent);

                sentLetter = sent;

                return false;
            }

            return true;
        }
    }
}
