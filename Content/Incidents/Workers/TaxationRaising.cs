using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Diplomacy.Content.Incidents.Workers
{
    public class TaxationRaising : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;

            Faction faction = parms.faction;

            int sumTaxation = 0;

            IntVec3 dropAt = DropCellFinder.RandomDropSpot(map);

            var playerSettlement = Find.World.worldObjects.Settlements.FirstOrDefault(s => s.Faction.IsPlayer);

            foreach (var settlement in Find.World.worldObjects.Settlements)
            {
                if(settlement.Faction == faction && Find.WorldGrid.TraversalDistanceBetween(settlement.Tile, playerSettlement.Tile) < 100)
                {
                    var silver = settlement.Goods.Where(t => t.def == ThingDefOf.Silver);

                    var wealth = silver.Sum(thing => thing.stackCount);

                    if (wealth > 0)
                    {
                        var taxation = Math.Min(100 + wealth / 20, wealth);

                        int sum = 0;

                        ActiveDropPodInfo dropPodInfo = new ActiveDropPodInfo();

                        foreach (var thing in silver)
                        {
                            if (sum == taxation) break;

                            int giveNum = Math.Min(taxation - sum, thing.stackCount);

                            dropPodInfo.innerContainer.TryAddOrTransfer(thing, giveNum);

                            sum += giveNum;

                            sumTaxation += giveNum;
                        }

                        DropPodUtility.MakeDropPodAt(dropAt, map, dropPodInfo);
                    }
                }
            }

            Messages.Message("RaiseTaxationNotification".Translate(faction.Name, sumTaxation.ToString()).ToString(), new LookTargets(dropAt, map), MessageTypeDefOf.PositiveEvent);

            return true;
        }
    }
}
