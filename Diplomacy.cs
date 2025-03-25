using Diplomacy.Content.Factions;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using Diplomacy.Content.Factions.FactionRelations;
using Diplomacy.Utils;
using Diplomacy.Content.GameComponents.Vassal;
using System.Linq;

namespace Diplomacy
{
    public class Diplomacy : Mod
    {
        public static Harmony Harmony = new Harmony("Mantodea.Diplomacy");

        public Diplomacy(ModContentPack content) : base(content) 
        {
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                if (Current.Game != null)
                {
                    Current.Game.GetComponent<VassalChecks>();
                }
            });
        }

        [DebugAction("Showcase", "Diplomacy", actionType = DebugActionType.ToolMap)]
        public static void DebugAction()
        {
            var list = Find.FactionManager.AllFactionsListForReading;

            var faction = list.First();

            var relations = typeof(Faction).GetField("relations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(faction) as List<FactionRelation>;

            foreach (var relation in relations)
            {
                if (relation.other.IsPlayer)
                    FactionRelationUtils.GetCustomFactionRelationKind<VassalRelation>().SetRelation(relation);
            }

            VassalChecks.AddNewVassal(faction);
        }
    }
}
