namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>
    /// Event arguments type for entity keyvalue changed event.
    /// </summary>
    public class EntityKeyValueChangedEventArgs : EntityEventArgs
    {
        /// <summary>Key of the keyvalue that was changed.</summary>
        public string Key { get; }

        /// <summary>Previous value.</summary>
        public string? PreviousValue { get; }

        /// <summary>Current value.</summary>
        public string CurrentValue { get; }

        /// <summary>Creates a new instance of this event object.</summary>
        public EntityKeyValueChangedEventArgs(Entity entity, string key, string? previousValue, string currentValue)
            : base(entity)
        {
            Key = key;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }
}
