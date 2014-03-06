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
    using System.Media;

    public enum GameStatus
    {
        STA_NULL,
        STA_START,
        STA_OVER
    };

    public enum ScoreStatus
    {
        SCO_CATCH,
        SCO_LOSE,
        SCO_NULL
    }

    public partial class MainWindow : Window
    {
        static GameStatus Running = GameStatus.STA_NULL;

        bool isRendering = false;
        bool isBackGestureActive = false;
        bool isForwardGestureActive = false;
        bool isCenterGestureActive = false;
        bool isObliqueRight = false;
        bool isObliqueLeft = false;
        bool timerImgZomIn = false;

        int timerImgCount = 3;

        static int MIN_BALL_COUNT = 1;
        static int MAX_BALL_COUNT = 2; // easy mode

        // Replace by int queue
        static Queue<Football> enqueBalls = new Queue<Football>();


        List<Football> balls = new List<Football>();
        static Random rand = new Random();
        static int generateClock = 0;
        static int startGameCount = 0;

        public static ScoreStatus playerStatus = ScoreStatus.SCO_NULL;
        public static bool netStatus = false;
        static BitmapImage net = new BitmapImage(new Uri(@"Images/net.png", UriKind.Relative));
        static BitmapImage plumpNet = new BitmapImage(new Uri(@"Images/plumpnet.png", UriKind.Relative));
        static BitmapImage catchBall = new BitmapImage(new Uri(@"Images/get.png", UriKind.Relative));
        static BitmapImage loseBall = new BitmapImage(new Uri(@"Images/miss.png", UriKind.Relative));
        static BitmapImage playerLose = new BitmapImage(new Uri(@"Images/player.png", UriKind.Relative));
        static BitmapImage playerCatch = new BitmapImage(new Uri(@"Images/player2.png", UriKind.Relative));
        


        public static int playerAngle = 2; // middle direction
        public static int sleepTime = 100;

        KinectSensor kinect;
        Skeleton[] skeletonData;

        Timer countDownTimer;
        public MainWindow()
        {
            InitializeComponent();

            // Initialization
            LoadPlayerImage();
            InitBalls();

            // Count down befoe start game
            countDownTimer = new Timer(
    new TimerCallback(countDownTimerDelegate), null, 30, 90);

            // Background worker
            CompositionTarget.Rendering += new EventHandler(Rendering);

            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            this.KeyDown += new KeyEventHandler(this.controlPlayerAngle);
        }

        private void countDownTimerDelegate(object sender)
        {
            this.Dispatcher.BeginInvoke(
             (Action)delegate()
             {
                 ChangeTimerImage();
             });
        }

        private void ChangeTimerImage()
        {
            if (Running == GameStatus.STA_START)
            {
                if (TimerImage.Opacity > 1.0)
                {
                    timerImgZomIn = false;
                    TimerImage.Opacity = 1.0;
                }
                if (TimerImage.Opacity <= 0.1)
                {
                    timerImgZomIn = true;
                    TimerImage.Opacity = 0.1;
                    LoadTimerImage();
                }

                if (timerImgZomIn)
                {
                    TimerImage.Opacity += 0.1;
                }
                else
                {
                    TimerImage.Opacity -= 0.1;
                }
            }
        }

        private void LoadTimerImage()
        {
            if (timerImgCount > 0)
            {
                this.TimerImage.Source = new BitmapImage(
                    new Uri(@"Images/timer" + timerImgCount + ".png", UriKind.Relative));
                timerImgCount -= 1;
            }
            else
            {
                this.TimerImage.Source = null;
                countDownTimer.Dispose();
            }
            
        }

        

        private void Rendering(object sender, EventArgs e)
        {
            if (Running == GameStatus.STA_START && isRendering)
            {
                isRendering = false;

                // Timer : wait 3 seconds to start game
                if (startGameCount <= 30)
                {
                    startGameCount += 1;
                }
                else
                {
                    // Calculate Generating Time
                    generateClock += 1;
                    if (generateClock == 12)
                    {
                        generateClock = 0;
                        GenerateBalls();
                    }

                    CheckNetStatus();
                    CheckPlayerStatus();

                    // Move Balls
                    CreateBallMoveAction();

                    this.ScoreText.Text =
                        GameData.getScore.ToString() + " / " + GameData.totalCount.ToString();
             
                    // if easy mode
                    DequeueBalls();
                    
                    
                }

            }
        }


        private void BackWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {                
                Thread.Sleep(sleepTime);
                isRendering = true;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect = (from sensor in KinectSensor.KinectSensors
                      where sensor.Status == KinectStatus.Connected
                      select sensor
                      ).FirstOrDefault();
            if (null != kinect)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady +=
                    new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                kinect.Start();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (kinect != null)
            {
                kinect.Stop();
            }
        }

        private void SkeletonFrame_Ready(object sender,
            SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    

                    // playing game
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
            // Recognize Joints
            Joint leftHand =  (from j in s.Joints
                               where j.JointType == JointType.HandLeft
                               select j).FirstOrDefault();
            Joint rightHand = (from j in s.Joints
                               where j.JointType == JointType.HandRight
                               select j).FirstOrDefault();
            Joint head =      (from j in s.Joints
                               where j.JointType == JointType.Head
                               select j).FirstOrDefault();
            Joint hipCenter = (from j in s.Joints
                               where j.JointType == JointType.HipCenter
                               select j).FirstOrDefault();

            // Drawing Point
            SetEllipsePosition(ellipseHead, head, isObliqueRight);
            SetEllipsePosition(ellipseLeftHand, leftHand, isBackGestureActive);
            SetEllipsePosition(ellipseRightHand, rightHand, isForwardGestureActive);
            SetEllipsePosition(ellipseHipCenter, hipCenter, isCenterGestureActive);

            leftHand.ScaleTo(400, 200);
            rightHand.ScaleTo(400, 200);
            head.ScaleTo(400, 200);
            hipCenter.ScaleTo(400, 200);

            // end game manully

            if (GameStatus.STA_OVER == Running)
            {
                System.Environment.Exit(0);
            }


            /* Body Control */
            // Start game
            if (
                   Math.Abs(rightHand.Position.Y - leftHand.Position.Y) < 0.3
                && Math.Abs(rightHand.Position.Z - leftHand.Position.Z) < 0.3
                && rightHand.Position.X - leftHand.Position.X > 1.0

                )
            {
                Running = GameStatus.STA_START;   
            }

            // End game
            if (
                   (rightHand.Position.Y < head.Position.Y)
                && (rightHand.Position.Y > head.Position.Y - 0.2)
                && (leftHand.Position.Y < head.Position.Y)
                && (leftHand.Position.Y > head.Position.Y - 0.2)

                && (rightHand.Position.Z < head.Position.Z)
                && (rightHand.Position.Z > head.Position.Z - 0.2)
                && (leftHand.Position.Z < head.Position.Z)
                && (leftHand.Position.Z > head.Position.Z - 0.2)

                )
            {
                Running = GameStatus.STA_OVER;
            }

            // most right and left
            if (
                (rightHand.Position.X - hipCenter.Position.X > 0.35)
                && Math.Abs(leftHand.Position.X - rightHand.Position.X) < 1
                && Math.Abs(leftHand.Position.Y - rightHand.Position.Y) < 1
                && Math.Abs(leftHand.Position.Z - rightHand.Position.Z) < 0.5
                && (rightHand.Position.Y < head.Position.Y)
                && ((head.Position.X - hipCenter.Position.X) > 0.1)
                && (head.Position.Y > rightHand.Position.Y)
                )
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

            if (
                 (leftHand.Position.X - hipCenter.Position.X < -0.35)
                && Math.Abs(leftHand.Position.X - rightHand.Position.X) < 1
                && Math.Abs(leftHand.Position.Y - rightHand.Position.Y) < 1
                && Math.Abs(leftHand.Position.Z - rightHand.Position.Z) < 0.5
                && (leftHand.Position.Y < head.Position.Y)
                && (head.Position.X - hipCenter.Position.X < -0.1)
                && (head.Position.Y > leftHand.Position.Y)
                )
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

            // Center direction
            if (
                (leftHand.Position.X > head.Position.X - 0.4) 
                && (leftHand.Position.X < head.Position.X)
                && (rightHand.Position.X < head.Position.X + 0.4)
                && (rightHand.Position.X > head.Position.X)
                && (rightHand.Position.X > head.Position.X + 0.15)
                && (leftHand.Position.X < head.Position.X - 0.15)
                )
            {
                if (!isCenterGestureActive)
                {
                    isCenterGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{D}");
                }
            }
            else
            {
                isCenterGestureActive = false;
            }

            // Oblique direction
            // oblique right
            if ((Math.Abs(leftHand.Position.X - rightHand.Position.X) < 1)
                && Math.Abs(leftHand.Position.Y - rightHand.Position.Y) < 1
                && Math.Abs(leftHand.Position.Z - rightHand.Position.Z) < 0.5
                && (leftHand.Position.Y > head.Position.Y)
                && (rightHand.Position.Y > head.Position.Y)
                && ((head.Position.X - hipCenter.Position.X) > 0.1)
                )
            {
                if (!isObliqueLeft && !isObliqueRight)
                {
                    isObliqueRight = false;
                    System.Windows.Forms.SendKeys.SendWait("{F}");
                }
            }

            // oblique left
            if ((Math.Abs(leftHand.Position.X - rightHand.Position.X) < 1)
                && Math.Abs(leftHand.Position.Y - rightHand.Position.Y) < 1
                && Math.Abs(leftHand.Position.Z - rightHand.Position.Z) < 0.5
                && (leftHand.Position.Y > head.Position.Y)
                && (rightHand.Position.Y > head.Position.Y)
                && (hipCenter.Position.X - head.Position.X > 0.1)
                && (hipCenter.Position.X - head.Position.X < 0.3)
                )
            {
                if (!isObliqueLeft && !isObliqueRight)
                {
                    isObliqueLeft = false;
                    System.Windows.Forms.SendKeys.SendWait("{S}");
                }
            }

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

            // set velocities
            for (int i = 0; i < 5; i++)
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
            this.Player.Source = playerLose;
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




        // Keyboard control
        private void controlPlayerAngle(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.A:
                    rotatePlayer.Angle = GameData.Angles[0].Item1;
                    playerAngle = 0;
                    break;
                case Key.S:
                    rotatePlayer.Angle = GameData.Angles[1].Item1;
                    playerAngle = 1;
                    break;
                case Key.D:
                    rotatePlayer.Angle = GameData.Angles[2].Item1;
                    playerAngle = 2;
                    break;
                case Key.F:
                    rotatePlayer.Angle = GameData.Angles[3].Item1;
                    playerAngle = 3;
                    break;
                case Key.G:
                    rotatePlayer.Angle = GameData.Angles[4].Item1;
                    playerAngle = 4;
                    break;
                default:
                    break;
            }
        }
    
        private void GenerateBalls()
        {
            
            // create some new balls
            int generateCount = rand.Next(MIN_BALL_COUNT, MAX_BALL_COUNT);
            GameData.totalCount += generateCount;

            for (int i = 0; i < generateCount; i++)
            {
                Football theBall = new Football();
                int EId = rand.Next(0, 5);
                theBall.eId = EId;
                theBall.isClosed = false;
                balls[EId].eId = EId;
                balls[EId].img.Source = CreateBallImg();
                theBall.state = BallState.EQUE;
                theBall.xV = GameData.velocities[EId].X;
                theBall.yV = GameData.velocities[EId].Y;

                enqueBalls.Enqueue(theBall);
            }
        }

        private void DequeueBalls()
        {
            if (enqueBalls.Count > 0)
            {
                Football exitBall = enqueBalls.Dequeue();
                balls[exitBall.eId].state = BallState.DQUE;
            }
        }

        private void CheckNetStatus()
        {
            if (netStatus == false)
            {
                Net.Source = net;
            }
            else
            {
                Net.Source = plumpNet;
            }
        }

        private void CheckPlayerStatus()
        {
            if (playerStatus == ScoreStatus.SCO_NULL)
            {
                CatchStatus.Source = null;
            }
            else if (playerStatus == ScoreStatus.SCO_CATCH)
            {
                Player.Source = playerCatch;
                CatchStatus.Source = catchBall;
            }
            else
            {
                Player.Source = playerLose;
                CatchStatus.Source = loseBall;
            }
        }
    }
}
