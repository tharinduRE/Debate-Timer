// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextMenu.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CountdownTimer.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Hourglass.Extensions;
    using Hourglass.Managers;
    using CountdownTimer.Properties;
    using Hourglass.Timing;

    /// <summary>
    /// A <see cref="System.Windows.Controls.ContextMenu"/> for the <see cref="TimerWindow"/>.
    /// </summary>
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        #region Private Members

        /// <summary>
        /// The <see cref="TimerWindow"/> that uses this context menu.
        /// </summary>
        private TimerWindow timerWindow;

        /// <summary>
        /// A <see cref="DispatcherTimer"/> used to raise events.
        /// </summary>
        private DispatcherTimer dispatcherTimer;

        /// <summary>
        /// The "New timer" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem newTimerMenuItem;

        /// <summary>
        /// The "Always on top" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem alwaysOnTopMenuItem;

        /// <summary>
        /// The "Full screen" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem fullScreenMenuItem;

        /// <summary>
        /// The "Prompt on exit" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem promptOnExitMenuItem;

        /// <summary>
        /// The "Show progress in taskbar" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem showProgressInTaskbarMenuItem;

        /// <summary>
        /// The "Show in notification area" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem showInNotificationAreaMenuItem;

        /// <summary>
        /// The "Theme" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem themeMenuItem;

        ///// <summary>
        ///// The "Light theme" <see cref="MenuItem"/>.
        ///// </summary>
        //private MenuItem lightThemeMenuItem;

        ///// <summary>
        ///// The "Dark theme" <see cref="MenuItem"/>.
        ///// </summary>
        //private MenuItem darkThemeMenuItem;

        ///// <summary>
        ///// The "Manage themes" <see cref="MenuItem"/>.
        ///// </summary>
        //private MenuItem manageThemesMenuItem;

        /// <summary>
        /// The "Theme" <see cref="MenuItem"/>s associated with <see cref="Theme"/>s.
        /// </summary>
        private IList<MenuItem> selectableThemeMenuItems;

        /// <summary>
        /// The "Sound" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem soundMenuItem;

        /// <summary>
        /// The "Sound" <see cref="MenuItem"/>s associated with <see cref="Sound"/>s.
        /// </summary>
        private IList<MenuItem> selectableSoundMenuItems;

        /// <summary>
        /// The "Loop sound" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem loopSoundMenuItem;

        /// <summary>
        /// The "About Menu Item" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem AboutMenuItem;

        /// <summary>
        /// The "Close" <see cref="MenuItem"/>.
        /// </summary>
        private MenuItem closeMenuItem;

        /// <summary>
        /// The date and time the menu was last visible.
        /// </summary>
        private DateTime lastShowed = DateTime.MinValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the date and time the menu was last visible.
        /// </summary>
        public DateTime LastShowed
        {
            get { return this.lastShowed; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds the <see cref="ContextMenu"/> to a <see cref="TimerWindow"/>.
        /// </summary>
        /// <param name="window">A <see cref="TimerWindow"/>.</param>
        public void Bind(TimerWindow window)
        {
            // Validate parameters
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            // Validate state
            if (this.timerWindow != null)
            {
                throw new InvalidOperationException();
            }

            // Initialize members
            this.timerWindow = window;

            this.timerWindow.ContextMenuOpening += this.WindowContextMenuOpening;
            this.timerWindow.ContextMenuClosing += this.WindowContextMenuClosing;
            this.timerWindow.ContextMenu = this;

            this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            this.dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            this.dispatcherTimer.Tick += this.DispatcherTimerTick;

            this.selectableThemeMenuItems = new List<MenuItem>();
            this.selectableSoundMenuItems = new List<MenuItem>();
            //this.selectableWindowTitleMenuItems = new List<MenuItem>();

            // Build the menu
            this.BuildMenu();
        }

        #endregion

        #region Private Methods (Lifecycle)

        /// <summary>
        /// Invoked when the context menu is opened.
        /// </summary>
        /// <param name="sender">The bound <see cref="TimerWindow"/>.</param>
        /// <param name="e">The event data.</param>
        private void WindowContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Do not show the context menu if the user interface is locked
            if (this.timerWindow.Options.LockInterface)
            {
                e.Handled = true;
                return;
            }

            // Update dynamic items
            //this.UpdateRecentInputsMenuItem();
            //this.UpdateSavedTimersMenuItem();
            //this.UpdateThemeMenuItem();
            this.UpdateSoundMenuItem();

            // Update binding
            this.UpdateMenuFromOptions();

            this.lastShowed = DateTime.Now;
            this.dispatcherTimer.Start();
        }

        /// <summary>
        /// Invoked when the <see cref="dispatcherTimer"/> interval has elapsed.
        /// </summary>
        /// <param name="sender">The <see cref="DispatcherTimer"/>.</param>
        /// <param name="e">The event data.</param>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            this.lastShowed = DateTime.Now;
            //this.UpdateSavedTimersHeaders();
        }

        /// <summary>
        /// Invoked just before the context menu is closed.
        /// </summary>
        /// <param name="sender">The bound <see cref="TimerWindow"/>.</param>
        /// <param name="e">The event data.</param>
        private void WindowContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            this.UpdateOptionsFromMenu();

            this.lastShowed = DateTime.Now;
            this.dispatcherTimer.Stop();

            AppManager.Instance.Persist();
        }

        #endregion

        #region Private Methods (Binding)

        /// <summary>
        /// Reads the options from the <see cref="TimerOptions"/> and applies them to this menu.
        /// </summary>
        private void UpdateMenuFromOptions()
        {
            // Always on top
            this.alwaysOnTopMenuItem.IsChecked = this.timerWindow.Options.AlwaysOnTop;

            // Full screen
            this.fullScreenMenuItem.IsChecked = this.timerWindow.IsFullScreen;

            // Prompt on exit
            this.promptOnExitMenuItem.IsChecked = this.timerWindow.Options.PromptOnExit;

            // Show progress in taskbar
            this.showProgressInTaskbarMenuItem.IsChecked = this.timerWindow.Options.ShowProgressInTaskbar;

            // Show in notification area
            this.showInNotificationAreaMenuItem.IsChecked = Settings.Default.ShowInNotificationArea;

            // Sound
            foreach (MenuItem menuItem in this.selectableSoundMenuItems)
            {
                menuItem.IsChecked = menuItem.Tag == this.timerWindow.Options.Sound;
            }

            // Loop sound
            this.loopSoundMenuItem.IsChecked = this.timerWindow.Options.LoopSound;

        }

        /// <summary>
        /// Reads the options from this menu and applies them to the <see cref="TimerOptions"/>.
        /// </summary>
        private void UpdateOptionsFromMenu()
        {
            // Always on top
            this.timerWindow.Options.AlwaysOnTop = this.alwaysOnTopMenuItem.IsChecked;

            // Full screen
            this.timerWindow.IsFullScreen = this.fullScreenMenuItem.IsChecked;

            // Prompt on exit
            this.timerWindow.Options.PromptOnExit = this.promptOnExitMenuItem.IsChecked;

            // Show progress in taskbar
            this.timerWindow.Options.ShowProgressInTaskbar = this.showProgressInTaskbarMenuItem.IsChecked;

            // Show in notification area
            Settings.Default.ShowInNotificationArea = this.showInNotificationAreaMenuItem.IsChecked;

            // Sound
            MenuItem selectedSoundMenuItem = this.selectableSoundMenuItems.FirstOrDefault(mi => mi.IsChecked);
            this.timerWindow.Options.Sound = selectedSoundMenuItem != null ? selectedSoundMenuItem.Tag as Sound : null;

            // Loop sound
            this.timerWindow.Options.LoopSound = this.loopSoundMenuItem.IsChecked;

        }

        /// <summary>
        /// Invoked when a checkable <see cref="MenuItem"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="MenuItem"/> where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void CheckableMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.UpdateOptionsFromMenu();
            this.UpdateMenuFromOptions();
        }

        #endregion

        #region Private Methods (Building)

        /// <summary>
        /// Builds or rebuilds the context menu.
        /// </summary>
        private void BuildMenu()
        {
            this.Items.Clear();

            // New timer
            this.newTimerMenuItem = new MenuItem();
            this.newTimerMenuItem.Header = Properties.Resources.ContextMenuNewTimerMenuItem;
            this.newTimerMenuItem.Click += this.NewTimerMenuItemClick;
            this.Items.Add(this.newTimerMenuItem);

            this.Items.Add(new Separator());

            // Always on top
            this.alwaysOnTopMenuItem = new MenuItem();
            this.alwaysOnTopMenuItem.Header = Properties.Resources.ContextMenuAlwaysOnTopMenuItem;
            this.alwaysOnTopMenuItem.IsCheckable = true;
            this.alwaysOnTopMenuItem.Click += this.CheckableMenuItemClick;
            this.Items.Add(this.alwaysOnTopMenuItem);

            // Full screen
            this.fullScreenMenuItem = new MenuItem();
            this.fullScreenMenuItem.Header = Properties.Resources.ContextMenuFullScreenMenuItem;
            this.fullScreenMenuItem.IsCheckable = true;
            this.fullScreenMenuItem.Click += this.CheckableMenuItemClick;
            this.Items.Add(this.fullScreenMenuItem);

            // Prompt on exit
            this.promptOnExitMenuItem = new MenuItem();
            this.promptOnExitMenuItem.Header = Properties.Resources.ContextMenuPromptOnExitMenuItem;
            this.promptOnExitMenuItem.IsCheckable = true;
            this.promptOnExitMenuItem.Click += this.CheckableMenuItemClick;
            this.Items.Add(this.promptOnExitMenuItem);

            // Show progress in taskbar
            this.showProgressInTaskbarMenuItem = new MenuItem();
            this.showProgressInTaskbarMenuItem.Header = Properties.Resources.ContextMenuShowProgressInTaskbarMenuItem;
            this.showProgressInTaskbarMenuItem.IsCheckable = true;
            this.showProgressInTaskbarMenuItem.Click += this.CheckableMenuItemClick;
            this.Items.Add(this.showProgressInTaskbarMenuItem);

            // Show in notification area
            this.showInNotificationAreaMenuItem = new MenuItem();
            this.showInNotificationAreaMenuItem.Header = Properties.Resources.ContextMenuShowInNotificationAreaMenuItem;
            this.showInNotificationAreaMenuItem.IsCheckable = true;
            this.showInNotificationAreaMenuItem.Click += this.CheckableMenuItemClick;
            this.Items.Add(this.showInNotificationAreaMenuItem);

            this.Items.Add(new Separator());

            // Theme
            this.themeMenuItem = new MenuItem();
            this.themeMenuItem.Header = Properties.Resources.ContextMenuThemeMenuItem;
            this.Items.Add(this.themeMenuItem);

            // Sound
            this.soundMenuItem = new MenuItem();
            this.soundMenuItem.Header = Properties.Resources.ContextMenuSoundMenuItem;
            this.Items.Add(this.soundMenuItem);

            Separator separator = new Separator();
            this.Items.Add(separator);

            // About
            this.AboutMenuItem = new MenuItem();
            this.AboutMenuItem.Header = Properties.Resources.ContextMenuAboutMenuItem;
            this.AboutMenuItem.Click += this.AboutMenuItemClick;
            this.Items.Add(this.AboutMenuItem);
            this.Items.Add(new Separator());

            // Close
            this.closeMenuItem = new MenuItem();
            this.closeMenuItem.Header = Properties.Resources.ContextMenuCloseMenuItem;
            this.closeMenuItem.Click += this.CloseMenuItemClick;
            this.Items.Add(this.closeMenuItem);
        }

        #endregion

        #region Private Methods (New Timer)

        /// <summary>
        /// Invoked when the "New timer" <see cref="MenuItem"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="MenuItem"/> where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void NewTimerMenuItemClick(object sender, RoutedEventArgs e)
        {
            TimerWindow window = new TimerWindow();
            window.RestoreFromWindow(this.timerWindow);
            window.Show();
        }

        #endregion

        #region Private Methods (Sound)

        /// <summary>
        /// Updates the <see cref="soundMenuItem"/>.
        /// </summary>
        private void UpdateSoundMenuItem()
        {
            this.soundMenuItem.Items.Clear();
            this.selectableSoundMenuItems.Clear();

            // Sounds
            this.CreateSoundMenuItem(Sound.NoSound);
            this.CreateSoundMenuItemsFromList(SoundManager.Instance.BuiltInSounds);
            this.CreateSoundMenuItemsFromList(SoundManager.Instance.UserProvidedSounds);

            // Options
            this.soundMenuItem.Items.Add(new Separator());

            if (this.loopSoundMenuItem == null)
            {
                this.loopSoundMenuItem = new MenuItem();
                this.loopSoundMenuItem.Header = Properties.Resources.ContextMenuLoopSoundMenuItem;
                this.loopSoundMenuItem.IsCheckable = true;
                this.loopSoundMenuItem.Click += this.CheckableMenuItemClick;
            }

            this.soundMenuItem.Items.Add(this.loopSoundMenuItem);
        }

        /// <summary>
        /// Creates a <see cref="MenuItem"/> for a <see cref="Sound"/>.
        /// </summary>
        /// <param name="sound">A <see cref="Sound"/>.</param>
        private void CreateSoundMenuItem(Sound sound)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = sound != null ? sound.Name : Properties.Resources.ContextMenuNoSoundMenuItem;
            menuItem.Tag = sound;
            menuItem.IsCheckable = true;
            menuItem.Click += this.SoundMenuItemClick;
            menuItem.Click += this.CheckableMenuItemClick;

            this.soundMenuItem.Items.Add(menuItem);
            this.selectableSoundMenuItems.Add(menuItem);
        }

        /// <summary>
        /// Creates a <see cref="MenuItem"/> for each <see cref="Sound"/> in the collection.
        /// </summary>
        /// <param name="sounds">A collection of <see cref="Sound"/>s.</param>
        private void CreateSoundMenuItemsFromList(IList<Sound> sounds)
        {
            if (sounds.Count > 0)
            {
                this.soundMenuItem.Items.Add(new Separator());
                foreach (Sound sound in sounds)
                {
                    this.CreateSoundMenuItem(sound);
                }
            }
        }

        /// <summary>
        /// Invoked when a sound <see cref="MenuItem"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="MenuItem"/> where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void SoundMenuItemClick(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem menuItem in this.selectableSoundMenuItems)
            {
                menuItem.IsChecked = object.ReferenceEquals(menuItem, sender);
            }
        }

        #endregion

        #region Private Methods (About)

        /// <summary>
        /// Invoked when the "About" <see cref="MenuItem"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="MenuItem"/> where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        #endregion

        #region Private Methods (Close)

        /// <summary>
        /// Invoked when the "Close" <see cref="MenuItem"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="MenuItem"/> where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.timerWindow.Close();
        }

        #endregion
    }
}
