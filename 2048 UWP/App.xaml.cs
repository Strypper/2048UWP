using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace _2048_UWP
{
    sealed partial class App : Application
    {
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            //http://blogs.u2u.be/diederik/post/2015/07/28/A-lap-around-Adaptive-Triggers.aspx
            // Override default minimum size.
            var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            view.SetPreferredMinSize(new Size { Width = 320, Height = 480 });

            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                //http://www.cnblogs.com/h82258652/p/4996087.html
                rootFrame.Navigated += delegate
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                    = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                };
                SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) =>
                {
                    if (rootFrame.CanGoBack)
                    {
                        args.Handled = true;
                        rootFrame.GoBack();
                    }
                };
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
                {
                    UserData.Load();
                }
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s1, e1) =>
                {
                    if (rootFrame != null)
                    {
                        if (rootFrame.CanGoBack)
                        {
                            e1.Handled = true;
                            rootFrame.GoBack();
                        }
                    }
                };
            }
            Window.Current.Activate();
        }
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            UserData.Save();
            deferral.Complete();
        }

        public class UserData
        {
            private static UserData _userData = null;
            static UserData() { _userData = new UserData(); }
            public static UserData CurrentInstance { get { return _userData; } }

            string score = "0", best = "0"; int[,] num = new int[4, 4];
            public string Score { get { return score; } set { score = value; } }
            public string Best { get { return best; } set { best = value; } }
            public int[,] Num { get { return num; } set { num = value; } }

            byte[,] accent = new byte[2, 4];
            public byte[,] AccentAndBg { get { return accent; } set { accent = value; } }
            public int Nth { get; set; }

            public static void Load()
            {
                Debug.WriteLine("Loading Data...");
                var rs = ApplicationData.Current.RoamingSettings;
                object v = null;
                if (rs.Values.TryGetValue("score", out v)) { CurrentInstance.Score = (string)v; } else { CurrentInstance.Score = "0"; }
                if (rs.Values.TryGetValue("best", out v)) { CurrentInstance.Best = (string)v; } else { CurrentInstance.Best = "0"; }
                if (rs.Values.TryGetValue("nth", out v)) { CurrentInstance.Nth = (int)v; } else { CurrentInstance.Nth = 0; }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (rs.Values.TryGetValue(i + "" + j, out v)) { CurrentInstance.Num[i, j] = (int)v; }
                        else { CurrentInstance.Num[i, j] = 0; }
                        if (i < 2)
                        {
                            if (rs.Values.TryGetValue(i + "a" + j, out v)) { CurrentInstance.AccentAndBg[i, j] = (byte)v; }

                        }
                    }
                }//for

              (Current.Resources["accent"] as SolidColorBrush).Color = Color.FromArgb(
                    CurrentInstance.AccentAndBg[0, 0],
                    CurrentInstance.AccentAndBg[0, 1],
                    CurrentInstance.AccentAndBg[0, 2],
                    CurrentInstance.AccentAndBg[0, 3]);
                (Current.Resources["bg"] as SolidColorBrush).Color = Color.FromArgb(
                    CurrentInstance.AccentAndBg[1, 0],
                    CurrentInstance.AccentAndBg[1, 1],
                    CurrentInstance.AccentAndBg[1, 2],
                    CurrentInstance.AccentAndBg[1, 3]);

                if (!rs.Values.TryGetValue("0a0", out v))
                {
                    (Current.Resources["accent"] as SolidColorBrush).Color = (Color)Current.Resources["SystemAccentColor"];
                    (Current.Resources["accent"] as SolidColorBrush).Color = Colors.Gray;
                }

                byte txtr = 0, txtg = 0, txtb = 0;
                if (rs.Values.TryGetValue("txtr", out v)) { txtr = (byte)v; }
                if (rs.Values.TryGetValue("txtg", out v)) { txtg = (byte)v; }
                if (rs.Values.TryGetValue("txtb", out v)) { txtb = (byte)v; }
                (Current.Resources["txt"] as SolidColorBrush).Color = Color.FromArgb(255, txtr, txtg, txtb);

            }//load

            public static void Save()
            {
                Debug.WriteLine("Saving Data");
                var rs = ApplicationData.Current.RoamingSettings;
                rs.Values["score"] = CurrentInstance.Score;
                rs.Values["best"] = CurrentInstance.best;
                CurrentInstance.AccentAndBg[0, 0] = (Current.Resources["accent"] as SolidColorBrush).Color.A;
                CurrentInstance.AccentAndBg[0, 1] = (Current.Resources["accent"] as SolidColorBrush).Color.R;
                CurrentInstance.AccentAndBg[0, 2] = (Current.Resources["accent"] as SolidColorBrush).Color.G;
                CurrentInstance.AccentAndBg[0, 3] = (Current.Resources["accent"] as SolidColorBrush).Color.B;
                CurrentInstance.AccentAndBg[1, 0] = (Current.Resources["bg"] as SolidColorBrush).Color.A;
                CurrentInstance.AccentAndBg[1, 1] = (Current.Resources["bg"] as SolidColorBrush).Color.R;
                CurrentInstance.AccentAndBg[1, 2] = (Current.Resources["bg"] as SolidColorBrush).Color.G;
                CurrentInstance.AccentAndBg[1, 3] = (Current.Resources["bg"] as SolidColorBrush).Color.B;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        rs.Values[i + "" + j] = CurrentInstance.Num[i, j];
                        if (i < 2)
                        {
                            rs.Values[i + "a" + j] = CurrentInstance.AccentAndBg[i, j];
                        }
                    }
                }//for
                rs.Values["txtr"] = (Current.Resources["txt"] as SolidColorBrush).Color.R;
                rs.Values["txtg"] = (Current.Resources["txt"] as SolidColorBrush).Color.G;
                rs.Values["txtb"] = (Current.Resources["txt"] as SolidColorBrush).Color.B;
                rs.Values["nth"] = CurrentInstance.Nth;
            }//save
        }//class
    }

}//namespace
