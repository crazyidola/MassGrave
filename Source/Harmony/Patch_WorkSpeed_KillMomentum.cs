using HarmonyLib;
using RimWorld;
using Verse;

namespace MassGrave
{
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class Patch_WorkSpeed_KillMomentum
    {
        static void Postfix(StatRequest req, StatDef ___stat, ref float __result)
        {
            // WorkSpeedGlobal 以外は無視
            if (___stat != StatDefOf.WorkSpeedGlobal)
                return;

            Pawn pawn = req.Thing as Pawn;
            if (pawn == null) return;

            // KillMomentum が付いていないなら無視
            if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("KillMomentum")))
                return;

            // +30%
            __result *= 1.30f;
        }
    }
}
