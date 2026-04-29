using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace MassGrave
{
    public class JoyGiver_VisitMassGrave : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            // 1. 大規模墓を探す
            var grave = FindClosestMassGrave(pawn);
            if (grave == null)
                return null;

            // 2. 空いているアクセス点を探す
            if (!grave.TryGetFreeAccessSpot(pawn, out IntVec3 spot))
                return null;

            // 3. Job を作成
            Job job = JobMaker.MakeJob(this.def.jobDef, grave, spot);
            job.ignoreJoyTimeAssignment = false;
            return job;
        }

        private Building_MassGrave FindClosestMassGrave(Pawn pawn)
        {
            return pawn.Map.listerBuildings
                .AllBuildingsColonistOfClass<Building_MassGrave>()
                .Where(b => b.CanPawnVisit(pawn))
                .OrderBy(b => pawn.Position.DistanceTo(b.Position))
                .FirstOrDefault();
        }
    }
}
