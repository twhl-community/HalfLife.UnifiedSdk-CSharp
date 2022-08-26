using HalfLife.UnifiedSdk.Utilities.Entities;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPEntity : Entity
    {
        public Sledge.Formats.Bsp.Objects.Entity Entity { get; }

        public override bool IsWorldspawn { get; }

        public BSPEntity(Sledge.Formats.Bsp.Objects.Entity entity, bool isWorldspawn)
            : base(entity.KeyValues.ToImmutableDictionary(kv => kv.Key, kv => kv.Value))
        {
            Entity = entity;
            IsWorldspawn = isWorldspawn;
        }

        protected override void SetKeyValue(string key, string value)
        {
            Entity.KeyValues[key] = value;
        }

        protected override void RemoveKeyValue(string key)
        {
            Entity.KeyValues.Remove(key);
        }

        protected override void RemoveAllKeyValues()
        {
            //Never remove the classname.
            var className = Entity.ClassName;
            Entity.KeyValues.Clear();
            Entity.ClassName = className;
        }
    }
}
