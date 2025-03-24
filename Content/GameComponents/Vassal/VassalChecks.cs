using System;
using System.Collections.Generic;
using System.Linq;
using Diplomacy.Content.Factions.FactionRelations;
using Diplomacy.Content.Incidents.Workers;
using Diplomacy.Patches.FactionPatches;
using Diplomacy.Utils;
using RimWorld;
using Verse;
using Verse.Noise;

namespace Diplomacy.Content.GameComponents.Vassal
{
    public class VassalChecks : GameComponent
    {
        private int _ticksCooldown;

        private bool _initialize;

        public static Dictionary<Faction, VassalData> FactionVassalDatas;

        public VassalChecks(Game game)
        {
            _ticksCooldown = 1800000; //30 days
        }

        public static float Tick2Day(int tick)
        {
            return (float)tick / 60000;
        }

        public void Initialize()
        {
            FactionVassalDatas = [];

            foreach (var faction in Find.FactionManager.AllFactionsListForReading)
            {
                if (!faction.IsPlayer && faction.PlayerRelationKind == FactionRelationUtils.GetFactionRelationKind<VassalRelation>())
                {
                    FactionVassalDatas.Add(faction, new(_ticksCooldown, 50));
                }
            }

            _initialize = true;
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            //TODO: Player as vassal situation
            if (!_initialize) Initialize();

            TaxationCheck();
        }

        public void TaxationCheck()
        {
            var vassals = FactionVassalDatas.Keys.ToList();

            foreach (var faction in vassals)
            {
                VassalData data = FactionVassalDatas[faction];

                if (data.TaxationCooldown <= 0)
                {
                    var loyalty = data.Loyalty;

                    var chance = loyalty < 0 ? 0 : (loyalty >= 50 ? 1 : loyalty / 50);

                    if (Rand.Chance(chance))
                    {
                        IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.CurrentMap);

                        parms.faction = faction;

                        IncidentDef.Named("TaxationRaising").Worker.TryExecute(parms);
                    }
                    else
                        Messages.Message("RaiseTaxationNotification_No".Translate(faction.Name).ToString(), null, MessageTypeDefOf.PositiveEvent);

                    data.TaxationCooldown = _ticksCooldown;
                }
                else if (FactionVassalDatas[faction].TaxationCooldown > 0)
                {
                    data.TaxationCooldown--;
                }

                FactionVassalDatas[faction] = data;
            }
        }

        public static void AddNewVassal(Faction faction)
        {
            if (faction.IsPlayer)
                Log.Error("Cannot add player faction.");
            else if (!FactionVassalDatas.ContainsKey(faction))
                FactionVassalDatas.Add(faction, new(0, 50));
            else
                Log.Error("Faction already added to Taxation Cooldown.");
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref _ticksCooldown, "_ticksCooldown");

            Scribe_Values.Look(ref FactionVassalDatas, "FactionVassalDatas");
        }
    }
}
