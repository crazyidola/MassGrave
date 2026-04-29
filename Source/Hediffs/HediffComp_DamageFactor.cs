using RimWorld;
using UnityEngine;
using Verse;

namespace MassGrave
{
    public class HediffCompProperties_DamageReduction : HediffCompProperties
    {
        public HediffCompProperties_DamageReduction()
        {
            compClass = typeof(HediffComp_DamageReduction);
        }
    }

    public class HediffComp_DamageReduction : HediffComp
    {
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn pawn = Pawn;
            if (pawn == null || pawn.Dead) return;

            int deaths = Find.StoryWatcher.statsRecord.colonistsKilled;

            // B10 = 最大10%軽減
            float reduction = Mathf.Clamp(deaths * 0.001f, 0f, 0.10f);

            // 精神感応倍率（0～1.5）
            float psy = pawn.GetStatValue(StatDefOf.PsychicSensitivity, true);
            float psyFactor = Mathf.Clamp(psy, 0f, 1.5f);

            float finalReduction = reduction * psyFactor;

            // ダメージを軽減（最終ダメージに適用）
            float newDamage = totalDamageDealt * (1f - finalReduction);

            // ダメージを反映
            pawn.TakeDamage(new DamageInfo(
                dinfo.Def,
                newDamage,
                dinfo.ArmorPenetrationInt,
                dinfo.Angle,
                dinfo.Instigator,
                dinfo.HitPart,
                dinfo.Weapon,
                dinfo.Category
            ));
        }
    }
}
