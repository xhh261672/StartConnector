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

namespace StartConnector
{
    using System.Media;
    using BallTrackPath;
    using System.Diagnostics;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    // Reference
    using Microsoft.Kinect;



    public partial class GameWindow : Window
    {
        static GameStatus Running = GameStatus.STA_NULL;

        //test rendering
        bool isRendering = false;
        bool isBackGestureActive = false;
        bool isForwardGestureActive = false;
        bool isCenterGestureActive = false;
        bool isObliqueRight = false;
        bool isObliqueLeft = false;
        bool timerImgZomIn = false;
        bool firstStart = false;

        int timerImgCount = 3;
        Queue<FlyingBall> enqueBalls = new Queue<FlyingBall>();
        List<FlyingBall> balls = new List<FlyingBall>();
        static Random rand = new Random();
        static int generateClock = 0;
        static int startGameCount = 31;

        public static ScoreStatus playerStatus = ScoreStatus.SCO_NULL;
        public static bool netStatus = false;
        static BitmapImage net = new BitmapImage(new Uri(@"Images/net.png", UriKind.Relative));
        static BitmapImage plumpNet = new BitmapImage(new Uri(@"Images/plumpnet.png", UriKind.Relative));
        static BitmapImage catchBall = new BitmapImage(new Uri(@"Images/get.png", UriKind.Relative));
        static BitmapImage loseBall = new BitmapImage(new Uri(@"Images/miss.png", UriKind.Relative));
        static BitmapImage playerLose = new BitmapImage(new Uri(@"Images/player.png", UriKind.Relative));
        static BitmapImage playerCatch = new BitmapImage(new Uri(@"Images/player2.png", UriKind.Relative));
        static BitmapImage gameOverImg = new BitmapImage(new Uri(@"Images/gameover.png", UriKind.Relative));


        public static int playerAngle = 2; // middle direction
        public static int sleepTime = 100;

        KinectSensor kinect;
        Skeleton[] skeletonData;

        Timer countDownTimer;

        public GameWindow()
        {
            InitializeComponent();

            // Initialization
            InitBallsData();

            // Count down befoe start game
            countDownTimer = new Timer(
                new TimerCallback(
                    countDownTimerDelegate), 
                    null,
                    20,
                    60
                    );
            // Background worker
            CompositionTarget.Rendering += new EventHandler(Rendering);

            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            //this.KeyDown += new KeyEventHandler(this.controlPlayerAngle);
        }

