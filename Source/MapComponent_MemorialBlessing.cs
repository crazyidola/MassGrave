using RimWorld;
using Verse;
using System.Linq;

namespace MassGrave
{
    public class MapComponent_MemorialBlessing : MapComponent
    {
        private int nextCheckTick = 0;

        public MapComponent_MemorialBlessing(Map map) : base(map)
        {
            Log.Message("MapComponent_MemorialBlessing: constructor called");
            nextCheckTick = Find.TickManager.TicksGame + 60;   // ★ 1秒後に最初のチェック
        }

        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 1000 == 0)
                Log.Message("MapComponentTick running: " + Find.TickManager.TicksGame);

            if (Find.TickManager.TicksGame >= nextCheckTick)
            {
                nextCheckTick = Find.TickManager.TicksGame + 60;   // ★ 以降も1秒ごと
                UpdateBlessing();
            }
        }

        private void UpdateBlessing()
        {
            // ★★★ デバッグログ：慰霊碑と入植者数を確認 ★★★
            var graves = map.listerBuildings
                .AllBuildingsColonistOfClass<MassGrave.Building_MassGrave>()
                .ToList();

            Log.Message($"UpdateBlessing called. hasMemorial={graves.Any()}, graveCount={graves.Count}, colonists={map.mapPawns.FreeColonistsCount}");

            bool hasMemorial = graves.Any();

            foreach (Pawn pawn in map.mapPawns.FreeColonists)
            {
                var def = HediffDef.Named("MemorialBlessing");
                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(def);

                if (hasMemorial)
                {
                    if (hediff == null)
                    {
                        // ★★★ デバッグログ：付与前後の状態を確認 ★★★
                        Log.Message($"Trying to add hediff to {pawn.Name}");
                        Log.Message($"HediffDef.Named result = {def}");

                        pawn.health.AddHediff(def);

                        Log.Message($"After AddHediff: now has = {pawn.health.hediffSet.HasHediff(def)}");
                    }
                }
                else
                {
                    if (hediff != null)
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}
