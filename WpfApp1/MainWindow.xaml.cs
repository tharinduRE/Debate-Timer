using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CountdownTimer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int time = 15;

		public MainWindow()
		{
			InitializeComponent();
	
		}

		public void StartCountdowntimer() {
			DispatcherTimer dtClockTime = new DispatcherTimer();

			dtClockTime.Interval = new TimeSpan(0, 0, 1);
			dtClockTime.Tick += new EventHandler(dtClockTime_Tick);

			dtClockTime.Start();
		}

		private void dtClockTime_Tick(object sender, EventArgs e) {

            DispatcherTimer dtClockTime = (DispatcherTimer)sender;
			if (time > 0)
			{
				if (time % 60 <= 10 && time % 60 != 0)
				{
					time--;
					lblClockTime.Content = string.Format("0{0}:0{1}", time / 60, time % 60);
				}
				else
				{
					time--;
					lblClockTime.Content = string.Format("0{0}:{1}", time / 60, time % 60);
				}
			}
			if (time < 10) {
				lblClockTime.Foreground = Brushes.Red;
			}

			if (time == 0) {
				dtClockTime.Stop();
			}

		}

		private void BtnStart_click(object sender, RoutedEventArgs e) {
			StartCountdowntimer();
		} 

		private void Window_keydown(object sender, KeyEventArgs e) {
			if (e.Key == Key.F11) {
				WindowStyle = WindowStyle.None;
				WindowState = WindowState.Maximized;
				ResizeMode = ResizeMode.NoResize;
			}
		}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window1 subWindow = new Window1();
            subWindow.ShowDialog();
        }

        private void MenuItemFullscreen_Click(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
        }
    }
}
