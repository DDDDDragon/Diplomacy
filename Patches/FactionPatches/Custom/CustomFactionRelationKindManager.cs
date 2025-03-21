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

        public static Dictionary<CustomFactionRelationKind, int> CustomRelations = new Dictionary<CustomFactionRelationKind, int>();

        public static bool CustomRelationExist(int kind) => CustomRelations.ContainsValue(kind);

        public static bool CustomRelationExist(FactionRelationKind kind) => CustomRelationExist((int)kind);

        public static CustomFactionRelationKind GetCustomFactionRelation(string ID) => 
            CustomRelations.Keys.FirstOrDefault(r => r.ID == ID);

        public static CustomFactionRelationKind GetCustomFactionRelation<T>() where T : CustomFactionRelationKind => 
            CustomRelations.Keys.FirstOrDefault(r => r.GetType() == typeof(T));

        public static CustomFactionRelationKind GetCustomFactionRelation(int kind) =>
            CustomRelations.Where(p => p.Value == kind).Count() == 0 ? null : CustomRelations.FirstOrDefault(p => p.Value == kind).Key;

        public static CustomFactionRelationKind GetCustomFactionRelation(FactionRelationKind kind) => GetCustomFactionRelation((int)kind);

        public static FactionRelationKind GetCustomFactionRelationKind(string ID) => 
            (FactionRelationKind)(GetCustomFactionRelation(ID) == null ? -1 : CustomRelations[GetCustomFactionRelation(ID)]);

        public static FactionRelationKind GetCustomFactionRelationKind<T>() where T : CustomFactionRelationKind => 
            (FactionRelationKind)(GetCustomFactionRelation<T>() == null ? -1 : CustomRelations[GetCustomFactionRelation<T>()]);
    }
}
