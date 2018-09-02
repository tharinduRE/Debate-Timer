// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppEntry.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CountdownTimer
{
    using System;
    using System.Linq;
    using System.Windows;

    using Hourglass.Extensions;
    using Hourglass.Managers;
    using CountdownTimer.Properties;
    using Hourglass.Timing;
    using CountdownTimer.Windows;

    using Microsoft.VisualBasic;

    using ExitEventArgs = System.Windows.ExitEventArgs;
    using StartupEventArgs = Microsoft.VisualBasic.

    /// <summary>
    /// Handles application start up, command-line arguments, and ensures that only one instance of the application is
    /// running at any time.
    /// </summary>
    public class AppEntry : WindowsFormsApplicationBase
    {
        /// <summary>
        /// An instance of the <see cref="App"/> class.
        /// </summary>
        private App app;

        /// <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            AppEntry appEntry = new AppEntry();
            appEntry.Run(args);
        }

        /// <summary>
        /// Invoked when the application starts.
        /// </summary>
        /// <param name="e">Contains the command-line arguments of the application and indicates whether the
        /// application startup should be canceled.</param>
        /// <returns>A value indicating whether the application should continue starting up.</returns>
        protected override void OnStartup(StartupEventArgs e)
        {
            AppManager.Instance.Initialize();

            this.app = new App();
            this.app.Exit += AppExit;
            this.app.Run();
        }

        /// <summary>
        /// Invoked just before the application shuts down, and cannot be canceled.
        /// </summary>
        /// <param name="sender">The application.</param>
        /// <param name="e">The event data.</param>
        private static void AppExit(object sender, ExitEventArgs e)
        {
            AppManager.Instance.Persist();
            AppManager.Instance.Dispose();
        }
    }
}
