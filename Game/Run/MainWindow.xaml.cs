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

namespace Run
{
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Point location1 = new Point(0, 0);
        Point location2 = new Point(110, 0);
        Point location3 = new Point(220, 0);


        Point playerPoint = new Point(110, 380);

        bool isRendering = false;
        public MainWindow()
        {
            InitializeComponent();
            LoadObstacle();
            LoadPlayer();
            this.KeyDown += new KeyEventHandler(PlayerMove);
            CompositionTarget.Rendering += new EventHandler(Rendering);
            RunBackWorker();
        }

        private void RunBackWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackWork);
            bw.RunWorkerAsync();
        }

        private void LoadObstacle()
        {
            string leftUri = @"Images/star.png";
            string rightUri = @"Images/shose.png";
            string middleUri = @"Images/star.png";

            BitmapImage lbmi = new BitmapImage(new Uri(
                leftUri, UriKind.Relative));
            BitmapImage rbmi = new BitmapImage(new Uri(
                rightUri, UriKind.Relative));
            BitmapImage mbmi = new BitmapImage(new Uri(
                            middleUri, UriKind.Relative));
            if (null != lbmi)
                this.LeftStar.Source = lbmi;
            if (null != rbmi)
                this.RightStar.Source = rbmi;
            if (null != mbmi)
                this.MiddleStar.Source = mbmi;
            Canvas.SetLeft(RightStar, location2.X);
            Canvas.SetLeft(MiddleStar, location3.X);
        }

        private void LoadPlayer()
        {
            string uri = @"Images/player.png";
            BitmapImage bmi = new BitmapImage(new Uri(
                uri, UriKind.Relative));
            if (null != bmi)
            {
                Console.WriteLine(@"shit");
                this.Player.Source = bmi;
                Canvas.SetLeft(Player, playerPoint.X);
                Canvas.SetTop(Player, playerPoint.Y);
            }
             
        }

        private void LeftImgMove()
        {
            Canvas.SetTop(LeftStar, location1.Y);
            Canvas.SetTop(RightStar, location2.Y);
            Canvas.SetTop(MiddleStar, location3.Y);

            location1.Y += 20;
            location2.Y += 10;
            location3.Y += 35;


            if (location1.Y > this.Height)
            {
                location1.Y = 0;
            }
            if (location2.Y > this.Height)
            {
                location2.Y = 0;
            }
            if (location3.Y > this.Height)
            {
                location3.Y = 0;
            }
        }

        

        private void BackWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(100);
                isRendering = true;
            }
        }
        private void Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {
                LeftImgMove();
                isRendering = false;
            }
        }

        //key board control
        private void PlayerMove(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left)
            {
                Console.WriteLine("left");
                if (playerPoint.X > 0)
                {
                    playerPoint.X -= 100;
                    Canvas.SetLeft(Player, playerPoint.X);
                }
            }
            else if (e.Key == Key.Right)
            {
                Console.WriteLine("right");
                if (playerPoint.X < this.Width - 165)
                {
                    playerPoint.X += 100;
                    Canvas.SetLeft(Player, playerPoint.X);
                }
            }
        }
    }
}
