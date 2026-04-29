using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MassGrave
{
    public class JobDriver_BuryMassGrave : JobDriver
    {
        private Building_MassGrave Grave => (Building_MassGrave)job.targetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed)
                && pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // 死体のところへ
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);

            // 死体を拾う
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);

            // 墓へ
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            // 墓に収容
            yield return new Toil
            {
                initAction = () =>
                {
                    var carried = pawn.carryTracker.CarriedThing as Corpse;
                    if (carried != null)
                    {
                        Grave.AddCorpse(carried);

                        // ★ 手から死体を消す
                        pawn.carryTracker.innerContainer.Remove(carried);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
