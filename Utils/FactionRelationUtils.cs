using Diplomacy.Patches.FactionPatches;
using RimWorld;

namespace Diplomacy.Utils
{
    public class FactionRelationUtils
    {
        public static CustomFactionRelationKind GetCustomFactionRelationKind<T>() where T : CustomFactionRelationKind => CustomFactionRelationKindManager.GetCustomFactionRelationKind<T>();

        public static CustomFactionRelationKind GetCustomFactionRelationKind(string ID) => CustomFactionRelationKindManager.GetCustomFactionRelationKind(ID);

        public static CustomFactionRelationKind GetCustomFactionRelationKind(int kind) => CustomFactionRelationKindManager.GetCustomFactionRelationKind(kind);

        public static CustomFactionRelationKind GetCustomFactionRelationKind(FactionRelationKind kind) => GetCustomFactionRelationKind((int)kind);

        public static FactionRelationKind GetFactionRelationKind<T>() where T : CustomFactionRelationKind => CustomFactionRelationKindManager.GetFactionRelationKind<T>();

        public static FactionRelationKind GetFactionRelationKind(string ID) => CustomFactionRelationKindManager.GetFactionRelationKind(ID);

        public static bool CustomFactionRelationKindExist(int kind) => CustomFactionRelationKindManager.CustomFactionRelationKindExist(kind, out _);

        public static bool CustomFactionRelationKindExist(FactionRelationKind kind) => CustomFactionRelationKindExist((int)kind);

        public static bool TryGetCustomFactionRelationKind(int kind, out CustomFactionRelationKind custom) => CustomFactionRelationKindManager.CustomFactionRelationKindExist(kind, out custom);

        public static bool TryGetCustomFactionRelationKind(FactionRelationKind kind, out CustomFactionRelationKind custom) => CustomFactionRelationKindManager.CustomFactionRelationKindExist(kind, out custom);
    }
}
