using Diplomacy.Content.CaravanArrivalActions;
using Diplomacy.Content.GameComponents;
using Diplomacy.Content.GameComponents.Vassal;
using Diplomacy.Patches.FactionPatches;
using Diplomacy.Patches.FactionPatches.Custom;
using Diplomacy.Utils;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Diplomacy.Content.Factions.FactionRelations
{
    public class VassalRelation : CustomFactionRelationKind
    {
        public override Color GetColor() => Color.yellow;

        public override string GetLabel() => "VassalLow".Translate();

        public override string GetLabelCap() => "Vassal".Translate();

        public override string ID => "Vassal";

        public override int MaxGoodwill => 75;

        public override int MinGoodwill => 75;

        public override string LegalAnotherFactionRelationKindID => "Suzerain";

        public override float GetPlayerBuyPriceMultiplier(Faction faction)
        {
            var loyalty = VassalChecks.FactionVassalDatas[faction].Loyalty;

            return loyalty < 0 ? 1 : (1000 - (float)loyalty) / 1000;
        }

        public override string Tip(Faction faction) => "VassalTip".Translate();

        public override bool PreDrawFactionRow(ref float __result, Faction faction, float rowY, Rect fillRect)
        {
            float num = fillRect.width - 300f - 40f - 70f - 54f - 16f - 120f;
            Faction[] array = Find.FactionManager.AllFactionsInViewOrder.Where((Faction f) => f != faction && f.HostileTo(faction) && ((!f.IsPlayer && !f.Hidden) || FactionUIUtils.showAll)).ToArray();
            Rect rect = new Rect(90f, rowY, 300f, 80f);
            Text.Font = GameFont.Small;
            Text.Anchor = (TextAnchor)3;
            Rect rect2 = new Rect(24f, rowY + (rect.height - 42f) / 2f, 42f, 42f);
            GUI.color = faction.Color;
            GUI.DrawTexture(rect2, (Texture)faction.def.FactionIcon);
            GUI.color = Color.white;
            string label = faction.Name.CapitalizeFirst() + "\n" + faction.def.LabelCap + "\n" + ((faction.leader != null) ? (faction.LeaderTitle.CapitalizeFirst() + ": " + faction.leader.Name.ToStringFull) : "");
            Widgets.Label(rect, label);
            Rect rect3 = new Rect(0f, rowY, rect.xMax, 80f);
            if (Mouse.IsOver(rect3))
            {
                TipSignal tip = new TipSignal(() => faction.Name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + faction.def.LabelCap.Resolve() + "\n\n" + faction.def.Description, faction.loadID ^ 0x738AC053);
                TooltipHandler.TipRegion(rect3, tip);
                Widgets.DrawHighlight(rect3);
            }

            if (Widgets.ButtonInvisible(rect3, doMouseoverSound: false))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(faction));
            }

            Rect rect4 = new Rect(rect.xMax, rowY, 40f, 80f);
            Widgets.InfoCardButtonCentered(rect4, faction);
            Rect rect5 = new Rect(rect4.xMax, rowY, 60f, 80f);
            if (ModsConfig.IdeologyActive && !Find.IdeoManager.classicMode)
            {
                if (faction.ideos != null)
                {
                    float num2 = rect5.x;
                    float num3 = rect5.y;
                    if (faction.ideos.PrimaryIdeo != null)
                    {
                        if (num2 + 40f > rect5.xMax)
                        {
                            num2 = rect5.x;
                            num3 += 45f;
                        }

                        Rect rect6 = new Rect(num2, num3 + (rect5.height - 40f) / 2f, 40f, 40f);
                        IdeoUIUtility.DoIdeoIcon(rect6, faction.ideos.PrimaryIdeo, doTooltip: true, delegate
                        {
                            IdeoUIUtility.OpenIdeoInfo(faction.ideos.PrimaryIdeo);
                        });
                        num2 += rect6.width + 5f;
                        num2 = rect5.x;
                        num3 += 45f;
                    }

                    List<Ideo> minor = faction.ideos.IdeosMinorListForReading;
                    int i;
                    for (i = 0; i < minor.Count; i++)
                    {
                        if (num2 + 22f > rect5.xMax)
                        {
                            num2 = rect5.x;
                            num3 += 27f;
                        }

                        if (num3 + 22f > rect5.yMax)
                        {
                            break;
                        }

                        Rect rect7 = new Rect(num2, num3 + (rect5.height - 22f) / 2f, 22f, 22f);
                        IdeoUIUtility.DoIdeoIcon(rect7, minor[i], doTooltip: true, delegate
                        {
                            IdeoUIUtility.OpenIdeoInfo(minor[i]);
                        });
                        num2 += rect7.width + 5f;
                    }
                }
            }
            else
            {
                rect5.width = 0f;
            }

            Rect rect8 = new Rect(rect5.xMax, rowY, 70f, 80f);
            if (!faction.IsPlayer)
            {
                string text = faction.PlayerRelationKind.GetLabelCap();
                if (faction.defeated)
                {
                    text = text.Colorize(ColorLibrary.Grey);
                }

                GUI.color = faction.PlayerRelationKind.GetColor();
                Text.Anchor = (TextAnchor)4;
                if (faction.HasGoodwill && !faction.def.permanentEnemy)
                {
                    Widgets.Label(new Rect(rect8.x, rect8.y - 25f, rect8.width, rect8.height), text);
                    Text.Font = GameFont.Medium;

                    Widgets.Label(new Rect(rect8.x, rect8.y - 5f, rect8.width, rect8.height), VassalChecks.FactionVassalDatas[faction].Loyalty.ToString());
                    Text.Font = GameFont.Small;
                    
                    var taxation = "";

                    if (faction is CustomFaction customFaction)
                    {
                        var list = VassalChecks.FactionVassalDatas;

                        taxation = list == null ? "" : VassalChecks.Tick2Day(list[customFaction].TaxationCooldown).ToString("0.0");
                    }

                    Widgets.Label(new Rect(rect8.x, rect8.y + 22f, rect8.width, rect8.height), "TaxationCooldown".Translate(taxation).ToString().Colorize(GetColor()));
                }
                else
                {
                    Widgets.Label(rect8, text);
                }

                GenUI.ResetLabelAlign();
                GUI.color = Color.white;
                if (Mouse.IsOver(rect8))
                {
                    TaggedString taggedString = "";
                    if (faction.def.permanentEnemy)
                    {
                        taggedString = "CurrentGoodwillTip_PermanentEnemy".Translate();
                    }
                    else if (faction.HasGoodwill)
                    {
                        var loyalty = int.Parse(VassalChecks.FactionVassalDatas[faction].Loyalty.ToStringWithSign());

                        taggedString = "Loyalty".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + (loyalty + ", " + faction.PlayerRelationKind.GetLabel()).Colorize(faction.PlayerRelationKind.GetColor());

                        taggedString += " ";

                        if (loyalty < 0)
                            taggedString += "Disloyal".Translate().Colorize(Color.red);
                        else taggedString += "Loyal".Translate().Colorize(Color.green);


                        TaggedString ongoingEvents = FactionUIUtils.GetOngoingEvents(faction);
                        if (!ongoingEvents.NullOrEmpty())
                        {
                            taggedString += "\n\n" + "OngoingEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + ongoingEvents;
                        }

                        TaggedString recentEvents = FactionUIUtils.GetRecentEvents(faction);
                        if (!recentEvents.NullOrEmpty())
                        {
                            taggedString += "\n\n" + "RecentEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + recentEvents;
                        }

                        string s = Tip(faction);

                        taggedString += "\n\n" + s.Colorize(ColoredText.SubtleGrayColor);
                    }

                    if (taggedString != "")
                    {
                        TooltipHandler.TipRegion(rect8, taggedString);
                    }

                    Widgets.DrawHighlight(rect8);
                }
            }

            Rect rect9 = new Rect(rect8.xMax, rowY, 54f, 80f);
            if (!faction.IsPlayer && faction.HasGoodwill && !faction.def.permanentEnemy)
            {
                FactionRelationKind relationKindForGoodwill = FactionUIUtils.GetRelationKindForGoodwill(faction.NaturalGoodwill);
                GUI.color = relationKindForGoodwill.GetColor();
                Rect rect10 = rect9.ContractedBy(7f);
                rect10.y = rowY + 30f;
                rect10.height = 20f;
                Text.Anchor = (TextAnchor)1;
                Widgets.DrawRectFast(rect10, Color.black);
                Widgets.Label(rect10, faction.NaturalGoodwill.ToStringWithSign());
                GenUI.ResetLabelAlign();
                GUI.color = Color.white;
                if (Mouse.IsOver(rect9))
                {
                    TaggedString taggedString2 = "NaturalGoodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + faction.NaturalGoodwill.ToStringWithSign().Colorize(relationKindForGoodwill.GetColor());
                    int goodwill = Mathf.Clamp(faction.NaturalGoodwill - 50, -100, 100);
                    int goodwill2 = Mathf.Clamp(faction.NaturalGoodwill + 50, -100, 100);
                    taggedString2 += "\n" + "NaturalGoodwillRange".Translate().Colorize(ColoredText.TipSectionTitleColor)
                        + ": " + goodwill.ToString().Colorize(FactionUIUtils.GetRelationKindForGoodwill(goodwill).GetColor()) 
                        + " " + "RangeTo".Translate() + " "
                        + goodwill2.ToString().Colorize(FactionUIUtils.GetRelationKindForGoodwill(goodwill2).GetColor());
                    TaggedString naturalGoodwillExplanation = FactionUIUtils.GetNaturalGoodwillExplanation(faction);
                    if (!naturalGoodwillExplanation.NullOrEmpty())
                    {
                        taggedString2 += "\n\n" + "AffectedBy".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + naturalGoodwillExplanation;
                    }

                    taggedString2 += "\n\n" + "NaturalGoodwillDescription".Translate(1.25f.ToStringPercent()).Colorize(ColoredText.SubtleGrayColor);
                    TooltipHandler.TipRegion(rect9, taggedString2);
                    Widgets.DrawHighlight(rect9);
                }
            }

            float num4 = rect9.xMax + 17f;
            for (int j = 0; j < array.Length; j++)
            {
                if (num4 >= rect9.xMax + num)
                {
                    num4 = rect9.xMax;
                    rowY += 27f;
                }

                FactionUIUtils.DrawFactionIconWithTooltip(new Rect(num4, rowY + 29f, 22f, 22f), array[j]);
                num4 += 27f;
            }

            Text.Anchor = 0;

            __result = 80f;

            return false;
        }

        public override bool PreGetPriceTooltip(ref string __result, ref Tradeable __instance, TradeAction action)
        {
            var type = typeof(Tradeable);

            var faction = TradeSession.trader.Faction;

            var flag = BindingFlags.Instance | BindingFlags.NonPublic;

            if (!__instance.HasAnyThing)
            {
                __result = "";
            }

            type.GetMethod("InitPriceDataIfNeeded", flag).Invoke(__instance, []);

            var priceFactorBuy_TraderPriceType = (float)type.GetField("priceFactorBuy_TraderPriceType", flag).GetValue(__instance);

            var priceGain_PlayerNegotiator = (float)type.GetField("priceGain_PlayerNegotiator", flag).GetValue(__instance);

            var priceGain_Leader = (float)type.GetField("priceGain_Leader", flag).GetValue(__instance);

            var priceFactorBuy_JoinAs = (float)type.GetField("priceFactorBuy_JoinAs", flag).GetValue(__instance);

            var priceGain_Settlement = (float)type.GetField("priceGain_Settlement", flag).GetValue(__instance);

            var priceFactorSell_TraderPriceType = (float)type.GetField("priceFactorSell_TraderPriceType", flag).GetValue(__instance);

            var priceFactorSell_ItemSellPriceFactor = (float)type.GetField("priceFactorSell_ItemSellPriceFactor", flag).GetValue(__instance);

            var priceFactorSell_HumanPawn = (float)type.GetField("priceFactorSell_HumanPawn", flag).GetValue(__instance);

            var priceGain_DrugBonus = (float)type.GetField("priceGain_DrugBonus", flag).GetValue(__instance);

            var priceGain_AnimalProduceBonus = (float)type.GetField("priceGain_AnimalProduceBonus", flag).GetValue(__instance);

            string text = ((action == TradeAction.PlayerBuys) ? "BuyPriceDesc".Translate() : "SellPriceDesc".Translate());
            if (TradeSession.TradeCurrency != 0)
            {
                __result = text;
            }

            text += "\n\n";
            text += StatDefOf.MarketValue.LabelCap + ": " + __instance.BaseMarketValue.ToStringMoney();
            if (action == TradeAction.PlayerBuys)
            {
                text += "\n  x " + 1.4f.ToString("F2") + " (" + "Buying".Translate() + ")";
                if (priceFactorBuy_TraderPriceType != 1f)
                {
                    text += "\n  x " + priceFactorBuy_TraderPriceType.ToString("F2") + " (" + "TraderTypePrice".Translate() + ")";
                }

                if (Find.Storyteller.difficulty.tradePriceFactorLoss != 0f)
                {
                    text += "\n  x " + (1f + Find.Storyteller.difficulty.tradePriceFactorLoss).ToString("F2") + " (" + "DifficultyLevel".Translate() + ")";
                }

                text += "\n";
                text += "\n" + "YourNegotiatorBonus".Translate() + ": -" + (priceGain_PlayerNegotiator - priceGain_Leader).ToStringPercent();
                if (ModsConfig.IdeologyActive)
                {
                    if (priceGain_Leader != 0f)
                    {
                        text += "\n" + "YourLeaderTradeBonus".Translate() + ": -" + priceGain_Leader.ToStringPercent();
                    }

                    if (priceFactorBuy_JoinAs != 1f)
                    {
                        text += "\n" + "Slave".Translate().CapitalizeFirst() + ": x" + priceFactorBuy_JoinAs.ToStringPercent();
                    }
                }

                if (priceGain_Settlement != 0f)
                {
                    text += "\n" + "TradeWithFactionBaseBonus".Translate() + ": -" + priceGain_Settlement.ToStringPercent();
                }

                if (faction != null)
                    text += "\n" + "VassalTradeBuyMultiplier".Translate() + ": " + (GetPlayerBuyPriceMultiplier(faction) - 1).ToStringPercent();
            }
            else
            {
                text += "\n  x " + 0.6f.ToString("F2") + " (" + "Selling".Translate() + ")";
                if (priceFactorSell_TraderPriceType != 1f)
                {
                    text += "\n  x " + priceFactorSell_TraderPriceType.ToString("F2") + " (" + "TraderTypePrice".Translate() + ")";
                }

                if (priceFactorSell_ItemSellPriceFactor != 1f)
                {
                    text += "\n  x " + priceFactorSell_ItemSellPriceFactor.ToString("F2") + " (" + "ItemSellPriceFactor".Translate() + ")";
                }

                if (priceFactorSell_HumanPawn != 1f)
                {
                    text += "\n  x " + priceFactorSell_HumanPawn.ToString("F2") + " (" + "Slave".Translate() + ")";
                }

                if (Find.Storyteller.difficulty.tradePriceFactorLoss != 0f)
                {
                    text += "\n  x " + (1f - Find.Storyteller.difficulty.tradePriceFactorLoss).ToString("F2") + " (" + "DifficultyLevel".Translate() + ")";
                }

                text += "\n";
                text += "\n" + "YourNegotiatorBonus".Translate() + ": " + (priceGain_PlayerNegotiator - priceGain_Leader).ToStringPercent();
                if (ModsConfig.IdeologyActive && priceGain_Leader != 0f)
                {
                    text += "\n" + "YourLeaderTradeBonus".Translate() + ": " + priceGain_Leader.ToStringPercent();
                }

                if (priceGain_Settlement != 0f)
                {
                    text += "\n" + "TradeWithFactionBaseBonus".Translate() + ": " + priceGain_Settlement.ToStringPercent();
                }

                if (priceGain_DrugBonus != 0f)
                {
                    text += "\n" + "TradingDrugsBonus".Translate() + ": " + priceGain_DrugBonus.ToStringPercent();
                }

                if (priceGain_AnimalProduceBonus != 0f)
                {
                    text += "\n" + "TradingAnimalProduceBonus".Translate() + ": " + priceGain_AnimalProduceBonus.ToStringPercent();
                }
            }

            text += "\n\n";
            float priceFor = __instance.GetPriceFor(action);
            text += "FinalPrice".Translate() + ": " + priceFor.ToStringMoney();
            if ((action == TradeAction.PlayerBuys && priceFor <= 0.5f) || (action == TradeAction.PlayerBuys && priceFor <= 0.01f))
            {
                text += " (" + "minimum".Translate() + ")";
            }

            __result = text;

            return false;
        }
    }
}
