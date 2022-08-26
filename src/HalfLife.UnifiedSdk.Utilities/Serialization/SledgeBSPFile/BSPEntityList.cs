using HalfLife.UnifiedSdk.Utilities.Entities;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPEntityList : EntityList
    {
        private readonly BSPMapBase _map;

        public BSPEntityList(BSPMapBase map)
            : base(map.GetEntities)
        {
            _map = map;
        }

        protected override Entity CreateNewEntityCore(string className)
        {
            return _map.CreateNewEntity(className);
        }

        protected override void RemoveAtCore(Entity entity, int index)
        {
            _map.Remove(entity);
        }
    }
}
