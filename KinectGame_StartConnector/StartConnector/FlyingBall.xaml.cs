using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace StartConnector
{
	public partial class FlyingBall : UserControl
	{
        /********** DATA MEMBER **************/
        //public Image img = football1;
        public bool isClosed;
        public int eId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public BallState state;
        Storyboard action;
        /********** DATA MEMBER **************/
		public FlyingBall()
		{
			this.InitializeComponent();
		}

        public void RotateBall()
        {
            ChangeFootballAngle.Angle = (ChangeFootballAngle.Angle + 30) % 360;
        }
       


        //private void ReleaseImage()
        //{
        //    this.isClosed = false;
        //    //this.img.Source = null;
        //    this.img.Margin = new Thickness(
        //        GameKernel.startPoint[eId].X,
        //        GameKernel.startPoint[eId].Y,
        //        0,
        //        0
        //    );
        //    Canvas.SetLeft(img, 0);
        //    Canvas.SetTop(img, 0);
        //    this.state = BallState.NONE;
        //    GameWindow.playerStatus = ScoreStatus.SCO_NULL;
        //}

        public void CalcScore()
        {
            double xPos, yPos;

            xPos = Canvas.GetLeft(this.football1);
            yPos = Canvas.GetTop(this.football1);
            if(eId == 2)
                Console.WriteLine("xPos: " + xPos + " yPos: " + yPos);

            if (Double.IsNaN(xPos) && Double.IsNaN(yPos))
            {
                Console.WriteLine("Hey Shiy, NotANumber?!");
                Canvas.SetLeft(this.football1, 0);
                Canvas.SetTop(this.football1, 0);
                xPos = Canvas.GetLeft(this.football1);
                yPos = Canvas.GetTop(this.football1);
            }
            
            Point ballPoint = new Point(xPos + this.football1.Margin.Left, yPos + this.football1.Margin.Top);
            double distance = GameKernel.CalcDistance(GameKernel.playerPoint, ballPoint);
            //Console.WriteLine("distance: " + distance);
            if (distance > 0 && distance < 50)
            {
                //Console.WriteLine("CATCHED!!!");
                isClosed = true;
                // catch the ball
                if (eId == GameWindow.playerAngle)
                {
                    GameKernel.getScore += 1;
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);

                    //this.ReleaseImage();
                    GameWindow.playerStatus = ScoreStatus.SCO_CATCH;
                }
                // lose the ball
                else
                {
                    GameWindow.playerStatus = ScoreStatus.SCO_LOSE;
                }
            }
            else if (distance > 50 && isClosed)
            {
                GameWindow.netStatus = true;
                //this.ReleaseImage();
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, 0);
                isClosed = false;

                if (GameWindow.playerStatus != ScoreStatus.SCO_NULL)
                {
                    GameWindow.playerStatus = ScoreStatus.SCO_NULL;
                }
            }
            else
            {
                GameWindow.playerStatus = ScoreStatus.SCO_NULL;
                GameWindow.netStatus = false;
            }
        }
        public void MoveBall()
        {

            if (action!=null && action.GetCurrentState() == ClockState.Active)
            {
                return;
            }
            switch(eId)
            {
                case 0:
                    MoveLeftBall();
                    break;
                case 1:
                    MoveObliqueLeftBall();
                    break;
                case 2:
                    MoveMiddleBall();
                    break;
                case 3:
                    MoveObliqueRightBall();
                    break;
                case 4:
                    MoveRightBall();
                    break;
                default:
                    break;
            }
        }

        public void MoveLeftBall()
        {
            action = (Storyboard)Resources["MostLeft"];
            action.Begin();
        }

        public void MoveObliqueLeftBall()
        {
            action = (Storyboard)Resources["ObliqueLeft"];
            action.Begin();
        }
        public void MoveMiddleBall()
        {
            action = (Storyboard)Resources["Middle"];
            action.Begin();
        }

        public void MoveObliqueRightBall()
        {
            action = (Storyboard)Resources["ObliqueRight"];
            action.Begin();
        }

        public void MoveRightBall()
        {
            action = (Storyboard)Resources["MostRight"];
            action.Begin();
        }
	}
}