using Diplomacy.Patches.FactionPatches.Custom;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Linq;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class FactionPatch
    {
        static FactionPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(
                AccessTools.Method(
                    typeof(Faction), 
                    nameof(Faction.TryAppendRelationKindChangedInfo), 
                    [typeof(TaggedString).MakeByRefType(), typeof(FactionRelationKind), typeof(FactionRelationKind), typeof(string)]
                ),
                postfix: new HarmonyMethod(typeof(FactionPatch), nameof(PostTryAppendRelationKindChangedInfo))
            );

            harmony.Patch(AccessTools.Method(typeof(Faction), nameof(Faction.TryAffectGoodwillWith)),
                prefix: new HarmonyMethod(typeof(FactionPatch), nameof(PreTryAffectGoodwillWith)));

            harmony.Patch(AccessTools.Method(typeof(Faction), nameof(Faction.Notify_GoodwillSituationsChanged)),
                prefix: new HarmonyMethod(typeof(FactionPatch), nameof(PreNotify_GoodwillSituationsChanged)));
        }

        public static void PostTryAppendRelationKindChangedInfo(ref TaggedString text, FactionRelationKind previousKind, FactionRelationKind newKind, string reason = null)
        {
            if (CustomFactionRelationKindManager.CustomRelationExist(previousKind))
                CustomFactionRelationKindManager.GetCustomFactionRelation(previousKind).RelationKindChangedTo(newKind, ref text, reason);

            if (CustomFactionRelationKindManager.CustomRelationExist(newKind))
                CustomFactionRelationKindManager.GetCustomFactionRelation(newKind).RelationKindChangedFrom(previousKind, ref text, reason);
        }

        public static bool PreTryAffectGoodwillWith(Faction __instance, Faction other, int goodwillChange, bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null, GlobalTargetInfo? lookTarget = null)
        {
            return (__instance as CustomFaction).PreTryAffectGoodwillWith(other, goodwillChange, canSendMessage, canSendHostilityLetter, reason, lookTarget);
        }

        public static bool PreNotify_GoodwillSituationsChanged(Faction __instance, Faction other, bool canSendHostilityLetter, string reason, GlobalTargetInfo? lookTarget)
        {
            FactionRelation factionRelation = __instance.RelationWith(other, false);
            
            factionRelation.CheckKindThresholds(__instance, canSendHostilityLetter, reason, lookTarget ?? GlobalTargetInfo.Invalid, out var flag);

            FactionRelation factionRelation2 = other.RelationWith(__instance, false);

            var kd1 = factionRelation.kind;

            var kd2 = factionRelation2.kind;

            if(CustomFactionRelationKindManager.CustomRelationExist(kd1) && __instance.IsPlayer)
            {
                var kind = CustomFactionRelationKindManager.GetCustomFactionRelation(kd1);

                var legalID = kind.LegalAnotherFactionRelationKindID;

                if (legalID == "" || CustomFactionRelationKindManager.GetCustomFactionRelation(legalID) == null) 
                    return false;

                var otherID = "";

                if (!CustomFactionRelationKindManager.CustomRelationExist(kd2))
                    otherID = Enum.GetName(typeof(FactionRelationKind), kd2);
                else otherID = CustomFactionRelationKindManager.GetCustomFactionRelation(kd2).ID;

                if (legalID != otherID)
                {
                    factionRelation2.kind = CustomFactionRelationKindManager.GetCustomFactionRelationKind(legalID);

                    var rea = kind.AnotherFactionRelationKindChangedReason(other, canSendHostilityLetter, lookTarget);

                    factionRelation2.baseGoodwill = Math.Max(factionRelation2.baseGoodwill, kind.MinGoodwill);

                    if(kind.MinGoodwill < kind.MaxGoodwill)
                        factionRelation2.baseGoodwill = Math.Min(factionRelation.baseGoodwill, kind.MaxGoodwill);

                    other.Notify_RelationKindChanged(__instance, factionRelation2.kind, canSendHostilityLetter, rea, lookTarget ?? GlobalTargetInfo.Invalid, out flag);
                }
            }
            else if (kd1 != kd2 && !CustomFactionRelationKindManager.CustomRelationExist(kd2))
            {
                factionRelation2.kind = kd1;

                other.Notify_RelationKindChanged(__instance, kd2, canSendHostilityLetter, reason, lookTarget ?? GlobalTargetInfo.Invalid, out flag);
            }

            return false;
        }
    }
}
