﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hourglass.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CountdownTimer.Properties;
    using Hourglass.Timing;

    /// <summary>
    /// Manages themes.
    /// </summary>
    public class ThemeManager : Manager
    {
        /// <summary>
        /// Singleton instance of the <see cref="ThemeManager"/> class.
        /// </summary>
        public static readonly ThemeManager Instance = new ThemeManager();

        /// <summary>
        /// A collection of themes.
        /// </summary>
        private readonly List<Theme> themes = new List<Theme>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ThemeManager"/> class from being created.
        /// </summary>
        private ThemeManager()
        {
        }

        /// <summary>
        /// Gets the default theme.
        /// </summary>
        public Theme DefaultTheme
        {
            get { return this.GetThemeByIdentifier("blue"); }
        }

        /// <summary>
        /// Gets the default dark theme.
        /// </summary>
        public Theme DefaultDarkTheme
        {
            get { return this.GetThemeByIdentifier("blue-dark"); }
        }

        /// <summary>
        /// Gets a collection of all themes.
        /// </summary>
        public IList<Theme> AllThemes
        {
            get { return this.themes.ToList(); }
        }

        /// <summary>
        /// Gets a collection of the themes stored in the assembly.
        /// </summary>
        public IList<Theme> BuiltInThemes
        {
            get { return this.themes.Where(t => t.Type != ThemeType.UserProvided).ToList(); }
        }

        /// <summary>
        /// Gets a collection of the light themes stored in the assembly.
        /// </summary>
        public IList<Theme> BuiltInLightThemes
        {
            get { return this.themes.Where(t => t.Type == ThemeType.BuiltInLight).ToList(); }
        }

        /// <summary>
        /// Gets a collection of the dark themes stored in the assembly.
        /// </summary>
        public IList<Theme> BuiltInDarkThemes
        {
            get { return this.themes.Where(t => t.Type == ThemeType.BuiltInDark).ToList(); }
        }

        /// <summary>
        /// Gets a collection of the themes defined by the user ordered by name.
        /// </summary>
        public IList<Theme> UserProvidedThemes
        {
            get { return this.themes.Where(t => t.Type == ThemeType.UserProvided).OrderBy(t => t.Name).ToList(); }
        }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        public override void Initialize()
        {
            this.themes.Clear();
            this.AddRange(this.GetBuiltInThemes());
            this.AddRange(this.GetUserProvidedThemes());
        }

        /// <summary>
        /// Persists the state of the class.
        /// </summary>
        public override void Persist()
        {
            Settings.Default.UserProvidedThemes = this.UserProvidedThemes;
        }

        /// <summary>
        /// Adds a theme.
        /// </summary>
        /// <param name="theme">A <see cref="Theme"/>.</param>
        public void Add(Theme theme)
        {
            if (this.GetThemeByIdentifier(theme.Identifier) == null)
            {
                this.themes.Add(theme);
            }
        }

        /// <summary>
        /// Adds the themes of the specified collection.
        /// </summary>
        /// <param name="collection">A collection of <see cref="Theme"/>s.</param>
        public void AddRange(IEnumerable<Theme> collection)
        {
            foreach (Theme theme in collection)
            {
                this.Add(theme);
            }
        }

        /// <summary>
        /// Adds a theme based on another theme.
        /// </summary>
        /// <param name="theme">A <see cref="Theme"/>.</param>
        /// <returns>The newly added theme.</returns>
        public Theme AddThemeBasedOnTheme(Theme theme)
        {
            string identifier = Guid.NewGuid().ToString();
            string name = Resources.ThemeManagerNewTheme;
            Theme newTheme = Theme.FromTheme(ThemeType.UserProvided, identifier, name, theme);
            this.Add(newTheme);
            return newTheme;
        }

        /// <summary>
        /// Returns the theme for the specified identifier, or <c>null</c> if no such theme is loaded.
        /// </summary>
        /// <param name="identifier">The identifier for the theme.</param>
        /// <returns>The theme for the specified identifier, or <c>null</c> if no such theme is loaded.</returns>
        public Theme GetThemeByIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            return this.themes.FirstOrDefault(t => t.Identifier == identifier);
        }

        /// <summary>
        /// Returns the theme for the specified identifier, or <see cref="DefaultTheme"/> if no such theme is loaded.
        /// </summary>
        /// <param name="identifier">The identifier for the theme.</param>
        /// <returns>The theme for the specified identifier, or <see cref="DefaultTheme"/> if no such theme is loaded.
        /// </returns>
        public Theme GetThemeOrDefaultByIdentifier(string identifier)
        {
            return this.GetThemeByIdentifier(identifier) ?? this.DefaultTheme;
        }

        /// <summary>
        /// Returns the first theme for the specified name, or <c>null</c> if no such theme is loaded.
        /// </summary>
        /// <param name="name">The name for the theme.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies how the strings will be
        /// compared.</param>
        /// <returns>The first theme for the specified name, or <c>null</c> if no such theme is loaded.</returns>
        public Theme GetThemeByName(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return this.themes.FirstOrDefault(t => string.Equals(t.Name, name, stringComparison));
        }

        /// <summary>
        /// Returns the first theme for the specified name, or <see cref="DefaultTheme"/> if no such theme is loaded.
        /// </summary>
        /// <param name="name">The name for the theme.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies how the strings will be
        /// compared.</param>
        /// <returns>The first theme for the specified name, or <see cref="DefaultTheme"/> if no such theme is loaded.
        /// </returns>
        public Theme GetThemeOrDefaultByName(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            return this.GetThemeByName(name, stringComparison) ?? this.DefaultTheme;
        }

        /// <summary>
        /// Returns the light variant of a theme.
        /// </summary>
        /// <param name="theme">A theme.</param>
        /// <returns>The light variant of the <paramref name="theme"/>.</returns>
        public Theme GetLightVariantForTheme(Theme theme)
        {
            switch (theme.Type)
            {
                case ThemeType.BuiltInLight:
                    return theme;

                case ThemeType.BuiltInDark:
                    return this.GetThemeOrDefaultByIdentifier(theme.Identifier.Replace("-dark", string.Empty));

                case ThemeType.UserProvided:
                    return this.DefaultTheme;

                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the dark variant of a theme.
        /// </summary>
        /// <param name="theme">A theme.</param>
        /// <returns>The dark variant of the <paramref name="theme"/>.</returns>
        public Theme GetDarkVariantForTheme(Theme theme)
        {
            switch (theme.Type)
            {
                case ThemeType.BuiltInLight:
                    return this.GetThemeOrDefaultByIdentifier(theme.Identifier + "-dark");

                case ThemeType.BuiltInDark:
                    return theme;

                case ThemeType.UserProvided:
                    return this.DefaultDarkTheme;

                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Removes a theme, and updates any timers using the theme to use the default theme.
        /// </summary>
        /// <param name="theme">A <see cref="Theme"/>.</param>
        public void Remove(Theme theme)
        {
            foreach (Timer timer in TimerManager.Instance.Timers.Where(t => t.Options.Theme == theme))
            {
                timer.Options.Theme = this.DefaultTheme;
            }

            this.themes.Remove(theme);
        }

        /// <summary>
        /// Loads the collection of themes defined in the assembly.
        /// </summary>
        /// <returns>A collection of themes defined in the assembly.</returns>
        private IList<Theme> GetBuiltInThemes()
        {
            return new List<Theme>
            {
                // Light themes
                new Theme(
                    ThemeType.BuiltInLight,
                    "red" /* identifier */,
                    Resources.ThemeManagerRedLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#C75050" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "orange" /* identifier */,
                    Resources.ThemeManagerOrangeLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#FF7F50" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "yellow" /* identifier */,
                    Resources.ThemeManagerYellowLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#FFC800" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "green" /* identifier */,
                    Resources.ThemeManagerGreenLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#57A64A" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "blue" /* identifier */,
                    Resources.ThemeManagerBlueLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#3665B3" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "purple" /* identifier */,
                    Resources.ThemeManagerPurpleLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#843179" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "gray" /* identifier */,
                    Resources.ThemeManagerGrayLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#666666" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInLight,
                    "black" /* identifier */,
                    Resources.ThemeManagerBlackLightTheme /* name */,
                    "#FFFFFF" /* backgroundColor */,
                    "#000000" /* progressBarColor */,
                    "#EEEEEE" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#000000" /* mainTextColor */,
                    "#808080" /* mainHintColor */,
                    "#808080" /* secondaryTextColor */,
                    "#808080" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),

                // Dark themes
                new Theme(
                    ThemeType.BuiltInDark,
                    "red-dark" /* identifier */,
                    Resources.ThemeManagerRedDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#C75050" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "orange-dark" /* identifier */,
                    Resources.ThemeManagerOrangeDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#FF7F50" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "yellow-dark" /* identifier */,
                    Resources.ThemeManagerYellowDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#FFC800" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "green-dark" /* identifier */,
                    Resources.ThemeManagerGreenDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#57A64A" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "blue-dark" /* identifier */,
                    Resources.ThemeManagerBlueDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#3665B3" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "purple-dark" /* identifier */,
                    Resources.ThemeManagerPurpleDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#843179" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "gray-dark" /* identifier */,
                    Resources.ThemeManagerGrayDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#666666" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */),
                new Theme(
                    ThemeType.BuiltInDark,
                    "black-dark" /* identifier */,
                    Resources.ThemeManagerBlackDarkTheme /* name */,
                    "#1E1E1E" /* backgroundColor */,
                    "#000000" /* progressBarColor */,
                    "#2D2D30" /* progressBackgroundColor */,
                    "#C75050" /* expirationFlashColor */,
                    "#808080" /* mainTextColor */,
                    "#505050" /* mainHintColor */,
                    "#505050" /* secondaryTextColor */,
                    "#505050" /* secondaryHintColor */,
                    "#0066CC" /* buttonColor */,
                    "#FF0000" /* buttonHoverColor */)
            };
        }

        /// <summary>
        /// Loads the collection of themes defined by the user.
        /// </summary>
        /// <returns>A collection of sounds defined by the user.</returns>
        private IList<Theme> GetUserProvidedThemes()
        {
            return Settings.Default.UserProvidedThemes;
        }
    }
}
