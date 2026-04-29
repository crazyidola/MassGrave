using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MassGrave
{
    public class JobDriver_VisitMassGrave : JobDriver
    {
        private const int PrayDuration = 4000; // 約66秒

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // 建物は予約しない、スポットだけ予約
            return pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);

            // 1. アクセスポットへ移動
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            // 2. 黙祷（継続 Toil）
            var pray = new Toil();
            pray.defaultCompleteMode = ToilCompleteMode.Delay;
            pray.defaultDuration = PrayDuration;

            pray.handlingFacing = true;

            pray.initAction = () =>
            {
                // 建物の向きに応じて「前方向」を決める
                IntVec3 forward = IntVec3.North.RotatedBy(TargetA.Thing.Rotation);

                // Pawn の位置 + forward を向く
                pawn.rotationTracker.FaceCell(pawn.Position + forward);
            };

            pray.tickAction = () =>
            {
                pawn.needs.joy.GainJoy(0.00035f, JoyKindDefOf.Social);
            };

            // ★ 黙祷が終わった瞬間に Thought を付与する
            pray.AddFinishAction(() =>
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(
                    DefDatabase<ThoughtDef>.GetNamed("RememberedTheDeceased_MassGrave")
                );
            });

            yield return pray;
        }
    }
}
