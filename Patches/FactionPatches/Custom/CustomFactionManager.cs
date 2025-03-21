using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace Diplomacy.Patches.FactionPatches.Custom
{
    [StaticConstructorOnStartup]
    public static class CustomFactionManager
    {
        public static Dictionary<string, Type> CustomFactions = new Dictionary<string, Type>();

        static CustomFactionManager()
        {
            Assembly assembly = typeof(CustomFactionManager).Assembly;

            foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(CustomFaction))))
            {
                CustomFactions.Add(type.Name, type);
            }
        }
    }
}
