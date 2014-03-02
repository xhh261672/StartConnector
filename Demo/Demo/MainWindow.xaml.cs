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
    

    public partial class MainWindow : Window
    {
        // class members
        bool isRendering = false;
        
        // Replace by int queue
        static Queue<Football> enqueBalls = new Queue<Football>();

        List<Football> balls = new List<Football>();
        static Random rand = new Random();

        
        // five ball move threads

        

        public MainWindow()
        {
            InitializeComponent();

            // Initialization
            LoadPlayerImage();
            InitBalls();


            GenerateBalls();
            while (enqueBalls.Count > 0)
            {
                GameData.Clock(2);
                DeueBalls();
            }

            // Background worker
            CompositionTarget.Rendering += new EventHandler(Rendering);
            RunBackWorker();

            // regist keydown event to control angle of player's orientation
            this.KeyDown += new KeyEventHandler(this.controlAngle);
            
            
            //balls[3].img.Source = CreateBallImg();
            //balls[2].img.Source = CreateNewball();
            //balls[1].img.Source = CreateNewball();
            //balls[4].img.Source = CreateNewball();
            //balls[0].img.Source = CreateNewball();


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
                balls[i].xV = GameData.velocities[i % 5].X;
                balls[i].yV = GameData.velocities[i % 5].Y;
            }
        }


        

        private void CreateBallMoveThread()
        {
            var dequeBalls =
                from ball in balls
                where ball.img != null && ball.state == BallState.DQUE
                select ball;
            foreach (Football ball in dequeBalls) 
            {
                ball.MoveBall(ball.xV, ball.yV);
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

        private void Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {

                isRendering = false;
                CreateBallMoveThread();                
                this.ScoreText.Text = GameData.getScore.ToString();                
            
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
            int generateCount = rand.Next(3, 6);
            GameData.totalCount += generateCount;

            for (int i = 0; i < generateCount; i++)
            {
                Football theBall = new Football();
                theBall.eId = rand.Next(0, 5);
                //theBall.img.Source = CreateBallImg();
                balls[theBall.eId].state = BallState.EQUE;
                balls[theBall.eId].img.Source = CreateBallImg();
                enqueBalls.Enqueue(theBall);
            }
            // set position and move ball            
            // ...
        }

        private void DeueBalls()
        {
            Football exitBall = enqueBalls.Dequeue();
            //exitBall.state = BallState.DQUE;
            balls[exitBall.eId].state = BallState.DQUE;
        }
    }
}
