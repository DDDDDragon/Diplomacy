using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Diplomacy.Content.CaravanArrivalActions
{
    public class TaxationCaravanArrivalAction : CaravanArrivalAction
    {
        private Settlement settlement;

        public override string Label => "RaiseTaxation".Translate(settlement.Label);

        public override string ReportString => "CaravanTaxation".Translate(settlement.Label);
    
        public TaxationCaravanArrivalAction() { }

        public TaxationCaravanArrivalAction(Settlement settlement)
        {
            this.settlement = settlement;
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
            if (!floatMenuAcceptanceReport)
            {
                return floatMenuAcceptanceReport;
            }

            if (settlement != null && settlement.Tile != destinationTile)
            {
                return false;
            }

            return CanRaiseTaxation(caravan, settlement);
        }

        public override void Arrived(Caravan caravan)
        {
            CameraJumper.TryJumpAndSelect(caravan);
            
            Pawn playerNegotiator = BestCaravanPawnUtility.FindBestNegotiator(caravan, settlement.Faction, settlement.TraderKind);
            
            RaiseTaxation(caravan, playerNegotiator);
        }

        public void RaiseTaxation(Caravan caravan, Pawn negotiator)
        {
            var silver = settlement.Goods.Where(t => t.def == ThingDefOf.Silver);

            var wealth = silver.Sum(thing => thing.stackCount);

            if (wealth > 0)
            {
                var taxation = Math.Min(100 + wealth / 20, wealth);

                int sum = 0;

                foreach(var thing in silver)
                {
                    if (sum == taxation) break;

                    int giveNum = Math.Min(taxation - sum, thing.stackCount);

                    settlement.GiveSoldThingToPlayer(thing, giveNum, negotiator);

                    sum += giveNum;
                }

                Messages.Message("RaiseTaxationNotification".Translate(settlement.Label, taxation.ToString()).ToString(), new LookTargets(settlement), MessageTypeDefOf.PositiveEvent);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref settlement, "settlement");
        }

        public static FloatMenuAcceptanceReport CanRaiseTaxation(Caravan caravan, Settlement settlement)
        {
            return settlement != null && settlement.Spawned && !settlement.HasMap && settlement.Faction != null && settlement.Faction != Faction.OfPlayer && !settlement.Faction.def.permanentEnemy && !settlement.Faction.HostileTo(Faction.OfPlayer) && settlement.CanTradeNow && HasNegotiator(caravan, settlement);
        }

        public static bool HasNegotiator(Caravan caravan, Settlement settlement)
        {
            Pawn pawn = BestCaravanPawnUtility.FindBestNegotiator(caravan, settlement.Faction, settlement.TraderKind);
            if (pawn != null)
            {
                return !pawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled;
            }

            return false;
        }

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, Settlement settlement)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CanRaiseTaxation(caravan, settlement), () => new TaxationCaravanArrivalAction(settlement), "RaiseTaxation".Translate(settlement.Label), caravan, settlement.Tile, settlement);
        }
    }
}
