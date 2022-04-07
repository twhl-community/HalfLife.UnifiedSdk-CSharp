namespace HalfLife.UnifiedSdk.Formats.Skill
{
    /// <summary>
    /// A <c>skill.json</c> section.
    /// </summary>
    public class SkillSection
    {
        /// <summary>
        /// Section description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Section condition.
        /// </summary>
        public string? Condition { get; set; }

        /// <summary>
        /// Set of skill variables.
        /// </summary>
        public Dictionary<string, float> Variables { get; set; } = new();
    }
}
