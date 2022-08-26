using HalfLife.UnifiedSdk.Utilities.Entities;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPEntityList : EntityList
    {
        private readonly BSPMapDataBase _mapData;

        public BSPEntityList(BSPMapDataBase mapData)
            : base(mapData.GetEntities())
        {
            _mapData = mapData;
        }

        protected override Entity CreateNewEntityCore(string className)
        {
            return _mapData.CreateNewEntity(className);
        }

        protected override void RemoveAtCore(Entity entity, int index)
        {
            _mapData.Remove(entity);
        }
    }
}
