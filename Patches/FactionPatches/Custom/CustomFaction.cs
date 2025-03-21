using RimWorld;
using RimWorld.Planet;
using System;
using System.Reflection;
using Verse;

namespace Diplomacy.Patches.FactionPatches.Custom
{
    public abstract class CustomFaction : Faction
    {
        public CustomFaction() { }

        public CustomFaction(Faction faction) 
        {
            foreach (var factionP in typeof(Faction).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var customP = typeof(CustomFaction).GetProperty(factionP.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (customP != null && customP.CanWrite)
                    customP.SetValue(this, factionP.GetValue(faction));
            }

            foreach (var factionF in typeof(Faction).GetFields(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var customF = typeof(CustomFaction).GetField(factionF.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (customF != null)
                    customF.SetValue(this, factionF.GetValue(faction));
            }
        }

        public CustomFaction LoadFrom(Faction faction)
        {
            foreach (var factionP in typeof(Faction).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var customP = typeof(CustomFaction).GetProperty(factionP.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (customP != null && customP.CanWrite && factionP.GetValue(faction) != null)
                    customP.SetValue(this, factionP.GetValue(faction));
            }

            foreach (var factionF in typeof(Faction).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var customF = typeof(CustomFaction).GetField(factionF.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (customF != null && factionF.GetValue(faction) != null)
                    customF.SetValue(this, factionF.GetValue(faction));
            }

            return this;
        }

        public virtual void CheckKindThresholds(bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter) { sentLetter = false; }

        public virtual bool PreTryAffectGoodwillWith(Faction other, int goodwillChange, bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null, GlobalTargetInfo? lookTarget = null) { return true; }
    }
}
