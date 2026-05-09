using System;
using EmberCrpg.Domain.Core;
using NUnit.Framework;

// Design note:
// These tests pin GameTime as a deterministic total-minute calendar primitive.
// They do not cover simulation ordering, replay ticks, real-time advancement,
// Unity clocks, weather, schedules, or logging.
namespace EmberCrpg.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies Ember's deterministic total-minute game calendar.
    /// </summary>
    public sealed class GameTimeTests
    {
        /// <summary>
        /// A constructed timestamp exposes the supplied total game minutes.
        /// </summary>
        [Test]
        public void Constructor_StoresTotalMinutes()
        {
            Assert.That(new GameTime(42L).TotalMinutes, Is.EqualTo(42L));
        }

        /// <summary>
        /// Negative total minutes are rejected before the game epoch.
        /// </summary>
        [Test]
        public void Constructor_NegativeTotalMinutes_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameTime(-1L));
        }

        /// <summary>
        /// The minute component wraps within the hour.
        /// </summary>
        [Test]
        public void Minute_WrapsWithinHour()
        {
            Assert.That(new GameTime(61L).Minute, Is.EqualTo(1));
        }

        /// <summary>
        /// The hour component wraps within the day.
        /// </summary>
        [Test]
        public void Hour_WrapsWithinDay()
        {
            Assert.That(new GameTime(25L * 60L).Hour, Is.EqualTo(1));
        }

        /// <summary>
        /// The day of month is one-based inside a thirty-day month.
        /// </summary>
        [Test]
        public void DayOfMonth_IsOneBasedWithinThirtyDayMonth()
        {
            Assert.That(new GameTime(30L * GameTime.MinutesPerDay).DayOfMonth, Is.EqualTo(1));
        }

        /// <summary>
        /// The month is one-based inside a twelve-month year.
        /// </summary>
        [Test]
        public void Month_IsOneBasedWithinYear()
        {
            Assert.That(new GameTime(GameTime.MinutesPerMonth).Month, Is.EqualTo(2));
        }

        /// <summary>
        /// The year is one-based and advances after each full year.
        /// </summary>
        [Test]
        public void Year_IsOneBasedAndAdvancesEveryYear()
        {
            Assert.That(new GameTime(GameTime.MinutesPerYear).Year, Is.EqualTo(2));
        }

        /// <summary>
        /// The day of year is one-based inside a 360-day year.
        /// </summary>
        [Test]
        public void DayOfYear_IsOneBasedWithinYear()
        {
            Assert.That(new GameTime(41L * GameTime.MinutesPerDay).DayOfYear, Is.EqualTo(42));
        }

        /// <summary>
        /// AddMinutes returns a new timestamp advanced by raw minutes.
        /// </summary>
        [Test]
        public void AddMinutes_ReturnsAdvancedTime()
        {
            Assert.That(new GameTime(10L).AddMinutes(5L).TotalMinutes, Is.EqualTo(15L));
        }

        /// <summary>
        /// AddMinutes rejects movement before the game epoch through the constructor invariant.
        /// </summary>
        [Test]
        public void AddMinutes_BeforeEpoch_ThrowsArgumentOutOfRange()
        {
            var time = new GameTime(3L);

            Assert.Throws<ArgumentOutOfRangeException>(() => time.AddMinutes(-4L));
        }

        /// <summary>
        /// AddHours uses sixty-minute hours.
        /// </summary>
        [Test]
        public void AddHours_UsesSixtyMinuteHours()
        {
            Assert.That(new GameTime(0L).AddHours(2L).TotalMinutes, Is.EqualTo(120L));
        }

        /// <summary>
        /// AddDays uses 1440-minute days.
        /// </summary>
        [Test]
        public void AddDays_Uses1440MinuteDays()
        {
            Assert.That(new GameTime(0L).AddDays(2L).TotalMinutes, Is.EqualTo(2880L));
        }

        /// <summary>
        /// AddYears uses 518400-minute years.
        /// </summary>
        [Test]
        public void AddYears_Uses518400MinuteYears()
        {
            Assert.That(new GameTime(0L).AddYears(2L).TotalMinutes, Is.EqualTo(1036800L));
        }

        /// <summary>
        /// The plus operator advances time by minutes.
        /// </summary>
        [Test]
        public void Operator_Plus_AddsMinutes()
        {
            Assert.That((new GameTime(10L) + 5L).TotalMinutes, Is.EqualTo(15L));
        }

        /// <summary>
        /// The minus operator returns the signed delta in minutes.
        /// </summary>
        [Test]
        public void Operator_Minus_ReturnsDeltaMinutes()
        {
            Assert.That(new GameTime(30L) - new GameTime(10L), Is.EqualTo(20L));
        }

        /// <summary>
        /// The less-than operator compares total minutes.
        /// </summary>
        [Test]
        public void Operator_LessThan_ComparesByTotalMinutes()
        {
            Assert.That(new GameTime(10L) < new GameTime(20L), Is.True);
        }

        /// <summary>
        /// Equality operators match the Equals method.
        /// </summary>
        [Test]
        public void Operator_EqualsAndNotEquals_MatchEqualsMethod()
        {
            var left = new GameTime(10L);
            var equal = new GameTime(10L);
            var different = new GameTime(20L);

            Assert.That(left == equal, Is.EqualTo(left.Equals(equal)));
            Assert.That(left != different, Is.EqualTo(!left.Equals(different)));
        }

        /// <summary>
        /// ToString includes the year, day of year, and zero-padded time.
        /// </summary>
        [Test]
        public void ToString_IncludesYearDayAndTime()
        {
            var time = new GameTime(41L * 1440L + 8L * 60L + 30L);

            Assert.That(time.ToString(), Is.EqualTo("Year 1 Day 42 08:30"));
        }
    }
}