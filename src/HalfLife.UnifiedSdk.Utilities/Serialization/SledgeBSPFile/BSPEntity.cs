using HalfLife.UnifiedSdk.Utilities.Entities;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPEntity : Entity
    {
        public Sledge.Formats.Bsp.Objects.Entity Entity { get; }

        public override bool IsWorldspawn { get; }

        public BSPEntity(EntityList entityList, Sledge.Formats.Bsp.Objects.Entity entity, bool isWorldspawn)
            : base(entityList, entity.SortedKeyValues.ToImmutableList())
        {
            Entity = entity;
            IsWorldspawn = isWorldspawn;
        }

        protected override void SetKeyValue(int index, string key, string value, bool overwrite)
        {
            if (overwrite)
            {
                Entity.SortedKeyValues[index] = new(key, value);
            }
            else
            {
                Entity.SortedKeyValues.Insert(index, new(key, value));
            }
        }

        protected override void RemoveKeyValue(int index, string key)
        {
            Entity.SortedKeyValues.RemoveAt(index);
        }

        protected override void RemoveAllKeyValues()
        {
            //Never remove the classname.
            var className = Entity.ClassName;
            Entity.SortedKeyValues.Clear();
            Entity.ClassName = className;
        }
    }
}
