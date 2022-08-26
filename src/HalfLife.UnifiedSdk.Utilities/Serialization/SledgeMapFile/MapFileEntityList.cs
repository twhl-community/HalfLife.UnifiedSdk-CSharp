using HalfLife.UnifiedSdk.Utilities.Entities;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileEntityList : EntityList
    {
        private readonly MapFileMapData _mapData;

        public MapFileEntityList(MapFileMapData mapData)
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
