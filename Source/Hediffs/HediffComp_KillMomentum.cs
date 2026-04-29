using RimWorld;
using Verse;

namespace MassGrave
{
    public class HediffCompProperties_KillMomentum : HediffCompProperties
    {
        public int durationTicks = 15000; // 6時間

        public HediffCompProperties_KillMomentum()
        {
            this.compClass = typeof(HediffComp_KillMomentum);
        }
    }

    public class HediffComp_KillMomentum : HediffComp
    {
        public HediffCompProperties_KillMomentum Props
            => (HediffCompProperties_KillMomentum)this.props;

        private int endTick;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            endTick = Find.TickManager.TicksGame + Props.durationTicks;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Find.TickManager.TicksGame >= endTick)
            {
                parent.pawn.health.RemoveHediff(parent);
            }
        }

        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            Pawn pawn = parent.pawn;

            // 祝福が無いなら発動しない
            if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("MemorialBlessing")))
                return;

            // スタック不可 → 上書き
            endTick = Find.TickManager.TicksGame + Props.durationTicks;
        }

    }
}
