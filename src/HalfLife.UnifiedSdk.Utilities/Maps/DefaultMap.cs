using HalfLife.UnifiedSdk.Utilities.Entities;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal sealed class DefaultMap : Map
    {
        public override EntityList Entities { get; }

        internal DefaultMap(MapData mapData)
            : base(mapData)
        {
            Entities = new(this, mapData.GetEntities());
        }
    }
}
