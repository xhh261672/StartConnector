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
    public enum BallState
    {
        EQUE,
        DQUE,
        NONE
    }
    public class Football
    {
        public Image img;
        public int gId;    // generate order
        public int qId;    // queue id
        public BallState state;

        //public void Hit(object sender, EventArgs e)
        //{
        //    // position from ball to user
        //    Point disPoint = this.img.TranslatePoint(new Point(), MainWindow.Player);
        //    double distance = Math.Sqrt(disPoint.X * disPoint.X + disPoint.Y * disPoint.Y);
        //    if (distance < 0)
        //    {
        //        // lose score
        //        releaseImage(img);
        //    }
        //    else if (distance > 0 && distance < 3.5)
        //    {
        //        // hit ball
        //    }
        //    else
        //    {

        //    }
        //}



        public void MoveBall(double xVelocity, double yVelocity, int exit)
        {
            double xPos = Canvas.GetLeft(this.img);
            double yPos = Canvas.GetTop(this.img);


            if (Double.IsNaN(xPos) && Double.IsNaN(yPos))
            {
                Canvas.SetLeft(this.img, MainWindow.startPoint[exit].X);
                Canvas.SetTop(this.img, MainWindow.startPoint[exit].Y);

            }
            else
            {
                xPos += xVelocity;
                yPos += yVelocity;
                Console.WriteLine(xPos + " : " + yPos);

                Canvas.SetLeft(this.img, xPos);
                Canvas.SetTop(this.img, yPos);
            }


            if ((xPos < -500) || (yPos > 400 && yPos < 500))
            {
                Canvas.SetLeft(this.img, MainWindow.startPoint[exit].X);
                Canvas.SetTop(this.img, MainWindow.startPoint[exit].Y);                
                releaseImage(this.img);
            }
        }

        // when a ball was hit or in doorframe, release the image resource
        private void releaseImage(Image img)
        {
            img.Source = null;
        }
    }
}
