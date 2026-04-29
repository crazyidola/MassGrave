using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;   // ← これ必須！

namespace MassGrave
{
    public class ITab_MassGrave : ITab
    {
        private Vector2 scrollPos;

        public ITab_MassGrave()
        {
            this.size = new Vector2(420f, 500f);
            this.labelKey = "Mass Grave";
        }

        private Building_MassGrave SelGrave => (Building_MassGrave)SelThing;

        protected override void FillTab()
        {
            var rect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, 300, 30), "巨大墓の中身");

            Text.Font = GameFont.Small;

            var listRect = new Rect(0, 40, rect.width - 20, rect.height - 50);

            // ★ viewRect の高さも Count() に変更
            var viewRect = new Rect(0, 0, listRect.width - 20, SelGrave.StoredCount * 40);

            Widgets.BeginScrollView(listRect, ref scrollPos, viewRect);

            float y = 0;

            // ★ IEnumerable → List に変換
            var corpsesList = SelGrave.StoredCorpses.ToList();

            // ★ 逆順ループ
            for (int i = corpsesList.Count - 1; i >= 0; i--)
            {
                var corpse = corpsesList[i];

                var row = new Rect(0, y, viewRect.width, 35);

                Widgets.Label(new Rect(0, y, 200, 30),
                    corpse.InnerPawn?.Name?.ToStringShort ?? "Unknown");

                if (Widgets.ButtonText(new Rect(220, y, 80, 30), "取り出す"))
                {
                    SelGrave.RemoveCorpse(corpse);
                    GenPlace.TryPlaceThing(corpse, SelGrave.Position, SelGrave.Map, ThingPlaceMode.Near);
                }

                y += 40;
            }

            Widgets.EndScrollView();
        }
    }
}
