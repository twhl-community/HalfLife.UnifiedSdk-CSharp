using HalfLife.UnifiedSdk.Utilities.Entities;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileEntityList : EntityList
    {
        private readonly MapFileMap _map;

        public MapFileEntityList(MapFileMap map)
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
