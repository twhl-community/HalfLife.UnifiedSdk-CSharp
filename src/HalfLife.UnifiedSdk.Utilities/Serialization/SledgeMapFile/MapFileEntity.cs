using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileEntity : Entity
    {
        public Sledge.Formats.Map.Objects.Entity Entity { get; }

        public override bool IsWorldspawn { get; }

        public MapFileEntity(EntityList entityList, Sledge.Formats.Map.Objects.Entity entity, bool isWorldspawn)
            : base(entityList, entity.Properties
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.ClassName, entity.ClassName))
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.SpawnFlags, entity.SpawnFlags.ToString()))
                .ToImmutableDictionary(kv => kv.Key, kv => kv.Value))
        {
            Entity = entity;
            IsWorldspawn = isWorldspawn;
        }

        protected override void SetKeyValue(string key, string value)
        {
            switch (key)
            {
                case KeyValueUtilities.ClassName:
                    Entity.ClassName = value;
                    break;

                case KeyValueUtilities.SpawnFlags:
                    Entity.SpawnFlags = int.Parse(value);
                    break;

                default:
                    Entity.Properties[key] = value;
                    break;
            }
        }

        protected override void RemoveKeyValue(string key)
        {
            switch (key)
            {
                case KeyValueUtilities.ClassName:
                    throw new ArgumentException("Cannot remove the classname keyvalue");

                case KeyValueUtilities.SpawnFlags:
                    Entity.SpawnFlags = 0;
                    break;

                default:
                    Entity.Properties.Remove(key);
                    break;
            }
        }

        protected override void RemoveAllKeyValues()
        {
            Entity.Properties.Clear();
        }
    }
}
