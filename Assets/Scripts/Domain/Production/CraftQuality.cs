using System;

// Design note:
// CraftQuality owns skill-based output quality tiers for production systems.
// It is a pure formula layer; job completion, item creation, XP awards, and random streams live elsewhere.
namespace EmberCrpg.Domain.Production
{
    /// <summary>
    /// Stable quality tier for produced items and crafted outputs.
    /// </summary>
    public enum QualityLevel
    {
        /// <summary>Baseline output with no quality bonus.</summary>
        Ordinary = 0,

        /// <summary>Slightly improved output.</summary>
        WellCrafted = 1,

        /// <summary>Clearly refined output.</summary>
        FinelyCrafted = 2,

        /// <summary>High-grade output.</summary>
        Superior = 3,

        /// <summary>Rare high-quality output.</summary>
        Exceptional = 4,

        /// <summary>Best possible crafted output tier.</summary>
        Masterwork = 5
    }

    /// <summary>
    /// Pure quality selection formula for production outputs.
    /// </summary>
    public static class CraftQuality
    {
        /// <summary>
        /// Determines output quality from effective skill and deterministic random value.
        /// </summary>
        public static QualityLevel FromEffectiveSkill(int effectiveSkill, double rngValue)
        {
            var skill = Math.Max(0, effectiveSkill);
            var roll = ClampRoll(rngValue);

            if (skill <= 2)
                return roll < 0.90 ? QualityLevel.Ordinary : QualityLevel.WellCrafted;

            if (skill <= 5)
                return roll < 0.50 ? QualityLevel.Ordinary :
                    roll < 0.85 ? QualityLevel.WellCrafted : QualityLevel.FinelyCrafted;

            if (skill <= 8)
                return roll < 0.20 ? QualityLevel.Ordinary :
                    roll < 0.50 ? QualityLevel.WellCrafted :
                    roll < 0.80 ? QualityLevel.FinelyCrafted :
                    roll < 0.95 ? QualityLevel.Superior : QualityLevel.Exceptional;

            if (skill <= 11)
                return roll < 0.05 ? QualityLevel.Ordinary :
                    roll < 0.20 ? QualityLevel.WellCrafted :
                    roll < 0.50 ? QualityLevel.FinelyCrafted :
                    roll < 0.80 ? QualityLevel.Superior :
                    roll < 0.95 ? QualityLevel.Exceptional : QualityLevel.Masterwork;

            if (skill <= 14)
                return roll < 0.05 ? QualityLevel.WellCrafted :
                    roll < 0.20 ? QualityLevel.FinelyCrafted :
                    roll < 0.50 ? QualityLevel.Superior :
                    roll < 0.80 ? QualityLevel.Exceptional : QualityLevel.Masterwork;

            return roll < 0.05 ? QualityLevel.FinelyCrafted :
                roll < 0.20 ? QualityLevel.Superior :
                roll < 0.50 ? QualityLevel.Exceptional : QualityLevel.Masterwork;
        }

        private static double ClampRoll(double rngValue)
        {
            if (double.IsNaN(rngValue) || rngValue < 0.0)
                return 0.0;

            if (rngValue >= 1.0)
                return 0.999999;

            return rngValue;
        }
    }
}