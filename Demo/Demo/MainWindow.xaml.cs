using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    using System.Diagnostics;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    /// <summary>
    /// MainWindow.xaml logic
    /// </summary>
    

    public partial class MainWindow : Window
    {
        // class members
        bool isRendering = false;

        // This ListQueue can be removed
        List<Queue<Football>> balls = new List<Queue<Football>>();
        
        // Replace by int queue
        Queue<int> ballsGid = new Queue<int>(5);
        
        List<Football> queBalls = new List<Football>();
        static public Point[] startPoint = new Point[5]; 
        static Random rand = new Random();
        static int totalCount = 0;
        // static int getScore = 0;

        Football ball3 = new Football();

        // five ball move threads
        Queue<Thread> ballMoveThread;




        public MainWindow()
        {
            InitializeComponent();

            LoadNetImage();
            LoadPlayerImage();

            CompositionTarget.Rendering += new EventHandler(Rendering);
            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            this.KeyDown += this.controlAngle;


            this.Football3.Source = CreateNewball();
            ball3.img = this.Football3;
            
        }

        //public class Params
        //{
        //    double xv;
        //    double yv;
        //    int e;

        //    public Params(double _xv, double _yv, int _e)
        //    {
        //        xv = _xv;
        //        yv = _yv;
        //        e = _e;
        //    }
        //}
        private void CreateBallMoveThread()
        {
            ballMoveThread = new Queue<Thread>();
            //Params par = new Params(30, 18, 2);
            ThreadStart starter = delegate { this.BallMoveThread(-30, 18, 3); };
            Thread t = new Thread(new ThreadStart(starter));
            t.Start();
            //ballMoveThread.Enqueue()

        }

        delegate void BallMoveDelegate(double xVelocity, double yVelocity, int exit);

        private void BallMoveThread(double xV, double yV, int e)
        {
            BallMoveDelegate bmd = new BallMoveDelegate(ball3.MoveBall);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, bmd, xV, yV, e);
        }

        // Load basic pictures
        private void LoadNetImage()
        {
            string netImgUrl = @"Images/net.png";
            this.Net.Source = new BitmapImage(new Uri(netImgUrl, UriKind.Relative));
        }

        private void LoadPlayerImage()
        {
            string playerImgUrl = @"Images/player.png";
            this.Player.Source = new BitmapImage(new Uri(playerImgUrl, UriKind.Relative));
        }

        private BitmapImage CreateNewball()
        {
            return new BitmapImage(new Uri(@"Images/football.png", UriKind.Relative));
        }

        // set the start points
        private void SetStartPoints()
        {
            startPoint[0] = new Point(10, 270);
            startPoint[1] = new Point(30, 30);
            startPoint[2] = new Point(470, 10);
            startPoint[3] = new Point(910, 30);
            startPoint[4] = new Point(910, 270);
        }


        private void Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {
                Console.WriteLine("rendering");
                isRendering = false;

                CreateBallMoveThread();

                //ball1.MoveBall(30, 17, 1);
                
            }
        }

        private void RunBackWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackWork);
            bw.RunWorkerAsync();
        }

        private void BackWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(100);
                isRendering = true;
            }
        }


        // Keyboard control
        private void controlAngle(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.A:
                    rotatePlayer.Angle = 275.7;
                    // this.Player.Source = null;
                    break;
                case Key.S:
                    rotatePlayer.Angle = 299.25;
                    break;
                case Key.D:
                    rotatePlayer.Angle = 0;
                    break;
                case Key.F:
                    rotatePlayer.Angle = 60.75;
                    break;
                case Key.G:
                    rotatePlayer.Angle = 84.30;
                    break;
                default:
                    break;
            }
        }
    

        private void ConnectImagesAndBalls()
        {
            
        }

        private void InitBallsList()
        {
            for (int i = 0; i < 5; i++)
                balls.Add(new Queue<Football>());
        }

        private void GenerateBalls()
        {
            // create some new balls
            int newBallsCount = rand.Next(2, 4);
            totalCount += newBallsCount;

            // find the null image to assign new image source
            //for (int i = 0; i < queBalls.Count; i++)
            foreach (Football ball in queBalls)
            {
                if (null == ball.img.Source)
                {
                    ball.img.Source = CreateNewball();

                    // enqueue new balls
                    ball.state = BallState.EQUE;
                    int randId = rand.Next(0, 5);
                    balls[randId].Enqueue(ball);
                }
            }
            // set position and move ball            
            // ...
        }


    
    }

}
