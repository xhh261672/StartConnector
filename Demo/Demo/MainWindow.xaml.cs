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
    using System.IO;
    
    // Reference
    using Microsoft.Kinect;
    using Coding4Fun.Kinect.Wpf;

    public partial class MainWindow : Window
    {
        // class members
        bool isRendering = false;
        bool isBackGestureActive = false;
        bool isForwardGestureActive = false;
        static Queue<Thread> threadList = new Queue<Thread>();
        // Replace by int queue
        static Queue<Football> enqueBalls = new Queue<Football>();

        List<Football> balls = new List<Football>();
        static Random rand = new Random();
        static int generateClock = 0;


        KinectSensor kinect;
        Skeleton[] skeletonData;


        public MainWindow()
        {
            InitializeComponent();

            // Initialization
            LoadPlayerImage();
            InitBalls();

            // Background worker
            CompositionTarget.Rendering += new EventHandler(Rendering);
            //CompositionTarget.Rendering += new EventHandler(sum);

            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            this.KeyDown += new KeyEventHandler(this.controlAngle);
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect = (from sensor in KinectSensor.KinectSensors
                      where sensor.Status == KinectStatus.Connected
                      select sensor).FirstOrDefault();
            if (null != kinect)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady +=
                    new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                kinect.Start();
            }
            else
            {
                Console.WriteLine("Kinect Not Detected");
                System.Environment.Exit(0);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            kinect.Stop();
        }

        private void SkeletonFrame_Ready(object sender,
            SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    skeletonData =
                        new Skeleton[
                            kinect.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                    Skeleton skeleton =
                        (from s in skeletonData
                         where s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();
                    if (null != skeleton)
                    {
                        canvas.Visibility = Visibility.Visible;
                        ProcessGesture(skeleton);
                    }
                }
            }
        }

        private void ProcessGesture(Skeleton s)
        {
            Joint leftHand = (from j in s.Joints
                              where j.JointType == JointType.HandLeft
                              select j).FirstOrDefault();
            Joint rightHand = (from j in s.Joints
                               where j.JointType == JointType.HandRight
                               select j).FirstOrDefault();
            Joint head = (from j in s.Joints
                          where j.JointType == JointType.Head
                          select j).FirstOrDefault();


            /*Control */
            var tempPosition = rightHand.ScaleTo(400, 200);
            //Console.WriteLine("X: {0}", tempPosition.Position.X);
            //Console.WriteLine("Y: {0}", tempPosition.Position.Y);
            if (rightHand.Position.X > head.Position.X + 0.45)
            {

                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isForwardGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{G}");
                }
            }
            else
            {
                isForwardGestureActive = false;
            }

            if (leftHand.Position.X < head.Position.X - 0.45)
            {
                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isBackGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{A}");
                }
            }
            else
            {
                isBackGestureActive = false;
            }

            SetEllipsePosition(ellipseHead, head, false);
            SetEllipsePosition(ellipseLeftHand, leftHand, isBackGestureActive);
            SetEllipsePosition(ellipseRightHand, rightHand, isForwardGestureActive);


        }

        private void SetEllipsePosition(Ellipse ellipse,
            Joint joint, bool isHighlighted)
        {
            joint.ScaleTo(400, 200);

            ColorImagePoint colorImagePoint =
                kinect.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position,
                ColorImageFormat.RgbResolution640x480Fps30);
            if (isHighlighted)
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = Brushes.Red;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = Brushes.Blue;
            }
            Canvas.SetLeft(ellipse, colorImagePoint.X - ellipse.ActualWidth / 2);
            Canvas.SetTop(ellipse, colorImagePoint.Y - ellipse.ActualHeight / 2);
        }

        private void InitBalls()
        {
            for (int i = 0; i < 10; i++)
                balls.Add(new Football());

            // set images
            balls[0].img = this.Football0;
            balls[1].img = this.Football1;
            balls[2].img = this.Football2;
            balls[3].img = this.Football3;
            balls[4].img = this.Football4;
            balls[5].img = this.Football5;
            balls[6].img = this.Football6;
            balls[7].img = this.Football7;
            balls[8].img = this.Football8;
            balls[9].img = this.Football9;

            // set velocities
            for (int i = 0; i < 10; i++)
            {
                balls[i].xV = GameData.velocities[i].X;
                balls[i].yV = GameData.velocities[i].Y;
            }
        }


        

        private void CreateBallMoveAction()
        {
            var dequeBalls =
                from ball in balls
                where (ball.img != null && ball.state == BallState.DQUE)
                select ball;
            foreach (Football ball in dequeBalls) 
            {
                ball.MoveBall();
            }
        }
               

        private void LoadPlayerImage()
        {
            string playerImgUrl = @"Images/player.png";
            this.Player.Source = new BitmapImage(new Uri(playerImgUrl, UriKind.Relative));
        }

        private BitmapImage CreateBallImg()
        {
            return new BitmapImage(new Uri(@"Images/football.png", UriKind.Relative));
        }


        private void RunBackWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackWork);
            bw.RunWorkerAsync();
        }

        delegate void GenerateBallsDelegate();
        private void GenerateBallThread()
        {
            //Console.WriteLine("Generate Balls");
            GenerateBallsDelegate gbd = new GenerateBallsDelegate(GenerateBalls);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, gbd);
            Thread.Sleep(9000);
        }

        private void CheckThreadCount()
        {
            if (threadList.Count > 3)
            {
                Thread thisThread = threadList.Dequeue();
                if (thisThread.IsAlive)
                {
                    thisThread.Abort();
                }
            }
            else
            {
                Thread t = new Thread(GenerateBallThread);
                t.Start();
                threadList.Enqueue(t);
            }
        }
        private void Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {
                isRendering = false;
                
                //
                ++generateClock;
                if (generateClock == 4)
                {   
                    generateClock = 0;
                    GenerateBalls();
                }
                //CheckThreadCount();

                CreateBallMoveAction();              
                this.ScoreText.Text = 
                    GameData.getScore.ToString() + " / " + GameData.totalCount.ToString();
                while (enqueBalls.Count > 0)
                {
                    DeueBalls();
                }
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


        // Keyboard control
        private void controlAngle(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.A:
                    rotatePlayer.Angle = 287.74;
                    // this.Player.Source = null;
                    break;
                case Key.S:
                    rotatePlayer.Angle = 311.08;
                    break;
                case Key.D:
                    rotatePlayer.Angle = 0;
                    break;
                case Key.F:
                    rotatePlayer.Angle = 48.92;
                    break;
                case Key.G:
                    rotatePlayer.Angle = 72.26;
                    break;
                default:
                    break;
            }
        }
    
        private void GenerateBalls()
        {
            
                // create some new balls
            int generateCount = rand.Next(1, 3);
            GameData.totalCount += generateCount;

            for (int i = 0; i < generateCount; i++)
            {
                Football theBall = new Football();
                int EId = rand.Next(0, 5);
                theBall.eId = EId;
                Console.WriteLine(EId);
                //balls[theBall.eId] =
                //    (from ball in balls
                //    where ball.img == null && ball != null
                //    select ball).First();
                balls[EId].eId = EId;
                balls[EId].img.Source = CreateBallImg();
                theBall.state = BallState.EQUE;
                theBall.xV = GameData.velocities[EId].X;
                theBall.yV = GameData.velocities[EId].Y;

                enqueBalls.Enqueue(theBall);
            }
            //}
        }

        private void DeueBalls()
        {
            if (enqueBalls.Count > 0)
            {
                Football exitBall = enqueBalls.Dequeue();
                balls[exitBall.eId].state = BallState.DQUE;
            }
        }
    }
}
