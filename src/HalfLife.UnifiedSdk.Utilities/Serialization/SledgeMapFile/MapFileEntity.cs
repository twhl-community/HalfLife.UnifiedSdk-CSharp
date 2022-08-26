using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using Sledge.Formats.Map.Objects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileEntity : IMapEntity
    {
        public Entity Entity { get; }

        public ImmutableDictionary<string, string> KeyValues => Entity.Properties
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.ClassName, Entity.ClassName))
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.SpawnFlags, Entity.SpawnFlags.ToString()))
                .ToImmutableDictionary(kv => kv.Key, kv => kv.Value);

        public bool IsWorldspawn { get; }

        public MapFileEntity(Entity entity, bool isWorldspawn)
        {
            Entity = entity;
            IsWorldspawn = isWorldspawn;
        }

        public void SetKeyValue(string key, string value)
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

        public void RemoveKeyValue(string key)
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

        public void RemoveAllKeyValues()
        {
            Entity.Properties.Clear();
        }
    }
}
