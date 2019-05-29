using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace _2048_UWP
{
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public const int LEFT = 0;
        public const int UP = 1;
        public const int RIGHT = 2;
        public const int DOWN = 3;
        Tile[,] tiles;
        Point start;
        int[,] num;
        int nth = 0;
        public MainPage()
        {
            this.InitializeComponent();
            bigmain.ManipulationStarted += _ManipulationStarted;
            //bigmain.ManipulationDelta += _ManipulationDelta;
            bigmain.ManipulationCompleted += _ManipulationCompleted;
            this.Loaded += MainPage_Loaded;
        }

        private void _ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) { start = e.Position; }
        //private void _ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        //{
        //    Point end = e.Position;
        //    e.Complete();
        //    if (Math.Abs(end.X - start.X) < 5 && Math.Abs(end.Y - start.Y) < 5)
        //    {
        //        return;
        //    }

        //    if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
        //    {
        //        if (end.X - start.X > 0) { Move(RIGHT); }
        //        else { Move(LEFT); }
        //    }
        //    else
        //    {
        //        if (end.Y - start.Y > 0) { Move(DOWN); }
        //        else { Move(UP); }
        //    }
        //}
        private void _ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) { }

        //private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        //{
        //    Debug.WriteLine(e.OriginalKey);

        //    if (e.OriginalKey == Windows.System.VirtualKey.A || e.OriginalKey == Windows.System.VirtualKey.Left || e.OriginalKey == Windows.System.VirtualKey.H)
        //    {
        //        Move(LEFT);
        //    }
        //    if (e.OriginalKey == Windows.System.VirtualKey.W || e.OriginalKey == Windows.System.VirtualKey.Up || e.OriginalKey == Windows.System.VirtualKey.K)
        //    {
        //        Move(UP);
        //    }
        //    if (e.OriginalKey == Windows.System.VirtualKey.D || e.OriginalKey == Windows.System.VirtualKey.Right || e.OriginalKey == Windows.System.VirtualKey.L)
        //    {
        //        Move(RIGHT);
        //    }
        //    if (e.OriginalKey == Windows.System.VirtualKey.S || e.OriginalKey == Windows.System.VirtualKey.Down || e.OriginalKey == Windows.System.VirtualKey.J)
        //    {
        //        Move(DOWN);
        //    }
        //}

        private async void IfGameOver()
        {
            if (isGameOver())
            {
                Debug.WriteLine("Game Over");
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Game Over",
                    Content = "\nCan't merge more number\nYour Score is:" + score.Text + ", Highest Score:" + best.Text + ".",
                    FullSizeDesired = false, 
                    PrimaryButtonText = "New Game",
                    SecondaryButtonText = "Return"
                };
                btn.LostFocus -= Btn_LostFocus;
                try
                {
                    var result = await dialog.ShowAsync();
                    Debug.WriteLine("Result=" + result);
                    if (ContentDialogResult.Primary == result)
                    {
                        NewGame(nth++);
                    }
                    btn.LostFocus += Btn_LostFocus;
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
            }

        }
        private bool isGameOver()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    Debug.WriteLine(i + " " + j);
                    if (tiles[i, j].Number == 0) return false;
                    if (i - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i - 1, j].Number)
                            return false;
                    if (j - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i, j - 1].Number) return false;
                    if (j + 1 < 4)
                        if (tiles[i, j].Number == tiles[i, j + 1].Number) return false;
                    if (i + 1 < 4)
                        if (tiles[i, j].Number == tiles[i + 1, j].Number) return false;
                }
            return true;
        }

        //private void Move(int direction)
        //{
        //    IfGameOver();
        //    Debug.WriteLine(direction);

        //    //Initialize the num intermediate array to all zeros
        //    num = new int[4, 4];
        //    for (int i = 0; i < 4; i++)
        //        for (int j = 0; j < 4; j++)
        //            num[i, j] = 0;

        //    //Clear the grid without numbers
        //    bool hasBlankMove = ClearBlank(direction);
        //    int i_score = int.Parse(score.Text);
        //    bool hasAddMove = AddNumber(direction, ref i_score);
        //    score.Text = i_score.ToString();
        //    if (i_score > int.Parse(best.Text)) { best.Text = i_score.ToString(); }

        //    if (hasAddMove | hasBlankMove)
        //    {
        //        //Generate 1 new number
        //        Random random = new Random();
        //        int a = random.Next(15);
        //        if (a == 0)
        //            a = 4;
        //        else
        //            a = 2;
        //        int x = 0, y = 0;
        //        do
        //        {   //Generate [0,3] Random Number
        //            x = random.Next(4);
        //            y = random.Next(4);
        //        } while (tiles[x, y].Number != 0);
        //        tiles[x, y].Number = a;
        //        //New Digital Animation
        //        tiles[x, y].Appera();
        //    }

        //    //for (int i = 0; i < 4; i++)
        //    //{
        //    //    for (int j = 0; j < 4; j++)
        //    //    {
        //    //        Debug.Write(tiles[i, j].Number + " ");

        //    //        //Update data to be saved
        //    //        App.UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
        //    //    }
        //    //    Debug.WriteLine("");
        //    //}
        //    Debug.WriteLine("score = " + score.Text + " best = " + best.Text);
        //    App.UserData.CurrentInstance.Score = score.Text;
        //    App.UserData.CurrentInstance.Best = best.Text;

        //}
        private bool ClearBlank(int o)
        {
            bool hasBlankMove = false;
            if (o == LEFT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 0;
                    for (int j = 0; j < 4; j++)
                        if (0 != tiles[i, j].Number)
                            num[i, t++] = tiles[i, j].Number;
                }
            }//LEFT

            if (o == RIGHT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 3;
                    for (int j = 3; j >= 0; j--)
                        if (0 != tiles[i, j].Number)
                            num[i, t--] = tiles[i, j].Number;

                }
            }// RIGHT

            if (o == UP)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                        if (0 != tiles[i, j].Number)
                            num[t++, j] = tiles[i, j].Number;
                }
            }// UP

            if (o == DOWN)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 3;
                    for (int i = 3; i >= 0; i--)
                        if (0 != tiles[i, j].Number)
                            num[t--, j] = tiles[i, j].Number;
                }
            }//DOWN

            //Update numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // After remove the blank, it's different from the orginal, indicating that there is movement
                    if (tiles[i, j].Number != num[i, j])
                        hasBlankMove = true;
                    tiles[i, j].Number = num[i, j];
                }
            }

            return hasBlankMove;
        }

        private bool AddNumber(int o, ref int s)
        {
            bool hasAddMove = false;
            if (o == LEFT)
            {
                Debug.WriteLine("←");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (tiles[i, j].Number == tiles[i, j + 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //Enlarged Animation
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j + 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j + 1; t < 3; t++)
                            {
                                tiles[i, t].Number = tiles[i, t + 1].Number;
                            }
                            tiles[i, 3].Number = 0;
                        }
                    }
                }// Each line

            }// LEFT

            if (o == RIGHT)
            {
                Debug.WriteLine("→");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        if (tiles[i, j].Number == tiles[i, j - 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //Enlarged Animation
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j - 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j - 1; t > 0; t--)
                            {
                                tiles[i, t].Number = tiles[i, t - 1].Number;
                            }
                            tiles[i, 0].Number = 0;
                        }
                    }
                }
            }// RIGHT

            if (o == UP)
            {
                Debug.WriteLine("↑");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (tiles[i, j].Number == tiles[i + 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //Enlarged Animation
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i + 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i + 1; t < 3; t++)
                            {
                                tiles[t, j].Number = tiles[t + 1, j].Number;
                            }
                            tiles[3, j].Number = 0;
                        }
                    }
                }
            }// UP

            if (o == DOWN)
            {
                Debug.WriteLine("↓");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 3; i > 0; i--)
                    {
                        if (tiles[i, j].Number == tiles[i - 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //Enlarged Animation
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i - 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i - 1; t > 0; t--)
                            {
                                tiles[t, j].Number = tiles[t - 1, j].Number;
                            }
                            tiles[0, j].Number = 0;
                        }
                    }
                }
            }//DOWN

            return hasAddMove;
        }

        private void MoveAnimate(Tile tile, int direction, int distance)
        {
            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = 0;
            moveTransform.Y = 0;
            tile.RenderTransform = moveTransform;
            Duration duration = new Duration(TimeSpan.FromSeconds(1));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            DoubleAnimation myDoubleAnimationY = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            myDoubleAnimationY.Duration = duration;
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            moveStoryboard.Children.Add(myDoubleAnimationX);
            moveStoryboard.Children.Add(myDoubleAnimationY);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            Storyboard.SetTarget(myDoubleAnimationY, moveTransform);
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            Storyboard.SetTargetProperty(myDoubleAnimationY, "Y");
            switch (direction)
            {
                case LEFT: myDoubleAnimationX.To = -tile.ActualWidth * distance; break;
                case UP: myDoubleAnimationY.To = -tile.ActualHeight * distance; break;
                case RIGHT: myDoubleAnimationX.To = tile.ActualWidth * distance; break;
                case DOWN: myDoubleAnimationY.To = tile.ActualHeight * distance; break;
            }
            //Canvas.ZIndex Attached Property
            //https://msdn.microsoft.com/zh-cn/library/windows/apps/windows.ui.xaml.controls.canvas.zindex.aspx
            tile.SetValue(Canvas.ZIndexProperty, 999);
            //Start Animation
            moveStoryboard.Begin();
        }

        private void NewGame(int times)
        {
            //Open game for the first time
            //if (times == 0)
            //{
            //    //If there is last game data
            //    score.Text = App.UserData.CurrentInstance.Score;
            //    best.Text = App.UserData.CurrentInstance.Best;
            //    bool all0 = true;
            //    for (int i = 0; i < 4; i++)
            //        for (int j = 0; j < 4; j++)
            //        {
            //            tiles[i, j].Number = App.UserData.CurrentInstance.Num[i, j];
            //            if (tiles[i, j].Number != 0) all0 = false;
            //        }
            //    if (all0) Init();
            //}
            ////Click on new game
            //else
                Init();
        }

        private void Init()
        {
            //Reset to Zero
            foreach (Tile t in tiles)
                t.Number = 0;
            Random random = new Random();
            //One is 2 and the other is 2 (90%), 4 is (10%)
            int a = 4;
            if (random.Next(0, 10) != 0) a = 2;
            int x1 = random.Next(0, 4),
                y1 = random.Next(0, 4);
            int x2, y2;
            do
            {
                x2 = random.Next(0, 4);
                y2 = random.Next(0, 4);
            } while (x1 == x2 && y1 == y2);
            tiles[x1, y1].Number = 2;
            tiles[x2, y2].Number = a;
            tiles[x1, y1].Appera();
            tiles[x2, y2].Appera();
            score.Text = "0";
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            #region Size
            //public Rect VisibleBounds { get; }
            Size size = new Size(Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width,
                                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Height);
            #endregion
            wrap.Width = size.Width;
            wrap.Height = size.Height;
            double up = 0.25, down = 0.75, left = 0.4, right = 0.6, logomargin = 10;

            if (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Orientation == Windows.UI.ViewManagement.ApplicationViewOrientation.Portrait)
            {
                //Vetical Screen
                //Arrange the head and main area vertically
                wrap.Orientation = Orientation.Vertical;
                //Horizontally arranging header content
                header.Orientation = Orientation.Horizontal;

                header.Height = size.Height * up;
                footer.Height = size.Height * down;
                header.Width = size.Width;
                footer.Width = size.Width;

                logo.Width = size.Width * left - logomargin;
                logo.Height = size.Height * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Width = size.Width * right;
                headerRight.Height = size.Height * up;

                bigmain.Width = (size.Width > size.Height * down) ? (size.Height * down) : (size.Width);
                bigmain.Height = (size.Width > size.Height * down) ? (size.Height * down) : (size.Width);
                bigmain.Margin = new Thickness((size.Width - bigmain.Width) / 2,
                    (size.Height * down - bigmain.Height) / 2,
                    (size.Width - bigmain.Width) / 2,
                    (size.Height * down - bigmain.Height) / 2);
            }
            else
            {
                //Horizontal Screen
                wrap.Orientation = Orientation.Horizontal;
                header.Orientation = Orientation.Vertical;

                header.Width = size.Width * up;
                footer.Width = size.Width * down;
                header.Height = size.Height;
                footer.Height = size.Height;

                logo.Height = size.Height * left - logomargin;
                logo.Width = size.Width * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Height = size.Height * right;
                headerRight.Width = size.Width * up;

                bigmain.Height = (size.Height > size.Width * down) ? (size.Width * down) : (size.Height);
                bigmain.Width = (size.Height > size.Width * down) ? (size.Width * down) : (size.Height);
                bigmain.Margin = new Thickness((size.Width * down - bigmain.Width) / 2,
                    (size.Height - bigmain.Height) / 2,
                    (size.Width * down - bigmain.Width) / 2,
                    (size.Height - bigmain.Height) / 2);
            }
            headergrid.Width = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Height = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Margin = new Thickness((headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2,
                (headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2);
            //if (tiles != null)
               // foreach (Tile tile in tiles) { tile.SetCenterXY(b00.ActualWidth / 2, b00.ActualHeight / 2); }
        }

        private void menugrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            menutxt.Foreground = new SolidColorBrush(Colors.White);

            //设置鼠标手型
            //http://www.cnblogs.com/webabcd/archive/2013/01/10/2853974.html
            Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
            menugrid.PointerExited += (o, e1) =>
            {
                try { menutxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { menutxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }
        private void newgrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            newtxt.Foreground = new SolidColorBrush(Colors.White);
            CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
            newgrid.PointerExited += (o, e1) =>
            {
                try { newtxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { newtxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }

        private void menugrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //UWP System level page navigation
            //http://www.cnblogs.com/Zixx/archive/2015/10/11/4868937.html
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Menu));
        }

        private void newgrid_Tapped(object sender, TappedRoutedEventArgs e) { NewGame(nth++); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //Make the keyboard focus 
            btn.Focus(FocusState.Programmatic);
            btn.LostFocus += Btn_LostFocus;
        }

        private void Btn_LostFocus(object sender, RoutedEventArgs e)
        {
            btn.Focus(FocusState.Programmatic);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            tiles = new Tile[4, 4];
            tiles[0, 0] = b00;
            tiles[0, 1] = b01;
            tiles[0, 2] = b02;
            tiles[0, 3] = b03;
            tiles[1, 0] = b10;
            tiles[1, 1] = b11;
            tiles[1, 2] = b12;
            tiles[1, 3] = b13;
            tiles[2, 0] = b20;
            tiles[2, 1] = b21;
            tiles[2, 2] = b22;
            tiles[2, 3] = b23;
            tiles[3, 0] = b30;
            tiles[3, 1] = b31;
            tiles[3, 2] = b32;
            tiles[3, 3] = b33;
            NewGame(nth++);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    App.UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
            base.OnNavigatedFrom(e);
        }
    }

}