using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomacy.Patches.FactionPatches.Custom
{
    public static class CustomFactionFactory<T> where T : CustomFaction
    {
        public static CustomFaction NewInstance(Faction faction) => Activator.CreateInstance<T>().LoadFrom(faction);
    }
}
