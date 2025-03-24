using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diplomacy.Patches.FactionPatches
{

    [StaticConstructorOnStartup]
    public class CustomFactionRelationKindManager
    {
        static CustomFactionRelationKindManager()
        {
            Assembly assembly = typeof(CustomFactionRelationKindManager).Assembly;

            int kind = 3;

            foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(CustomFactionRelationKind))))
            {
                var instance = Activator.CreateInstance(type) as CustomFactionRelationKind;

                CustomRelations.Add(instance, kind);

                kind++;
            }
        }

        internal static Dictionary<CustomFactionRelationKind, int> CustomRelations = new Dictionary<CustomFactionRelationKind, int>();

        internal static bool CustomFactionRelationKindExist(int kind) => CustomRelations.ContainsValue(kind);

        internal static bool CustomFactionRelationKindExist(FactionRelationKind kind) => CustomFactionRelationKindExist((int)kind);

        internal static CustomFactionRelationKind GetCustomFactionRelationKind(string ID) => 
            CustomRelations.Keys.FirstOrDefault(r => r.ID == ID);

        internal static CustomFactionRelationKind GetCustomFactionRelationKind<T>() where T : CustomFactionRelationKind => 
            CustomRelations.Keys.FirstOrDefault(r => r.GetType() == typeof(T));

        internal static CustomFactionRelationKind GetCustomFactionRelationKind(int kind) =>
            CustomRelations.Where(p => p.Value == kind).Count() == 0 ? null : CustomRelations.FirstOrDefault(p => p.Value == kind).Key;

        internal static CustomFactionRelationKind GetCustomFactionRelationKind(FactionRelationKind kind) => GetCustomFactionRelationKind((int)kind);

        internal static FactionRelationKind GetFactionRelationKind(string ID) => 
            (FactionRelationKind)(GetCustomFactionRelationKind(ID) == null ? -1 : CustomRelations[GetCustomFactionRelationKind(ID)]);

        internal static FactionRelationKind GetFactionRelationKind<T>() where T : CustomFactionRelationKind => 
            (FactionRelationKind)(GetCustomFactionRelationKind<T>() == null ? -1 : CustomRelations[GetCustomFactionRelationKind<T>()]);
    }
}
