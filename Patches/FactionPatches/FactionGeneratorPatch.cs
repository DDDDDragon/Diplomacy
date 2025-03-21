using HarmonyLib;
using RimWorld;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using Diplomacy.Content.Faction.DefExtensions;
using Diplomacy.Patches.FactionPatches.Custom;
using System.Linq;
using System;
using Diplomacy.Content.Faction;

namespace Diplomacy.Patches.FactionPatches
{
    [StaticConstructorOnStartup]
    public static class FactionGeneratorPatch
    {
        static FactionGeneratorPatch()
        {
            var harmony = Diplomacy.Harmony;

            harmony.Patch(AccessTools.Method(typeof(FactionGenerator), nameof(FactionGenerator.NewGeneratedFaction)), 
                transpiler: new HarmonyMethod(typeof(FactionGeneratorPatch), nameof(NewGeneratedFactionTranspiler)));
        }

        public static IEnumerable<CodeInstruction> NewGeneratedFactionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            foreach (var code in codes)
            {
                if (code.opcode == OpCodes.Newobj && (ConstructorInfo)code.operand == typeof(Faction).GetConstructor([]))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);

                    yield return new CodeInstruction(OpCodes.Call, typeof(FactionGeneratorPatch).GetMethod(nameof(GenerateFaction)));
                }
                else yield return code;
            }
        }

        public static Faction GenerateFaction(FactionGeneratorParms parms)
        {
            var def = parms.factionDef;

            var clazz = def.GetModExtension<FactionDefExtension>()?.factionClass;

            if (clazz != null && CustomFactionManager.CustomFactions.Keys.Contains(clazz))
            {
                return Activator.CreateInstance(CustomFactionManager.CustomFactions[clazz]) as Faction;
            }
            else return new BaseCustomFaction();
        }
    }
}
