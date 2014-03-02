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
        public int gId;   // generate order
        public int eId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public int perms; // wait time
        public BallState state;
        public void MoveBall(double xVelocity, double yVelocity)
        {

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

            Point playerPoint = new Point(500, 320);
            Point ballPoint = new Point(xPos + img.Margin.Left, yPos + img.Margin.Top);
            double distance = GameData.CalcDistance(playerPoint, ballPoint);
            Console.WriteLine(distance);
            if (distance > 0 && distance < 40)
            {
                ++GameData.getScore;
                this.ReleaseImage();
            }
        }

        private void ReleaseImage()
        {
            this.img.Source = null;
            Canvas.SetLeft(this.img, 0);
            Canvas.SetTop(this.img, 0);
            // this.state = BallState.NONE;

        }

        // when a ball was hit or in doorframe, release the image resource
        private void releaseImage(Image img)
        {
            img.Source = null;
            Canvas.SetLeft(img, GameData.startPoint.X);
            Canvas.SetTop(img, GameData.startPoint.Y);
        }
    }

    
}
