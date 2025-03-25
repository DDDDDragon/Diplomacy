using Diplomacy.Patches.FactionPatches;
using Diplomacy.Utils;
using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diplomacy.Patches.WorldObjectPatches
{
    [StaticConstructorOnStartup]
    public static class SitePatch
    {
        static SitePatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(Site), nameof(Site.GetFloatMenuOptions)),
                postfix: new HarmonyMethod(typeof(SitePatch), nameof(PostGetFloatMenuOptions)));
        }

        public static void PostGetFloatMenuOptions(ref IEnumerable<FloatMenuOption> __result, Site __instance, Caravan caravan)
        {
            var faction = caravan.Faction;

            var relation = faction.RelationKindWith(__instance.Faction);

            if(FactionRelationUtils.TryGetCustomFactionRelationKind(relation, out var custom))
            {
                foreach (var option in custom.GetSiteCaravanFloatMenuOptions(__instance, caravan))
                {
                    __result.Append(option);
                }
            }
        }
    }
}
