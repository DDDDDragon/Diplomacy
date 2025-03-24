using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Diplomacy.Utils
{
    public class FactionUIUtils
    {
        public static bool showAll => getShowAll();

        public static List<int> tmpTicks => getTmpTicks();

        public static List<int> tmpCustomGoodwill = new List<int>();

        private static bool getShowAll()
        {
            return (bool)typeof(FactionUIUtility).GetField("showAll", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }

        private static List<int> getTmpTicks()
        {
            return (List<int>)typeof(FactionUIUtility).GetField("tmpTicks", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }

        private static List<int> getTmpCustomGoodwill()
        {
            return (List<int>)typeof(FactionUIUtility).GetField("tmpCustomGoodwill", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }

        public static FactionRelationKind GetRelationKindForGoodwill(int goodwill)
        {
            if (goodwill <= -75)
            {
                return FactionRelationKind.Hostile;
            }

            if (goodwill >= 75)
            {
                return FactionRelationKind.Ally;
            }

            return FactionRelationKind.Neutral;
        }

        public static TaggedString GetOngoingEvents(Faction other)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<GoodwillSituationManager.CachedSituation> situations = Find.GoodwillSituationManager.GetSituations(other);
            for (int i = 0; i < situations.Count; i++)
            {
                if (situations[i].maxGoodwill < 100)
                {
                    string text = "- " + situations[i].def.Worker.GetPostProcessedLabelCap(other);
                    text = text + ": " + (situations[i].maxGoodwill.ToStringWithSign() + " " + "max".Translate()).Colorize(FactionRelationKind.Hostile.GetColor());
                    stringBuilder.AppendInNewLine(text);
                }
            }

            return stringBuilder.ToString();
        }

        public static TaggedString GetRecentEvents(Faction other)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<HistoryEventDef> allDefsListForReading = DefDatabase<HistoryEventDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                int recentCountWithinTicks = Find.HistoryEventsManager.GetRecentCountWithinTicks(allDefsListForReading[i], 3600000, other);
                if (recentCountWithinTicks <= 0)
                {
                    continue;
                }

                Find.HistoryEventsManager.GetRecent(allDefsListForReading[i], 3600000, tmpTicks, tmpCustomGoodwill, other);
                int num = 0;
                for (int j = 0; j < tmpTicks.Count; j++)
                {
                    num += tmpCustomGoodwill[j];
                }

                if (num != 0)
                {
                    string text = "- " + allDefsListForReading[i].LabelCap;
                    if (recentCountWithinTicks != 1)
                    {
                        text = text + " x" + recentCountWithinTicks;
                    }

                    text = text + ": " + num.ToStringWithSign().Colorize((num >= 0) ? FactionRelationKind.Ally.GetColor() : FactionRelationKind.Hostile.GetColor());
                    stringBuilder.AppendInNewLine(text);
                }
            }

            return stringBuilder.ToString();
        }

        public static TaggedString GetNaturalGoodwillExplanation(Faction other)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<GoodwillSituationManager.CachedSituation> situations = Find.GoodwillSituationManager.GetSituations(other);
            for (int i = 0; i < situations.Count; i++)
            {
                if (situations[i].naturalGoodwillOffset != 0)
                {
                    string text = "- " + situations[i].def.Worker.GetPostProcessedLabelCap(other);
                    text = text + ": " + situations[i].naturalGoodwillOffset.ToStringWithSign().Colorize((situations[i].naturalGoodwillOffset >= 0) ? FactionRelationKind.Ally.GetColor() : FactionRelationKind.Hostile.GetColor());
                    stringBuilder.AppendInNewLine(text);
                }
            }

            return stringBuilder.ToString();
        }

        public static void DrawFactionIconWithTooltip(Rect r, Faction faction)
        {
            GUI.color = faction.Color;
            GUI.DrawTexture(r, (Texture)faction.def.FactionIcon);
            GUI.color = Color.white;
            if (Mouse.IsOver(r))
            {
                TipSignal tip = new TipSignal(() => faction.Name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + faction.def.LabelCap.Resolve() + "\n\n" + faction.def.Description, faction.loadID ^ 0x738AC053);
                TooltipHandler.TipRegion(r, tip);
                Widgets.DrawHighlight(r);
            }

            if (Widgets.ButtonInvisible(r, doMouseoverSound: false))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(faction));
            }
        }
    }
}
