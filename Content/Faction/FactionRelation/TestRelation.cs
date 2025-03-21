using Diplomacy.Patches.FactionPatches;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace Diplomacy.Content.Faction.FactionRelation
{
    public class TestRelationKind : CustomFactionRelationKind
    {
        public override Color GetColor()
        {
            return Color.yellow;
        }

        public override string GetLabel()
        {
            return "星怒";
        }

        public override string GetLabelCap()
        {
            return "星怒";
        }

        public override string ID => "test1";

        public override string LegalAnotherFactionRelationKindID => "test2";

        public override IEnumerable<FloatMenuOption> GetSettlementCaravanFloatMenuOptions(Settlement settlement, Caravan caravan)
        {
            yield return new FloatMenuOption("超市所有人", delegate
            {

            }, MenuOptionPriority.Default, null, null, 0f, null, settlement, true, 0);
        }
    }
}
