using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>Extensions for <see cref="Entity"/>.</summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Gets <paramref name="key"/> as an <see cref="int"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static int GetInteger(this Entity entity, string key, int defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value && int.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets <paramref name="key"/> as a <see cref="double"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static double GetDouble(this Entity entity, string key, double defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value && ParsingUtilities.TryParseDouble(value, out var parsedValue))
            {
                return parsedValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets <paramref name="key"/> as a <see cref="bool"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static bool GetBool(this Entity entity, string key, bool defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value && bool.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets <paramref name="key"/> as a <see cref="Vector2"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static Vector2 GetVector2(this Entity entity, string key, Vector2 defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value)
            {
                return ParsingUtilities.ParseVector2(value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets <paramref name="key"/> as a <see cref="Vector3"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static Vector3 GetVector3(this Entity entity, string key, Vector3 defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value)
            {
                return ParsingUtilities.ParseVector3(value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets <paramref name="key"/> as a <see cref="Vector4"/> if it exists and if it could be parsed, <paramref name="defaultValue"/> otherwise.
        /// </summary>
        public static Vector4 GetVector4(this Entity entity, string key, Vector4 defaultValue = default)
        {
            if (entity.GetStringOrNull(key) is string value)
            {
                return ParsingUtilities.ParseVector4(value);
            }

            return defaultValue;
        }

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetInteger(this Entity entity, string key, int value) => entity.SetString(key, value.ToString());

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetDouble(this Entity entity, string key, double value) => entity.SetString(key, ParsingUtilities.DoubleToString(value));

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetBool(this Entity entity, string key, int value) => entity.SetString(key, value.ToString());

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetVector2(this Entity entity, string key, Vector2 value) => entity.SetString(key, ParsingUtilities.Vector2ToString(value));

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetVector3(this Entity entity, string key, Vector3 value) => entity.SetString(key, ParsingUtilities.Vector3ToString(value));

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/></summary>
        public static void SetVector4(this Entity entity, string key, Vector4 value) => entity.SetString(key, ParsingUtilities.Vector4ToString(value));

        /// <summary>Gets the <c>spawnflags</c> value.</summary>
        public static int GetSpawnFlags(this Entity entity) => entity.GetInteger(KeyValueUtilities.SpawnFlags);

        /// <summary>Gets the <c>model</c> value.</summary>
        public static string GetModel(this Entity entity) => entity.GetString(KeyValueUtilities.Model);

        /// <summary>Gets the <c>targetname</c> value.</summary>
        public static string GetTargetName(this Entity entity) => entity.GetString(KeyValueUtilities.TargetName);

        /// <summary>Gets the <c>target</c> value.</summary>
        public static string GetTarget(this Entity entity) => entity.GetString(KeyValueUtilities.Target);

        /// <summary>Gets the <c>globalname</c> value.</summary>
        public static string GetGlobalName(this Entity entity) => entity.GetString(KeyValueUtilities.GlobalName);

        /// <summary>Gets the <c>delay</c> value.</summary>
        public static double GetDelay(this Entity entity) => entity.GetDouble(KeyValueUtilities.Delay);

        /// <summary>Gets the <c>origin</c> value.</summary>
        public static Vector3 GetOrigin(this Entity entity) => entity.GetVector3(KeyValueUtilities.Origin);

        /// <summary>Gets the <c>angles</c> value.</summary>
        public static Vector3 GetAngles(this Entity entity) => entity.GetVector3(KeyValueUtilities.Angles);

        /// <summary>Gets the <c>rendermode</c> value.</summary>
        public static RenderMode GetRenderMode(this Entity entity) => (RenderMode)entity.GetInteger(KeyValueUtilities.RenderMode);

        /// <summary>Gets the <c>renderamt</c> value.</summary>
        public static double GetRenderAmount(this Entity entity) => entity.GetDouble(KeyValueUtilities.RenderAmount);

        /// <summary>Gets the <c>rendercolor</c> value.</summary>
        public static Vector3 GetRenderColor(this Entity entity) => entity.GetVector3(KeyValueUtilities.RenderColor);

        /// <summary>Sets the <c>spawnflags</c> value.</summary>
        public static void SetSpawnFlags(this Entity entity, int spawnFlags) => entity.SetInteger(KeyValueUtilities.SpawnFlags, spawnFlags);

        /// <summary>Sets the <c>model</c> value.</summary>
        public static void SetModel(this Entity entity, string model) => entity.SetString(KeyValueUtilities.Model, model);

        /// <summary>Sets the <c>targetname</c> value.</summary>
        public static void SetTargetName(this Entity entity, string targetName) => entity.SetString(KeyValueUtilities.TargetName, targetName);

        /// <summary>Sets the <c>target</c> value.</summary>
        public static void SetTarget(this Entity entity, string target) => entity.SetString(KeyValueUtilities.Target, target);

        /// <summary>Sets the <c>globalname</c> value.</summary>
        public static void SetGlobalName(this Entity entity, string globalName) => entity.SetString(KeyValueUtilities.GlobalName, globalName);

        /// <summary>Sets the <c>delay</c> value.</summary>
        public static void SetDelay(this Entity entity, double delay) => entity.SetDouble(KeyValueUtilities.Delay, delay);

        /// <summary>Sets the <c>origin</c> value.</summary>
        public static void SetOrigin(this Entity entity, Vector3 value) => entity.SetVector3(KeyValueUtilities.Origin, value);

        /// <summary>Sets the <c>angles</c> value.</summary>
        public static void SetAngles(this Entity entity, Vector3 value) => entity.SetVector3(KeyValueUtilities.Angles, value);

        /// <summary>Sets the <c>rendermode</c> value.</summary>
        public static void SetRenderMode(this Entity entity, RenderMode value) => entity.SetInteger(KeyValueUtilities.RenderMode, (int)value);

        /// <summary>Sets the <c>renderamount</c> value.</summary>
        public static void SetRenderAmount(this Entity entity, double value) => entity.SetDouble(KeyValueUtilities.RenderAmount, value);

        /// <summary>Sets the <c>rendercolor</c> value.</summary>
        public static void SetRenderColor(this Entity entity, Vector3 value) => entity.SetVector3(KeyValueUtilities.RenderColor, value);

        /// <summary>
        /// Returns an enumerable without any <c>classname</c> keyvalues.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> WithoutClassName(this IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            ArgumentNullException.ThrowIfNull(keyValues);
            return keyValues.Where(kv => kv.Key != KeyValueUtilities.ClassName);
        }

        /// <summary>
        /// Replaces all keyvalues in <paramref name="entity"/> with those in <paramref name="keyValues"/>.
        /// The class name is not changed.
        /// </summary>
        public static void ReplaceKeyValues(this Entity entity, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity == keyValues)
            {
                return;
            }

            entity.Clear();

            //Copy all keyvalues
            foreach (var kv in keyValues.WithoutClassName())
            {
                entity.SetString(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Replaces all keyvalues in <paramref name="entity"/>> with those in <paramref name="other"/>.
        /// The class name is updated if it differs (unless <paramref name="entity"/> is worldspawn).
        /// </summary>
        public static void ReplaceWith(this Entity entity, Entity other)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(other);

            if (entity == other)
            {
                return;
            }

            entity.Clear();

            if (other.ClassName != entity.ClassName && !entity.IsWorldspawn)
            {
                entity.ClassName = other.ClassName;
            }

            entity.ReplaceKeyValues(other);
        }

        internal static bool HasKeyValueCore(this Entity entity, string key, string value)
        {
            return entity.GetStringOrNull(key) is string candidate && candidate == value;
        }

        /// <summary>
        /// Returns whether the entity has the key <paramref name="key"/> with value <paramref name="value"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/>, <paramref name="key"/> or <paramref name="value"/> are <see langword="null"/>.
        /// </exception>
        public static bool HasKeyValue(this Entity entity, string key, string value)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            return entity.HasKeyValueCore(key, value);
        }
    }
}
