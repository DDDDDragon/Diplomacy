using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diplomacy.Utils
{
    public class ThingUtils
    {
        public static Faction GetThingFaction(Thing thing)
        {
            // 1. 直接获取Thing的派系
            if (thing.Faction != null)
                return thing.Faction;

            // 2. 通过持有者获取
            if (thing.ParentHolder is Pawn holder)
                return holder.Faction;

            // 3. 通过地图位置获取
            if (thing.Map != null)
            {
                // 检查是否在派系基地中
                if (thing.Map.Parent is Settlement settlement1)
                    return settlement1.Faction;

                // 检查领地所属
                Faction territoryFaction = thing.Map.ParentFaction;
                if (territoryFaction != null)
                    return territoryFaction;
            }

            // 4. 如果是囚犯的物品
            if (thing.ParentHolder is Pawn prisoner && prisoner.IsPrisoner)
                return prisoner.HostFaction;

            // 5. 如果是商队物品
            if (thing.ParentHolder is Pawn_CarryTracker carryTracker)
                return carryTracker.pawn.Faction;

            if (TradeSession.trader is Settlement settlement2)
                return settlement2.Faction;

            return null; // 无法确定派系
        }
    }
}
