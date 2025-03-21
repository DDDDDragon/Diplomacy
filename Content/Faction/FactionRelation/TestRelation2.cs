using Diplomacy.Patches.FactionPatches;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Diplomacy.Content.Faction.FactionRelation
{
    public class TestRelation2 : CustomFactionRelationKind
    {
        public override Color GetColor()
        {
            return Color.yellow;
        }

        public override string GetLabel()
        {
            return "主人";
        }

        public override string GetLabelCap()
        {
            return "主人";
        }

        public override string ID => "test2";

        public override string LegalAnotherFactionRelationKindID => "test1";
    }
}
