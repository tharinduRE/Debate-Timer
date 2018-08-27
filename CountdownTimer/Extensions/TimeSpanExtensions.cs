﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanExtensions.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hourglass.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using CountdownTimer.Properties;

    /// <summary>
    /// Provides extensions methods for the <see cref="TimeSpan"/> struct.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Rounds a <see cref="TimeSpan"/> down to the nearest second.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/>.</param>
        /// <returns><paramref name="timeSpan"/> rounded down to the nearest second.</returns>
        public static TimeSpan RoundDown(this TimeSpan timeSpan)
        {
            return new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        /// <summary>
        /// Rounds a <see cref="TimeSpan"/> up to the nearest second.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/>.</param>
        /// <returns><paramref name="timeSpan"/> rounded up to the nearest second.</returns>
        public static TimeSpan RoundUp(this TimeSpan timeSpan)
        {
            return new TimeSpan(
                timeSpan.Days,
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds + ((timeSpan.Ticks % TimeSpan.TicksPerSecond) > 0 ? 1 : 0));
        }

        /// <summary>
        /// Rounds a <see cref="Nullable{TimeSpan}"/> down to the nearest second.
        /// </summary>
        /// <param name="timeSpan">A <see cref="Nullable{TimeSpan}"/>.</param>
        /// <returns><paramref name="timeSpan"/> rounded down to the nearest second, or <c>null</c> if <paramref
        /// name="timeSpan"/> is <c>null</c>.</returns>
        public static TimeSpan? RoundDown(this TimeSpan? timeSpan)
        {
            return timeSpan?.RoundDown();
        }

        /// <summary>
        /// Rounds a <see cref="Nullable{TimeSpan}"/> up to the nearest second.
        /// </summary>
        /// <param name="timeSpan">A <see cref="Nullable{TimeSpan}"/>.</param>
        /// <returns><paramref name="timeSpan"/> rounded up to the nearest second, or <c>null</c> if <paramref
        /// name="timeSpan"/> is <c>null</c>.</returns>
        public static TimeSpan? RoundUp(this TimeSpan? timeSpan)
        {
            return timeSpan?.RoundUp();
        }

        /// <summary>
        /// Converts the value of a <see cref="TimeSpan"/> object to its equivalent natural string representation.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/>.</param>
        /// <returns>The natural string representation of the <see cref="TimeSpan"/>.</returns>
        public static string ToNaturalString(this TimeSpan timeSpan)
        {
            return timeSpan.ToNaturalString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the value of a <see cref="TimeSpan"/> object to its equivalent natural string representation.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/>.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/>.</param>
        /// <returns>The natural string representation of the <see cref="TimeSpan"/>.</returns>
        public static string ToNaturalString(this TimeSpan timeSpan, IFormatProvider provider)
        {
            List<string> parts = new List<string>();

            // Days
            if (timeSpan.Days != 0)
            {
                parts.Add(GetStringWithUnits(timeSpan.Days, "Day", provider));
            }

            // Hours
            if (timeSpan.Hours != 0 || parts.Count != 0)
            {
                parts.Add(GetStringWithUnits(timeSpan.Hours, "Hour", provider));
            }

            //// Minutes
            //if (timeSpan.Minutes != 0 || parts.Count != 0)
            //{
            //    parts.Add(GetStringWithUnits(timeSpan.Minutes, "Minute", provider));
            //}

            // Leading Zero Digit for Minutes and Seconds
            if (timeSpan.Minutes <= 9)
            {
                // Minutes - Display even if it is zero
                parts.Add(GetStringWithUnits(timeSpan.Minutes, "DigitMinute", provider));
            }
            else
            {
                // Minutes - Display even if it is zero
                parts.Add(GetStringWithUnits(timeSpan.Minutes, "Minute", provider)); 
            }


            if (timeSpan.Seconds <= 9)
            {    // Seconds
                parts.Add(GetStringWithUnits(timeSpan.Seconds, "DigitSecond", provider));
            }
            else
            {
                // Seconds
                parts.Add(GetStringWithUnits(timeSpan.Seconds, "Second", provider)); 
            }

            // Join parts
            return string.Join(
                Resources.ResourceManager.GetString("TimeSpanExtensionsUnitSeparator", provider),
                parts);
        }

        /// <summary>
        /// Converts the value of a <see cref="Nullable{TimeSpan}"/> object to its equivalent natural string
        /// representation.
        /// </summary>
        /// <param name="timeSpan">A <see cref="Nullable{TimeSpan}"/>.</param>
        /// <returns>The natural string representation of the <see cref="TimeSpan"/> represented by <paramref
        /// name="timeSpan"/>, or <see cref="string.Empty"/> if <paramref name="timeSpan"/> is <c>null</c>.</returns>
        public static string ToNaturalString(this TimeSpan? timeSpan)
        {
            return timeSpan.ToNaturalString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the value of a <see cref="Nullable{TimeSpan}"/> object to its equivalent natural string
        /// representation.
        /// </summary>
        /// <param name="timeSpan">A <see cref="Nullable{TimeSpan}"/>.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/>.</param>
        /// <returns>The natural string representation of the <see cref="TimeSpan"/> represented by <paramref
        /// name="timeSpan"/>, or <see cref="string.Empty"/> if <paramref name="timeSpan"/> is <c>null</c>.</returns>
        public static string ToNaturalString(this TimeSpan? timeSpan, IFormatProvider provider)
        {
            return timeSpan.HasValue ? timeSpan.Value.ToNaturalString(provider) : string.Empty;
        }

        /// <summary>
        /// Returns a string for the specified value with the specified unit (e.g., "5 minutes").
        /// </summary>
        /// <param name="value">A value.</param>
        /// <param name="unit">The unit part of the resource name for the unit string.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/>.</param>
        /// <returns>A string for the specified value with the specified unit.</returns>
        private static string GetStringWithUnits(int value, string unit, IFormatProvider provider)
        {
            string resourceName = string.Format(
                CultureInfo.InvariantCulture,
                "TimeSpanExtensions{0}{1}FormatString",
                value == 1 ? "1" : "N",
                value == 1 ? unit : unit + "s");

            return string.Format(
                Resources.ResourceManager.GetEffectiveProvider(provider),
                Resources.ResourceManager.GetString(resourceName, provider),
                value);
        }
    }
}
