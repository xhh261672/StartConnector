using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkeletonRecognize
{
    using System;
    using Microsoft.Kinect;
    using System.Linq;
    using System.Windows;
    
    using System.Runtime.InteropServices; //DllImport
    
    using Coding4Fun.Kinect.Wpf;//Joint,ScaleTo

    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void mouse_event(
            int dwFlags,
            int dx,
            int dy,
            int cButtons,
            int dwExtraInfo
        );
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,
            byte bScan,
            int dwFlags,
            int dwExtraInfo
        );



        KinectSensor sensor;
        Skeleton[]   skeletonData;
        byte[]       pixelData;
        bool isBackGestureActive = false,
                     isForwardGestureActive = false;



        public MainWindow()
        {
            InitializeComponent();


            
        }


        // initial window event
        private void Window_loaded(object sender, RoutedEventArgs e)
        {
            init_kinect();
            
            
        }

        //close window event
        private void Window_Closed(object sender, EventArgs e)
        {
            close_kinect();
        }


        private void init_kinect()
        {
            // LINQ find first sensor
            sensor = (from a_sensor in KinectSensor.KinectSensors
                      where a_sensor.Status == KinectStatus.Connected
                      select a_sensor).FirstOrDefault();
            // open stream
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30); // colorStream
            sensor.SkeletonStream.Enable(); // skeletonStream

            // start
            sensor.Start();

            // registe events
            sensor.ColorFrameReady += __ColorFrameReady;
            sensor.SkeletonFrameReady +=
                new EventHandler<SkeletonFrameReadyEventArgs>(__SkeletonFrameReady);
        
        }


        private void close_kinect()
        {
            sensor.Stop();
        }

        private void __ColorFrameReady(object sender, 
            ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (null != imageFrame)
                {
                    this.pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(pixelData);
                    this.ColorImage.Source =
                        BitmapImage.Create(imageFrame.Width, imageFrame.Height, 96, 96,
                        PixelFormats.Bgr32, null, pixelData,
                        imageFrame.Width * imageFrame.BytesPerPixel);
                }
            }
        }

        private void __SkeletonFrameReady(object sender,
            SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    skeletonData =
                        new Skeleton[
                            sensor.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                    Skeleton skeleton =
                        (from s in skeletonData
                         where s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();
                    if (null != skeleton)
                    {
                        SkeletonCanvas.Visibility = Visibility.Visible;
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
            var tempPosition = rightHand.ScaleTo(640, 480);
            Console.WriteLine("X: {0}", tempPosition.Position.X);
            Console.WriteLine("Y: {0}", tempPosition.Position.Y);
            SetCursorPos((int)tempPosition.Position.X, (int)tempPosition.Position.Y);
            if (rightHand.Position.X > head.Position.X + 0.45)
            {

                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isForwardGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{Right}");
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
                    System.Windows.Forms.SendKeys.SendWait("{Left}");
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


            joint.ScaleTo(640, 480);



            ColorImagePoint colorImagePoint =
                sensor.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position,
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




    }
}