        private void countDownTimerDelegate(object sender)
        {
            this.Dispatcher.BeginInvoke(
             (Action)delegate()
             {
                 //ChangeTimerImage();
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
            if (timerImgCount >= 0)
            {
                TimerImage.Source = new BitmapImage(
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
                    
                    GenerateBalls();


                    // Move Balls
                    CreateBallMoveAction();

                    string scoreStr = GameKernel.getScore.ToString();
                    string totalStr = GameKernel.totalCount.ToString();
                    if (GameKernel.getScore >= 0 && GameKernel.getScore < 10)
                    {
                        scoreStr = " " + scoreStr;
                    }
                    if (GameKernel.totalCount >= 0 && GameKernel.totalCount < 10)
                    {
                        totalStr = " " + totalStr;
                    }
                    this.ScoreText.Text
                        = scoreStr
                        + "/"
                        + totalStr;

                    DequeueBalls();
                    
                }
                
            }
            else if (GameStatus.STA_OVER == Running)
            {
                TimerImage.Source = gameOverImg;
                TimerImage.Opacity = 1;
                double hitRate = 0;
                if (GameKernel.totalCount != 0)
                {
                    hitRate = Math.Round(((double)GameKernel.getScore
                        * 100
                        / (double)GameKernel.totalCount),
                        1);
                }
                string hitRateStr = hitRate.ToString();
                string scoreStr = GameKernel.getScore.ToString();
                if (Double.IsNaN(hitRate))
                {
                    hitRateStr = "??";
                }
                this.ScoreText.Text
                        = scoreStr
                        + ", "
                        + hitRateStr
                        + "%";
            }
            
        }

        private void BackWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(10);
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
            Joint leftHand = (from j in s.Joints
                              where j.JointType == JointType.HandLeft
                              select j).FirstOrDefault();
            Joint rightHand = (from j in s.Joints
                               where j.JointType == JointType.HandRight
                               select j).FirstOrDefault();
            Joint head = (from j in s.Joints
                          where j.JointType == JointType.Head
                          select j).FirstOrDefault();
            Joint hipCenter = (from j in s.Joints
                               where j.JointType == JointType.HipCenter
                               select j).FirstOrDefault();

            /* Body Control */
            // Start game
            if (
                   Math.Abs(rightHand.Position.Y - leftHand.Position.Y) < 0.3
                && Math.Abs(rightHand.Position.Z - leftHand.Position.Z) < 0.3
                && rightHand.Position.X - leftHand.Position.X > 1.3
                && !firstStart
                )
            {
                firstStart = true;
                TimerImage.Source = null;
                
                Running = GameStatus.STA_START;
                GameKernel.totalCount = 0;
                GameKernel.getScore = 0;
            }

            // End game
            if (
                   (rightHand.Position.Y < head.Position.Y)
                && (rightHand.Position.Y > head.Position.Y - 0.15)
                && (leftHand.Position.Y < head.Position.Y)
                && (leftHand.Position.Y > head.Position.Y - 0.15)

                && (rightHand.Position.X > head.Position.X)
                && (rightHand.Position.X < head.Position.X + 0.15)
                && (leftHand.Position.X < head.Position.X)
                && (leftHand.Position.X > head.Position.X - 0.15)

                && (rightHand.Position.Z < head.Position.Z)
                && (rightHand.Position.Z > head.Position.Z - 0.2)
                && (leftHand.Position.Z < head.Position.Z)
                && (leftHand.Position.Z > head.Position.Z - 0.2)

                )
            {
                Running = GameStatus.STA_OVER;
                firstStart = false;
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


        private void InitBallsData()
        {
            //rfb = FlyingBall5;
            //rfb.img = FlyingBall5.FlyingBallImageSource;
            //rfb.xV = GameKernel.velocities[4].X;
            //rfb.yV = GameKernel.velocities[4].Y;
            //rfb.eId = 4;
               

            //balls.
            for (int i = 0; i < 10; i++)
                balls.Add(new FlyingBall());

            balls[0] = LeftBall;
            balls[1] = ObliqueLeftBall;
            balls[2] = MiddleBall;
            balls[3] = ObliqueRightBall;
            balls[4] = RightBall;

            // set velocities
            for (int i = 0; i < 5; i++)
            {
                balls[i].xV = GameKernel.velocities[i].X;
                balls[i].yV = GameKernel.velocities[i].Y;
            }
        }

        private void CreateBallMoveAction()
        {
            
            var dequeBalls =
                from ball in balls
                where (ball.state == BallState.DQUE)
                select ball;
            foreach (FlyingBall ball in dequeBalls)
            {
                
                ball.MoveBall();
                ball.RotateBall();
            }
        }

        private BitmapImage CreateBallImg()
        {
            return new BitmapImage(new Uri(@"Images/FlyingBall.png", UriKind.Relative));
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
            switch (e.Key)
            {
                case Key.A:
                    //rotatePlayer.Angle = GameKernel.Angles[0].Item1;
                    Maya.ControlAction(1);
                    playerAngle = 0;
                    break;
                case Key.S:
                    //rotatePlayer.Angle = GameKernel.Angles[1].Item1;
                    Maya.ControlAction(2);
                    playerAngle = 1;
                    break;
                case Key.D:
                    //rotatePlayer.Angle = GameKernel.Angles[2].Item1;
                    Maya.ControlAction(3);
                    playerAngle = 2;
                    break;
                case Key.F:
                    //rotatePlayer.Angle = GameKernel.Angles[3].Item1;
                    Maya.ControlAction(4);
                    playerAngle = 3;
                    break;
                case Key.G:
                    //rotatePlayer.Angle = GameKernel.Angles[4].Item1;
                    Maya.ControlAction(5);
                    playerAngle = 4;
                    break;
                case Key.F1:
                    Running = GameStatus.STA_START;
                    break;
                case Key.Escape:
                    Running = GameStatus.STA_OVER;
                    ResultPanel.ShowResult();
                    break;
                default:
                    break;
            }
        }

        private void GenerateBalls()
        {
            if (generateClock == 12)
            {
                GameKernel.totalCount += 1;

                FlyingBall theBall = new FlyingBall();
                int EId = rand.Next(0, 5);
                theBall.eId = EId;
                theBall.isClosed = false;
                balls[EId].eId = EId;
                //balls[EId].img.Source = CreateBallImg();
                theBall.state = BallState.EQUE;
                theBall.xV = GameKernel.velocities[EId].X;
                theBall.yV = GameKernel.velocities[EId].Y;

                enqueBalls.Enqueue(theBall);

                // set defualt clock
                generateClock = 0;
            }
        }

        private void DequeueBalls()
        {
            if (enqueBalls.Count > 0)
            {
                FlyingBall exitBall = enqueBalls.Dequeue();
                balls[exitBall.eId].state = BallState.DQUE;
            }
        }
    }
}
