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
        bool hashPlayerd = false;
        bool runBackWorker = true;

        int timerImgCount = 3;
        Queue<FlyingBall> enqueBalls = new Queue<FlyingBall>();
        BackgroundWorker bw;
        List<FlyingBall> balls = new List<FlyingBall>(5);
        List<FlyingBottle> bottles = new List<FlyingBottle>(3);
        
        static Random rand = new Random();
        static int startGameCount = 31;

        public static ScoreStatus playerStatus = ScoreStatus.SCO_NULL;
        public static bool netStatus = false;
        static BitmapImage bottleImage = new BitmapImage(new Uri(@"Images/bottle.png", UriKind.Relative));
        static BitmapImage ballImage = new BitmapImage(new Uri(@"Images/newBall.png", UriKind.Relative));
        //static BitmapImage catchBall = new BitmapImage(new Uri(@"Images/get.png", UriKind.Relative));
        //static BitmapImage loseBall = new BitmapImage(new Uri(@"Images/miss.png", UriKind.Relative));
        //static BitmapImage playerLose = new BitmapImage(new Uri(@"Images/player.png", UriKind.Relative));
        //static BitmapImage playerCatch = new BitmapImage(new Uri(@"Images/player2.png", UriKind.Relative));
        static BitmapImage gameOverImg = new BitmapImage(new Uri(@"Images/gameover.png", UriKind.Relative));


        public static int playerAngle = 2;
        //public static int sleepTime = 100;

        

        Timer countDownTimer;

        public GameWindow()
        {
            InitializeComponent();
            this.
            InitObjectsData();

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
            if (timerImgCount >= 0)
            {
                TimerImage.Source = new BitmapImage(
                    new Uri(@"Images/timer" + timerImgCount + ".png", UriKind.Relative));
                --timerImgCount;
            }
            else
            {
                this.TimerImage.Source = null;
                countDownTimer.Dispose();
            }

        }



        private void Rendering(object sender, EventArgs e)
        {
            //Console.WriteLine("Rendering");
            if (Running == GameStatus.STA_START && isRendering)
            {
                isRendering = false;

                // Timer : wait 3 seconds to start game
                if (startGameCount <= 30)
                    startGameCount++;
                else
                {
                    GenerateObject();
                    UpdateScore_and_RotateObjects();

                    string scoreStr = Kernel.getScore.ToString();
                    string totalStr = Kernel.totalCount.ToString();
                    if (Kernel.getScore >= 0 && Kernel.getScore < 10)
                    {
                        scoreStr = " " + scoreStr;
                    }
                    if (Kernel.totalCount >= 0 && Kernel.totalCount < 10)
                    {
                        totalStr = " " + totalStr;
                    }
                    this.ScoreText.Text
                        = scoreStr
                        + "/"
                        + totalStr;
                }
                
            }
            else if (Running == GameStatus.STA_OVER)
            {
                TimerImage.Source = gameOverImg;
                TimerImage.Opacity = 1.0;
                Kernel.hitRate = 0.0;
                if (Kernel.totalCount != 0)
                {
                    Kernel.hitRate = Math.Round(((double)Kernel.getScore
                        * 100
                        / (double)Kernel.totalCount),
                        1);
                }
                string hitRateStr = Kernel.hitRate.ToString();
                string scoreStr = Kernel.getScore.ToString();
                //if (Double.IsNaN(Kernel.hitRate))
                //{
                //    hitRateStr = "??";
                //}
                this.ScoreText.Text
                        = scoreStr
                        + ", "
                        + hitRateStr
                        + "%";
            }
            
        }

        private void BackWork(object sender, DoWorkEventArgs e)
        {
            while (runBackWorker)
            {
                Thread.Sleep(100);
                isRendering = true;
            }
        }

        public static KinectSensor kinect;
        public static Skeleton[] skeletonData;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(kinect == null)
            {
                kinect = (from sensor in KinectSensor.KinectSensors
                          where sensor.Status == KinectStatus.Connected
                          select sensor
                          ).FirstOrDefault();
            }            
            else
            {
                //kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady +=
                    new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                //kinect.Start();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (kinect != null)
            {
                kinect.SkeletonFrameReady
                    -= new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                kinect.Stop();
            }
            TurnOffBackWorker();
            this.Close();
            Application.Current.Shutdown();
            
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
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton skeleton =
                        (from s in skeletonData
                         where s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();
                    if (null != skeleton)
                    {
                        GameWindowCanvas.Visibility = Visibility.Visible;
                        ProcessGesture(skeleton);

                    }
                }
            }
        }

        /* Hand Guesture Variables 
         *****************************/
        //static public List<GesturePoint> gesturePoints = new List<GesturePoint>();
        //bool gesturePointTrackingEnabled = true;
        GesturePoint startGesture, currentGesture;
        //double swipeLength = 1.2, swipeDeviation = 0.3;
        int swipeTime = 1000;
        /******************************
         * Hand Guesture Variables */



        /********************************* Swipe Event ****************************************/

        public void TrackingSwipeGesture(SkeletonPoint currentPoint)
        {
            //get current
            currentGesture = new GesturePoint()
            {
                X = currentPoint.X,
                Y = currentPoint.Y,
                Z = currentPoint.Z,
                T = DateTime.Now
            };
            // if curr.x is less than spine.x - 0.1
            // assign start point
            if ((currentGesture.Z < Kernel.spine.Position.Z - 0.7 && currentGesture.Z > Kernel.spine.Position.Z) ||
                (currentGesture.X < Kernel.spine.Position.X - 0.1)
                )
            {
                startGesture = currentGesture;
                return;
            }
            if (((currentGesture.Y > Kernel.hipCenter.Position.Y && currentGesture.Y < Kernel.head.Position.Y))
            && (currentGesture.Z > Kernel.spine.Position.Z - 0.5 && currentGesture.Z < Kernel.spine.Position.Z)
            && (currentGesture.X > Kernel.spine.Position.X - 0.4 || currentGesture.X < Kernel.spine.Position.X + 0.1))
            {
                if ((currentGesture.T - startGesture.T).Milliseconds <= swipeTime)
                {
                    //judge gesture
                    // seccussful gesture
                    if ((currentGesture.X - startGesture.X < 1 && currentGesture.X - startGesture.X > 0.6)
                        && (Math.Abs(currentGesture.Y - startGesture.Y) < 0.3)
                        && (Math.Abs(currentGesture.Z - startGesture.Z) < 0.2)
                       )
                    {
                        startGesture = currentGesture;
                        removeAllBottles();
                        Console.WriteLine("TEAH!!!!!!!!!");
                    }
                }
            }
        }

        /********************************* Swipe Event ****************************************/
        private void ProcessGesture(Skeleton s)
        {
            // Recognize Joints
            Kernel.leftHand = (from j in s.Joints
                              where j.JointType == JointType.HandLeft
                              select j).FirstOrDefault();
            Kernel.rightHand = (from j in s.Joints
                               where j.JointType == JointType.HandRight
                               select j).FirstOrDefault();
            Kernel.head = (from j in s.Joints
                          where j.JointType == JointType.Head
                          select j).FirstOrDefault();
            Kernel.hipCenter = (from j in s.Joints
                               where j.JointType == JointType.HipCenter
                               select j).FirstOrDefault();
            Kernel.spine = (from j in s.Joints
                           where j.JointType == JointType.Spine
                           select j).FirstOrDefault();
            Kernel.shoulderCenter = (from j in s.Joints
                           where j.JointType == JointType.ShoulderCenter
                           select j).FirstOrDefault();
            Kernel.elbowLeft = (from j in s.Joints
                                    where j.JointType == JointType.ElbowLeft
                                    select j).FirstOrDefault();
            Kernel.elbowRight = (from j in s.Joints
                                    where j.JointType == JointType.ElbowRight
                                    select j).FirstOrDefault();

            /* Body Control */
            //HandleGestureTracking(rightHand.Position);
            TrackingSwipeGesture(Kernel.rightHand.Position);
            // Start game

            if (

                   Math.Abs(Kernel.rightHand.Position.Y - Kernel.leftHand.Position.Y) < 0.3
                && Math.Abs(Kernel.rightHand.Position.Z - Kernel.leftHand.Position.Z) < 0.3
                && Kernel.rightHand.Position.X - Kernel.leftHand.Position.X > 1.3
                && !firstStart
                )
            {
                firstStart = true;
                TimerImage.Source = null;
                
                Running = GameStatus.STA_START;
                Kernel.totalCount = 0;
                Kernel.getScore = 0;
            }

            // End game
            if (
                   (Kernel.rightHand.Position.Y < Kernel.head.Position.Y)
                && (Kernel.rightHand.Position.Y > Kernel.head.Position.Y - 0.15)
                && (Kernel.leftHand.Position.Y < Kernel.head.Position.Y)
                && (Kernel.leftHand.Position.Y > Kernel.head.Position.Y - 0.15)

                && (Kernel.rightHand.Position.X > Kernel.head.Position.X)
                && (Kernel.rightHand.Position.X < Kernel.head.Position.X + 0.15)
                && (Kernel.leftHand.Position.X < Kernel.head.Position.X)
                && (Kernel.leftHand.Position.X > Kernel.head.Position.X - 0.15)

                && (Kernel.rightHand.Position.Z < Kernel.head.Position.Z)
                && (Kernel.rightHand.Position.Z > Kernel.head.Position.Z - 0.2)
                && (Kernel.leftHand.Position.Z < Kernel.head.Position.Z)
                && (Kernel.leftHand.Position.Z > Kernel.head.Position.Z - 0.2)

                )
            {
                Running = GameStatus.STA_OVER;
                firstStart = false;
            }

            // most right and left
            if (
                (Kernel.rightHand.Position.X - Kernel.hipCenter.Position.X > 0.35)
                && Math.Abs(Kernel.leftHand.Position.X - Kernel.rightHand.Position.X) < 1
                && Math.Abs(Kernel.leftHand.Position.Y - Kernel.rightHand.Position.Y) < 1
                && Math.Abs(Kernel.leftHand.Position.Z - Kernel.rightHand.Position.Z) < 0.5
                && (Kernel.rightHand.Position.Y < Kernel.head.Position.Y)
                && ((Kernel.head.Position.X - Kernel.hipCenter.Position.X) > 0.1)
                && (Kernel.head.Position.Y > Kernel.rightHand.Position.Y)
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
                 (Kernel.leftHand.Position.X - Kernel.hipCenter.Position.X < -0.35)
                && Math.Abs(Kernel.leftHand.Position.X - Kernel.rightHand.Position.X) < 1
                && Math.Abs(Kernel.leftHand.Position.Y - Kernel.rightHand.Position.Y) < 1
                && Math.Abs(Kernel.leftHand.Position.Z - Kernel.rightHand.Position.Z) < 0.5
                && (Kernel.leftHand.Position.Y < Kernel.head.Position.Y)
                && (Kernel.head.Position.X - Kernel.hipCenter.Position.X < -0.1)
                && (Kernel.head.Position.Y > Kernel.leftHand.Position.Y)
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

            // Middle direction
            if (
                (Kernel.leftHand.Position.X > Kernel.head.Position.X - 0.4)
                && (Kernel.leftHand.Position.X < Kernel.head.Position.X)
                && (Kernel.rightHand.Position.X < Kernel.head.Position.X + 0.4)
                && (Kernel.rightHand.Position.X > Kernel.head.Position.X)
                && (Kernel.rightHand.Position.Y > Kernel.head.Position.Y + 0.15)
                && (Kernel.leftHand.Position.Y > Kernel.head.Position.Y + 0.15)
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
            if ((Math.Abs(Kernel.leftHand.Position.X - Kernel.rightHand.Position.X) < 1)
                && Math.Abs(Kernel.leftHand.Position.Y - Kernel.rightHand.Position.Y) < 1
                && Math.Abs(Kernel.leftHand.Position.Z - Kernel.rightHand.Position.Z) < 0.5
                && (Kernel.leftHand.Position.Y > Kernel.head.Position.Y)
                && (Kernel.rightHand.Position.Y > Kernel.head.Position.Y)
                && ((Kernel.head.Position.X - Kernel.hipCenter.Position.X) > 0.1)
                )
            {
                if (!isObliqueLeft && !isObliqueRight)
                {
                    isObliqueRight = false;
                    System.Windows.Forms.SendKeys.SendWait("{F}");
                }
            }

            // oblique left
            if ((Math.Abs(Kernel.leftHand.Position.X - Kernel.rightHand.Position.X) < 1)
                && Math.Abs(Kernel.leftHand.Position.Y - Kernel.rightHand.Position.Y) < 1
                && Math.Abs(Kernel.leftHand.Position.Z - Kernel.rightHand.Position.Z) < 0.5
                && (Kernel.leftHand.Position.Y > Kernel.head.Position.Y)
                && (Kernel.rightHand.Position.Y > Kernel.head.Position.Y)
                && (Kernel.hipCenter.Position.X - Kernel.head.Position.X > 0.1)
                && (Kernel.hipCenter.Position.X - Kernel.head.Position.X < 0.3)
                )
            {
                if (!isObliqueLeft && !isObliqueRight)
                {
                    isObliqueLeft = false;
                    System.Windows.Forms.SendKeys.SendWait("{S}");
                }
            }
        }


        private void InitObjectsData()
        {
              
            //balls.
            for (int i = 0; i < 5; i++)
                balls.Add(new FlyingBall());
            for (int i = 0; i < 3; i++)
            {
                bottles.Add(new FlyingBottle());
                bottles[i].bId = -1;
            }

            bottles[0] = bottle1;
            bottles[1] = bottle2;
            bottles[2] = bottle3;

            balls[0] = LeftBall;
            balls[1] = ObliqueLeftBall;
            balls[2] = MiddleBall;
            balls[3] = ObliqueRightBall;
            balls[4] = RightBall;
        }

        private void UpdateScore_and_RotateObjects()
        {
            var dequeBalls =
                from ball in balls
                where (ball.state == BallState.DQUE)
                select ball;
            foreach (FlyingBall ball in dequeBalls)
            {
                ball.CalcScore();
                ball.RotateBall();
            }

            var generateBottles =
                from bottle in bottles
                where (bottle.state == BallState.DQUE)
                select bottle;
            foreach (FlyingBottle bottle in generateBottles)
            {
                bottle.CalcScore();
                bottle.RotateBottle();
            }
        }


        private void RunBackWorker()
        {
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackWork);
            bw.RunWorkerAsync();
            
        }

        private void TurnOffBackWorker()
        {
            Console.WriteLine("CLOSED");
            runBackWorker = false;
            CompositionTarget.Rendering -= new EventHandler(Rendering);
            bw.DoWork -= new DoWorkEventHandler(BackWork);
            bw.Dispose();
        }

        // Keyboard control
        private void controlPlayerAngle(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    Maya.ControlAction(1);
                    playerAngle = 0;
                    break;
                case Key.S:
                    Maya.ControlAction(2);
                    playerAngle = 1;
                    break;
                case Key.D:
                    Maya.ControlAction(3);
                    playerAngle = 2;
                    break;
                case Key.F:
                    Maya.ControlAction(4);
                    playerAngle = 3;
                    break;
                case Key.G:
                    Maya.ControlAction(5);
                    playerAngle = 4;
                    break;
                case Key.F1:
                    Running = GameStatus.STA_START;
                    hashPlayerd = true;
                    break;
                case Key.Escape:
                    Running = GameStatus.STA_OVER;
                    if (hashPlayerd)
                        ResultPanel.ShowResult();
                    break;
                default:
                    break;
            }
        }

        public void removeAllBottles()
        {
            var bottleCollection = (from bottle in bottles
                                    where bottle.state == BallState.DQUE
                                    select bottle);
            foreach (FlyingBottle bottle in bottleCollection)
            {
                bottle.updateBottleState();
                bottle.action.Stop();
                
            }
        }

        static int dequeCount = 8;
        private void GenerateObject()
        {
            if (dequeCount == 0)
            {
                ++Kernel.totalCount;

                int EId;
                EId = rand.Next(0, 20) % 6;
                //objId.Enqueue(EId);

                #region GenerateBall
                if (EId != 5)
                {
                    FlyingBall theBall = new FlyingBall();
                    theBall.eId = EId;
                    
                    balls[EId].eId = EId;
                    balls[EId].image.Source = ballImage;
                    theBall.state = BallState.EQUE;

                    enqueBalls.Enqueue(theBall);

                    DequeueBalls();                
                }
                #endregion
                #region GenerateBottles
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        bottles[i].bId = rand.Next(0, 5);
                        bottles[i].state = BallState.DQUE;
                        bottles[i].image.Source = bottleImage;
                        bottles[i].MoveBottle();
                    }
                    return;
                }
                #endregion
            }
            dequeCount--;
        }
        private void DequeueBalls()
        {
            // dequeCount can't be less than 0
            if (dequeCount<=0 && enqueBalls.Count>0)
            {
                FlyingBall exitBall = enqueBalls.Dequeue();
                balls[exitBall.eId].state = BallState.DQUE;
                dequeCount = 8;
                balls[exitBall.eId].MoveBall();
            }
        }


    }
}
