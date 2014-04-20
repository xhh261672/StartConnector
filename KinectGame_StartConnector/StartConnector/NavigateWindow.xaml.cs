using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StartConnector
{
    using Microsoft.Kinect;

    using System.Windows.Media.Animation;
    using System.Windows.Navigation;
    using System.Windows.Threading;
    using System.Threading;

    public partial class NavigateWindow : NavigationWindow
    {
        public NavigateWindow()
        {
            InitializeComponent();
        }
       
        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;
                _navArgs = e;
                this.IsHitTestVisible = false;
                DoubleAnimation da = new DoubleAnimation(0.3d, new Duration(TimeSpan.FromMilliseconds(300)));
                da.Completed += FadeOutCompleted;
                this.BeginAnimation(OpacityProperty, da);
            }
            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= FadeOutCompleted;

            this.IsHitTestVisible = true;

            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                    {
                        NavigationService.Navigate(_navArgs.Content);
                    }
                    else
                    {
                        NavigationService.Navigate(_navArgs.Uri);
                    }
                    break;
                case NavigationMode.Back:
                    NavigationService.GoBack();
                    break;

                case NavigationMode.Forward:
                    NavigationService.GoForward();
                    break;
                case NavigationMode.Refresh:
                    NavigationService.Refresh();
                    break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate()
            {
                DoubleAnimation da = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(200)));
                this.BeginAnimation(OpacityProperty, da);
            });
        }

        public void SwitchStartGame(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // start game
                case Key.Right:
                    if (null != GameWindow.kinect)
                        GameWindow.kinect.SkeletonFrameReady -= 
                        new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                    GameWindow gameWindow = new GameWindow();
                    gameWindow.Show();
                    (this as Window).Close();
                    break;
                // keep munaul
                default:
                    //Console.WriteLine("BUG");
                    break;
            }
        }

        private void Menual_Loaded(object sender, RoutedEventArgs e)
        {
            GameWindow.kinect = (from sensor in KinectSensor.KinectSensors
                                 where sensor.Status == KinectStatus.Connected
                                 select sensor
                      ).FirstOrDefault();
            if (null != GameWindow.kinect)
            {
                GameWindow.kinect.SkeletonStream.Enable();
                GameWindow.kinect.SkeletonFrameReady +=
                    new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                GameWindow.kinect.Start();
            }
        }

        GesturePoint currentGesture, startGesture;
        int swipeTime = 1000;

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
            Console.WriteLine("x: " + currentGesture.X + "z: " + currentGesture.Z);
            Console.WriteLine("spine: " + Kernel.spine.Position.Z);
            if ((currentGesture.Z < Kernel.spine.Position.Z - 0.7 && currentGesture.Z > Kernel.spine.Position.Z) ||
                (currentGesture.X > Kernel.spine.Position.X + 0.1)
                )
            {
                Console.WriteLine("start point");
                startGesture = currentGesture;
                return;
            }
            // valid gesture range
                //Console.WriteLine("1");
                //Console.WriteLine("2");
            if (((currentGesture.Y > Kernel.hipCenter.Position.Y && currentGesture.Y < Kernel.head.Position.Y))
            && (currentGesture.Z > Kernel.spine.Position.Z - 0.7 && currentGesture.Z < Kernel.spine.Position.Z)
            && (currentGesture.X < Kernel.spine.Position.X + 0.4 || currentGesture.X > Kernel.spine.Position.X - 0.1))
            {
              //  Console.WriteLine("3");
                // valid time duration
                if ((currentGesture.T - startGesture.T).Milliseconds <= swipeTime)
                {
                    //judge gesture
                    // seccussful gesture
                   // Console.WriteLine("ohyeah");
                    if ((currentGesture.X - startGesture.X > -1 && currentGesture.X - startGesture.X < -0.6)
                        && (Math.Abs(currentGesture.Y - startGesture.Y) < 0.3)
                        && (Math.Abs(currentGesture.Z - startGesture.Z) < 0.2)
                       )
                    {
                        Console.WriteLine("ohyeah");
                        // assign current gesture to start
                        startGesture = currentGesture;

                        // do things you want
                        GameWindow.kinect.SkeletonFrameReady -=
                        new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                        GameWindow gameWindow = new GameWindow();
                        gameWindow.Show();
                        (this as Window).Close();
                    }
                }
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
                    GameWindow.skeletonData =
                        new Skeleton[
                            GameWindow.kinect.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(GameWindow.skeletonData);
                    Skeleton skeleton =
                        (from s in GameWindow.skeletonData
                         where s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();
                    if (null != skeleton)
                    {
                        //GameWindowCanvas.Visibility = Visibility.Visible;
                        StartGameGesture(skeleton);
                    }
                }
            }
        }

        private void StartGameGesture(Skeleton s)
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
            Kernel.spine = (from j in s.Joints
                            where j.JointType == JointType.Spine
                               select j).FirstOrDefault();

            TrackingSwipeGesture(Kernel.leftHand.Position);
            //Console.WriteLine((Kernel.rightHand.Position.X > 0.2 + Kernel.leftHand.Position.X));
            //if (Kernel.rightHand.Position.X > Kernel.leftHand.Position.X + 0.3)
            //{
                
            //}
        }

        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;

    }
}
