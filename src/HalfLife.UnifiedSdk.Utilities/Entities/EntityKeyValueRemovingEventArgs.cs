namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>
    /// Event arguments type for entity keyvalue removed event.
    /// </summary>
    public class EntityKeyValueRemovingEventArgs : EntityEventArgs
    {
        /// <summary>Key of the keyvalue that was removed.</summary>
        public string Key { get; }

        /// <summary>Creates a new instance of this event object.</summary>
        public EntityKeyValueRemovingEventArgs(Entity entity, string key)
            : base(entity)
        {
            Key = key;
        }
    }
}
