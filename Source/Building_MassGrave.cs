using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MassGrave
{
    public class Building_MassGrave : Building, IThingHolder
    {
        public const int MaxBodies = 30;

        private ThingOwner<Thing> innerContainer;
        private List<IntVec3> accessSpots;

        public Building_MassGrave()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public IEnumerable<Corpse> StoredCorpses =>
            innerContainer.OfType<Corpse>();

        public int StoredCount =>
            innerContainer.OfType<Corpse>().Count();

        public void AddCorpse(Corpse corpse)
        {
            if (corpse == null) return;

            if (corpse.holdingOwner != null)
            {
                corpse.holdingOwner.TryTransferToContainer(corpse, innerContainer);
            }
            else
            {
                innerContainer.TryAdd(corpse);
            }
        }

        public void RemoveCorpse(Corpse corpse)
        {
            innerContainer.Remove(corpse);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }

        // ★ override ではなく “実装”
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, innerContainer);
        }

        // ★ override ではなく “実装”
        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (innerContainer != null)
            {
                innerContainer.TryDropAll(this.Position, this.Map, ThingPlaceMode.Near);
            }

            base.Destroy(mode);
        }

        public override string GetInspectString()
        {
            return $"遺体数: {StoredCount}/{MaxBodies}";
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            yield return new Command_Action
            {
                defaultLabel = $"遺体数: {StoredCount}/{MaxBodies}",
                action = () => { }
            };
        }

        public bool CanPawnVisit(Pawn pawn)
        {
            return true;
        }

        public bool TryGetFreeAccessSpot(Pawn pawn, out IntVec3 spot)
        {
            // 基本形（北向き時）
            var baseSpots = new List<IntVec3>()
    {
        new IntVec3(-1, 0, -1),
        new IntVec3(0, 0, -1),
        new IntVec3(1, 0, -1)
    };

            // ★ 毎回、建物の向きに合わせて回転させる
            accessSpots = new List<IntVec3>();
            foreach (var s in baseSpots)
            {
                accessSpots.Add(this.Position + s.RotatedBy(this.Rotation));
            }

            foreach (var s in accessSpots)
            {
                LocalTargetInfo cellTarget = new LocalTargetInfo(s);

                if (pawn.Map.reservationManager.CanReserve(pawn, cellTarget) &&
                    pawn.CanReserveAndReach(s, PathEndMode.OnCell, Danger.None))
                {
                    spot = s;
                    return true;
                }
            }

            spot = IntVec3.Invalid;
            return false;
        }
    }
}
