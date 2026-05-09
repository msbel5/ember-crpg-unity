using System;

// Design note:
// SkillProgression owns XP threshold and rust formulas for actor skills.
// It transforms SkillRecord values but does not award XP, choose jobs, roll quality, or touch world state.
namespace EmberCrpg.Domain.Skills
{
    /// <summary>
    /// Pure skill progression rules for XP levels and rust.
    /// </summary>
    public static class SkillProgression
    {
        private static readonly int[] Thresholds =
        {
            0, 500, 1100, 1800, 2600, 3500, 4500, 5600,
            6800, 8100, 9500, 11000, 12600, 14300, 16100
        };

        private static readonly string[] LevelNames =
        {
            "Dabbling", "Novice", "Adequate", "Competent", "Skilled",
            "Proficient", "Talented", "Adept", "Expert", "Professional",
            "Accomplished", "Great", "Master", "High Master", "Grand Master"
        };

        /// <summary>
        /// Derives skill level from cumulative XP.
        /// </summary>
        public static int LevelFromXp(int xp)
        {
            if (xp < 0)
                return 0;

            if (xp >= Thresholds[14])
                return 14 + ((xp - Thresholds[14]) / 2000);

            for (var level = Thresholds.Length - 1; level >= 0; level--)
            {
                if (xp >= Thresholds[level])
                    return level;
            }

            return 0;
        }

        /// <summary>
        /// Returns the display name for a skill level.
        /// </summary>
        public static string LevelName(int level)
        {
            if (level < 0)
                return LevelNames[0];

            if (level >= LevelNames.Length)
                return "Legendary";

            return LevelNames[level];
        }

        /// <summary>
        /// Returns the unused-tick threshold before rust can apply.
        /// </summary>
        public static int RustThreshold(int level)
        {
            return level >= 15 ? 500 : 200;
        }

        /// <summary>
        /// Applies one rust tick and returns the updated skill record.
        /// </summary>
        public static SkillRecord TickRust(SkillRecord record, bool usedThisTick)
        {
            if (usedThisTick)
                return record.WithRust(Math.Max(0, record.RustyLevel - 1), 0);

            var unusedCounter = record.UnusedCounter + 1;
            if (unusedCounter > RustThreshold(record.Level))
                return record.WithRust(record.RustyLevel + 1, 0);

            return record.WithRust(record.RustyLevel, unusedCounter);
        }
    }
}