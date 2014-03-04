using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls; // Image
using System.Windows.Data;


namespace Demo
{

    using System.Threading;
    using System.Diagnostics;
    //using System.Threading;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Threading;

    // test
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Drawing;
    public enum BallState
    {
        EQUE,
        DQUE,
        NONE
    }

    public class Football
    {
        public Image img;
        public bool isClosed;
        public int eId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public BallState state;
        public void MoveBall()
        {

            double xPos = Canvas.GetLeft(this.img);
            double yPos = Canvas.GetTop(this.img);

            if (Double.IsNaN(xPos) && Double.IsNaN(yPos))
            {
                Canvas.SetLeft(this.img, 0);
                Canvas.SetTop(this.img, 0);
            }
            else
            {
                xPos += xV;
                yPos += yV;

                Canvas.SetLeft(this.img, xPos);
                Canvas.SetTop(this.img, yPos);
            }

            Point playerPoint = new Point(500, 320);
            Point ballPoint = new Point(xPos + img.Margin.Left, yPos + img.Margin.Top);
            double distance = GameData.CalcDistance(playerPoint, ballPoint);

            if (distance > 0 && distance < 50)
            {
                isClosed = true;
                // catch the ball
                if (eId == MainWindow.playerAngle)
                {
                    GameData.getScore += 1;
                    this.ReleaseImage();
                }

                
            }
            else if (distance > 50 && isClosed)
            {
                MainWindow.netStatus = true;
                this.ReleaseImage();
            }
            else
            {
                MainWindow.netStatus = false;
            }
        }

        private void ReleaseImage()
        {
            this.isClosed = false;
            this.img.Source = null;
            this.img.Margin = new Thickness(GameData.startPoint[eId].X, GameData.startPoint[eId].Y, 0, 0);
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            this.state = BallState.NONE;
        }

        
    }

    
}
