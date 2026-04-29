using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace MassGrave
{
    public class WorkGiver_BuryMassGrave : WorkGiver
    {
        public override Job NonScanJob(Pawn pawn)
        {
            var grave = pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("MassGrave_Large"))
                .OfType<Building_MassGrave>()
                .FirstOrDefault(g => pawn.CanReserve(g) && g.StoredCount < Building_MassGrave.MaxBodies);

            if (grave == null) return null;

            var corpse = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse)
     .OfType<Corpse>()
     .FirstOrDefault(c =>
         pawn.CanReserve(c)
         && c.InnerPawn != null
         && c.InnerPawn.Faction == Faction.OfPlayer   // ★ 入植者のみ
     );


            if (corpse == null) return null;

            var job = JobMaker.MakeJob(
                DefDatabase<JobDef>.GetNamed("BuryMassGrave"),
                grave,
                corpse
            );

            job.count = 1; // ★ Invalid count: -1 を消す

            return job;
        }
    }
}
