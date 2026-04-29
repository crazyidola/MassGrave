using HarmonyLib;
using RimWorld;
using Verse;

namespace MassGrave
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("Kill")]
    public static class Patch_Pawn_Kill
    {
        static void Prefix(Pawn __instance, DamageInfo? dinfo)
        {
            // 加害者が存在しない場合は無視
            if (dinfo == null)
                return;

            var instigatorPawn = dinfo.Value.Instigator as Pawn;
            if (instigatorPawn == null)
                return;

            var killer = instigatorPawn;

            // 自殺は除外
            if (killer == __instance)
                return;

            // ★ KillMomentum を付与 ★
            var def = HediffDef.Named("KillMomentum");
            killer.health.AddHediff(def);

            Log.Message($"KillMomentum added to {killer.Name} for killing {__instance.Name}");
        }
    }
}
