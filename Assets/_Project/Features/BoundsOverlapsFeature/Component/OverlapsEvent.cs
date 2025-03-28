using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct OverlapsEvent : IEcsComponent
    {
        public entlong largestEntity;
        public entlong smallestEntity;
        public entlong GetOther(entlong entity)
        {
            if (entity.GetIDUnchecked() == largestEntity.GetIDUnchecked())
            {
                return smallestEntity;
            }
            return largestEntity;
        }
    }
}