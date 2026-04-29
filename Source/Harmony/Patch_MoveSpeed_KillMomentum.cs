using HarmonyLib;
using RimWorld;
using Verse;

namespace MassGrave
{
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class Patch_MoveSpeed_KillMomentum
    {
        static void Postfix(StatRequest req, StatDef ___stat, ref float __result)
        {
            if (___stat != StatDefOf.MoveSpeed)
                return;

            Pawn pawn = req.Thing as Pawn;
            if (pawn == null) return;

            if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("KillMomentum")))
                return;

            __result *= 1.30f;
        }
    }
}
