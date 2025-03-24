using Diplomacy.Patches.FactionPatches;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Diplomacy.Content.Factions.FactionRelations
{
    public class SuzerainRelation : CustomFactionRelationKind
    {
        public override Color GetColor()
        {
            return Color.yellow;
        }

        public override string GetLabel()
        {
            return "SuzerainLow".Translate();
        }

        public override string GetLabelCap()
        {
            return "Suzerain".Translate();
        }

        public override string ID => "Suzerain";

        public override string LegalAnotherFactionRelationKindID => "Vassal";
    }
}
