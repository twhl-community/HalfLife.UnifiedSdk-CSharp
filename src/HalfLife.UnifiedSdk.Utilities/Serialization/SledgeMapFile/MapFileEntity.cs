using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileEntity : Entity
    {
        public Sledge.Formats.Map.Objects.Entity Entity { get; }

        public override bool IsWorldspawn { get; }

        public MapFileEntity(EntityList entityList, Sledge.Formats.Map.Objects.Entity entity, bool isWorldspawn)
            : base(entityList, entity.SortedProperties
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.ClassName, entity.ClassName))
                .Append(new KeyValuePair<string, string>(KeyValueUtilities.SpawnFlags, entity.SpawnFlags.ToString()))
                .ToImmutableList())
        {
            Entity = entity;
            IsWorldspawn = isWorldspawn;
        }

        protected override void SetKeyValue(int index, string key, string value, bool overwrite)
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
                    {
                        index = GetAdjustedIndex(index);

                        if (overwrite)
                        {
                            Entity.SortedProperties[index] = new(key, value);
                        }
                        else
                        {
                            Entity.SortedProperties.Insert(index, new(key, value));
                        }
                        break;
                    }
            }
        }

        protected override void RemoveKeyValue(int index, string key)
        {
            switch (key)
            {
                case KeyValueUtilities.ClassName:
                    throw new ArgumentException("Cannot remove the classname keyvalue");

                case KeyValueUtilities.SpawnFlags:
                    Entity.SpawnFlags = 0;
                    break;

                default:
                    index = GetAdjustedIndex(index);
                    Entity.SortedProperties.RemoveAt(index);
                    break;
            }
        }

        protected override void RemoveAllKeyValues()
        {
            Entity.SortedProperties.Clear();
        }

        private int GetAdjustedIndex(int index)
        {
            var classnameIndex = IndexOf(KeyValueUtilities.ClassName);
            var spawnflagsIndex = IndexOf(KeyValueUtilities.SpawnFlags);

            Debug.Assert(index != classnameIndex);
            Debug.Assert(index != spawnflagsIndex);

            if (classnameIndex < index)
            {
                --index;
            }

            if (spawnflagsIndex != -1 && spawnflagsIndex <= index)
            {
                --index;
            }

            return index;
        }
    }
}
