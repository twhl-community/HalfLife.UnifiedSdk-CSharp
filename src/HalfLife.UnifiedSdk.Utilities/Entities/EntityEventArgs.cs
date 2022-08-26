using System;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>Event arguments type for anything involving entities.</summary>
    public class EntityEventArgs : EventArgs
    {
        /// <summary>The entity involved in this event.</summary>
        public Entity Entity { get; }

        /// <summary>Creates a new instance of this event object.</summary>
        public EntityEventArgs(Entity entity)
        {
            Entity = entity;
        }
    }
}
