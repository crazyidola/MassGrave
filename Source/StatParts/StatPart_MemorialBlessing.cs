using RimWorld;
using Verse;
using UnityEngine;

namespace MassGrave
{
    public class StatPart_MemorialBlessing : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null || pawn.Dead) return;

            if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("MemorialBlessing")))
                return;

            int deaths = Find.StoryWatcher.statsRecord.colonistsKilled;

            // Aカテゴリ（免疫・学習）1% × 死者数（上限100%）
            float A = Mathf.Clamp(deaths * 0.01f, 0f, 1.0f);

            // Bカテゴリ（照準時間）1% × 死者数（上限20%）
            float B20 = Mathf.Clamp(deaths * 0.01f, 0f, 0.20f);

            // Cカテゴリ（作業速度・治癒）1% × 死者数
            float C30 = Mathf.Clamp(deaths * 0.01f, 0f, 0.30f);   // 作業速度
            float C50 = Mathf.Clamp(deaths * 0.01f, 0f, 0.50f);   // 傷の治りやすさ

            // Dカテゴリ（温度）
            float heat = Mathf.Clamp(deaths * 1f, 0f, 10f);   // +10
            float cold = Mathf.Clamp(deaths * 1f, 0f, 10f);   // -10

            // Eカテゴリ（耐性）
            float toxic = Mathf.Clamp((deaths - 20) * 0.01f, 0f, 0.10f);
            float toxicEnv = Mathf.Clamp((deaths - 30) * 0.01f, 0f, 0.10f);
            float vacuum = Mathf.Clamp((deaths - 40) * 0.01f, 0f, 0.10f);

            // 精神感応性スケーリング
            float psySens = pawn.GetStatValue(StatDefOf.PsychicSensitivity, true);
            float psyFactor = Mathf.Clamp(psySens, 0f, 1.5f);

            float baseVal = val;   // 元の値を保存


            // Aカテゴリ（加算系）
            if (parentStat == StatDefOf.ImmunityGainSpeed)
            {
                float bonus = baseVal * A;
                val = baseVal + bonus * psyFactor;
            }
            else if (parentStat == StatDefOf.GlobalLearningFactor)
            {
                float bonus = baseVal * A;
                val = baseVal + bonus * psyFactor;
            }

            // Bカテゴリ（照準時間：減算系）
            else if (parentStat == DefDatabase<StatDef>.GetNamed("AimingDelayFactor"))
            {
                float reduction = baseVal * B20;
                val = baseVal - reduction * psyFactor;
            }

            // Cカテゴリ（作業速度：加算系）
            else if (parentStat == DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"))
            {
                float bonus = baseVal * C30;
                val = baseVal + bonus * psyFactor;
            }

            // Cカテゴリ（治療速度：加算系）
            else if (parentStat == DefDatabase<StatDef>.GetNamed("MedicalTendSpeed"))
            {
                float bonus = baseVal * C50;
                val = baseVal + bonus * psyFactor;
            }

            // Cカテゴリ（治療品質：加算系、絶対値）
            else if (parentStat == StatDefOf.MedicalTendQuality)
            {
                float bonus = C50;  // 絶対値加算
                val = baseVal + bonus * psyFactor;
            }

            // Dカテゴリ（温度：絶対値加算）
            else if (parentStat == StatDefOf.ComfyTemperatureMax)
            {
                val = baseVal + heat * psyFactor;
            }
            else if (parentStat == StatDefOf.ComfyTemperatureMin)
            {
                val = baseVal - cold * psyFactor;
            }

            // Eカテゴリ（耐性：乗算系）
            else if (parentStat == DefDatabase<StatDef>.GetNamed("ToxicResistance"))
            {
                val = baseVal * (1f + toxic * psyFactor);
            }
            else if (parentStat == DefDatabase<StatDef>.GetNamed("ToxicEnvironmentResistance"))
            {
                val = baseVal * (1f + toxicEnv * psyFactor);
            }
            else if (parentStat == DefDatabase<StatDef>.GetNamed("VacuumResistance"))
            {
                val = baseVal * (1f + vacuum * psyFactor);
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null) return null;

            if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("MemorialBlessing")))
                return null;

            int deaths = Find.StoryWatcher.statsRecord.colonistsKilled;
            return $"慰霊碑の祝福（死亡者 {deaths} 名）";
        }
    }
}
