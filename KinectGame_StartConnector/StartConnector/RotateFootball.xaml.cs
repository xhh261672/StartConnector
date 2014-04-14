using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls; // Image
using System.Windows.Data;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace StartConnector
{

    public partial class RotateFootball : UserControl
    {
        /********** DATA MEMBER **************/
        public Image img;
        public bool isClosed;
        public int eId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public BallState state;
        /********** DATA MEMBER **************/


        public RotateFootball()
        {
            InitializeComponent();
            isClosed = false;
            
        }
        /********** METHODS **************/

        public void RotateBall()
        {
            // rotating
            ChangeFootballAngle.Angle = (ChangeFootballAngle.Angle + 30) % 360;
        }
        public void MoveBall()
        {
            double xPos, yPos;
            //try
            //{
                if(img == null)
                {
                    Console.WriteLine("NULL IMG");
                    return;
                }
                xPos = Canvas.GetLeft(this);
                yPos = Canvas.GetTop(this);
                
                if (Double.IsNaN(xPos) && Double.IsNaN(yPos))
                {
                    Console.WriteLine("Hey Shiy, NotANumber?!");
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                }
                else
                {
                    Console.WriteLine("before xPos " + xPos + " yPos " + yPos);
                    xPos += xV;
                    yPos += yV;


                    Canvas.SetLeft(this, xPos);
                    Canvas.SetTop(this, yPos);
                    Console.WriteLine("after  xPos " + xPos + " yPos " + yPos);
                }

                Point ballPoint = new Point(xPos + this.Margin.Left, yPos + this.Margin.Top);
                double distance = GameKernel.CalcDistance(GameKernel.playerPoint, ballPoint);
                Console.WriteLine("distance: " + distance);
                if (distance > 0 && distance < 50)
                {
                    isClosed = true;
                    // catch the ball
                    if (eId == MainWindow.playerAngle)
                    {
                        GameKernel.getScore += 1;
                        Canvas.SetLeft(this, 0);
                        Canvas.SetTop(this, 0);

                        //this.ReleaseImage();
                        MainWindow.playerStatus = ScoreStatus.SCO_CATCH;
                    }
                    // lose the ball
                    else
                    {
                        MainWindow.playerStatus = ScoreStatus.SCO_LOSE;
                    }
                }

                else if (distance > 50 && isClosed)
                {
                    MainWindow.netStatus = true;
                    //this.ReleaseImage();
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    isClosed = false;
                    
                    if (MainWindow.playerStatus != ScoreStatus.SCO_NULL)
                    {
                        MainWindow.playerStatus = ScoreStatus.SCO_NULL;
                    }
                }
                else
                {
                    MainWindow.playerStatus = ScoreStatus.SCO_NULL;
                    MainWindow.netStatus = false;
                }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.StackTrace);
            //}

            
        }

        private void ReleaseImage()
        {
            this.isClosed = false;
            this.img.Source = null;
            this.img.Margin = new Thickness(
                GameKernel.startPoint[eId].X,
                GameKernel.startPoint[eId].Y,
                0,
                0
            );
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            this.state = BallState.NONE;
            MainWindow.playerStatus = ScoreStatus.SCO_NULL;
        }
    }
}
