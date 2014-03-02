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
    public class Football
    {
        public Image img;
        public int gId;   // generate order
        public int qId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public int perms; // wait time
        public BallState state;
        //public MainWindow thisWindow = new MainWindow();
        public void MoveBall(double xVelocity, double yVelocity)
        {
            //Console.WriteLine("moveBall thread called");

            double xPos = Canvas.GetLeft(this.img);
            double yPos = Canvas.GetTop(this.img);


            if (Double.IsNaN(xPos) && Double.IsNaN(yPos))
            {
                Canvas.SetLeft(this.img, GameData.startPoint.X);
                Canvas.SetTop(this.img, GameData.startPoint.Y);
            }
            else
            {
                xPos += xVelocity;
                yPos += yVelocity;

                Canvas.SetLeft(this.img, xPos);
                Canvas.SetTop(this.img, yPos);
            }

            Point disPoint = this.img.TranslatePoint(new Point(0, 0), new MainWindow().canvas);
            double distance = Math.Sqrt(disPoint.X * disPoint.X + disPoint.Y * disPoint.Y);
            //Point ds = img.PointFromScreen(disPoint);
            //Console.WriteLine(ds.X + " : " + ds.Y);
            //Point playerPoint = new Point(500, 320);
            //Point ballPoint = new Point(xPos + Canvas.GetLeft(img), yPos + Canvas.GetTop(img));
            //double distance = Math.Sqrt((playerPoint.X - ballPoint.X) * (playerPoint.X - ballPoint.X)
               // + (playerPoint.Y - ballPoint.Y) * (playerPoint.Y - ballPoint.Y));
            if (distance > 0 && distance < 40)
            {
                ++GameData.getScore;
                ReleaseImage(img);
            }
        }

        private void ReleaseImage(Image img)
        {
            img.Source = null;
        }

        // when a ball was hit or in doorframe, release the image resource
        private void releaseImage(Image img)
        {
            img.Source = null;
            Canvas.SetLeft(img, GameData.startPoint.X);
            Canvas.SetTop(img, GameData.startPoint.Y);
        }
    }

    public partial class MainWindow : Window
    {
        // class members
        bool isRendering = false;
        
        // Replace by int queue
        static Queue<int> ballsGid = new Queue<int>();
        static Queue<Thread> ballsThread = new Queue<Thread>();

        List<Football> balls = new List<Football>();
        static Random rand = new Random();

        Football ball3 = new Football();
        // five ball move threads

        

        public MainWindow()
        {
            InitializeComponent();

            // Initialization
            LoadPlayerImage();
            InitBalls();

            // Background worker
            CompositionTarget.Rendering += new EventHandler(Rendering);
            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            this.KeyDown += new KeyEventHandler(this.controlAngle);

            this.Football3.Source = CreateNewball();
            ball3.img = this.Football3;
            
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
            balls[0].xV = balls[5].xV = GameData.velocities[0].X;
            balls[0].yV = balls[5].yV = GameData.velocities[0].Y;
            balls[1].xV = balls[5].xV = GameData.velocities[1].X;
            balls[1].yV = balls[5].yV = GameData.velocities[1].Y;
            balls[2].xV = balls[5].xV = GameData.velocities[2].X;
            balls[2].yV = balls[5].yV = GameData.velocities[2].Y;
            balls[3].xV = balls[5].xV = GameData.velocities[3].X;
            balls[3].yV = balls[5].yV = GameData.velocities[3].Y;
            balls[4].xV = balls[5].xV = GameData.velocities[4].X;
            balls[4].yV = balls[5].yV = GameData.velocities[4].Y;
        }


        

        private void CreateBallMoveThread()
        {
            var dequeBalls =
                from ball in balls
                where ball.img.Source != null
                select ball;
            foreach (Football ball in dequeBalls) 
            {
                BallMoveThread(ball, ball.xV, ball.yV);            
            }
            
        }

        delegate void BallMoveDelegate(double xVelocity, double yVelocity);

        private void BallMoveThread(Football theBall, double xV, double yV)
        {
            //Console.WriteLine("Ball move thread called");
            BallMoveDelegate bmd = new BallMoveDelegate(theBall.MoveBall);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, bmd, xV, yV);
        }

        // Load basic pictures
        
        

        private void LoadPlayerImage()
        {
            string playerImgUrl = @"Images/player.png";
            this.Player.Source = new BitmapImage(new Uri(playerImgUrl, UriKind.Relative));
        }

        private BitmapImage CreateNewball()
        {
            return new BitmapImage(new Uri(@"Images/football.png", UriKind.Relative));
        }


        private void RunBackWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackWork);
            bw.RunWorkerAsync();
        }

        private void Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {
                //Point disPoint = this.ball3.img.TranslatePoint(new Point(), this.Player);
                //double distance = Math.Sqrt(disPoint.X * disPoint.X + disPoint.Y * disPoint.Y);

                isRendering = false;

                this.ScoreText.Text = GameData.getScore.ToString();                
                CreateBallMoveThread();                
            
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
                    rotatePlayer.Angle = 290;
                    // this.Player.Source = null;
                    break;
                case Key.S:
                    rotatePlayer.Angle = 325;
                    break;
                case Key.D:
                    rotatePlayer.Angle = 0;
                    break;
                case Key.F:
                    rotatePlayer.Angle = 35;
                    break;
                case Key.G:
                    rotatePlayer.Angle = 70;
                    break;
                default:
                    break;
            }
        }
    
        private void GenerateBalls()
        {
            // create some new balls
            int newBallsCount = rand.Next(2, 4);
            GameData.totalCount += newBallsCount;

            // find the null image to assign new image source
            //for (int i = 0; i < queBalls.Count; i++)
            foreach (Football ball in balls)
            {
                if (null == ball.img.Source)
                {
                    ball.img.Source = CreateNewball();

                    // enqueue new balls
                    ball.state = BallState.EQUE;
                    int randId = rand.Next(0, 5);
                    //balls[randId].Enqueue(ball);
                    ballsGid.Enqueue(randId);
                }
            }
            // set position and move ball            
            // ...
        }


    
    }

}
