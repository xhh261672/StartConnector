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
            Console.WriteLine((rightHand.Position.X > 0.2 + leftHand.Position.X));
            if (rightHand.Position.X > leftHand.Position.X + 0.3)
            {
                GameWindow.kinect.SkeletonFrameReady -=
                        new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrame_Ready);
                GameWindow gameWindow = new GameWindow();
                gameWindow.Show();
                (this as Window).Close();

            }
        }

        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;


    }

}
