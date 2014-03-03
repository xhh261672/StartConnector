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
        public void MoveBall()
        {

            double xPos = Canvas.GetLeft(this.img);
            double yPos = Canvas.GetTop(this.img);
            //Console.WriteLine("xPos: " + xPos + " --- yPos: " + yPos);


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
            //Console.WriteLine(distance);
            if (distance > 0 && distance < 30)
            {
                ++GameData.getScore;
                this.ReleaseImage();
            }
            else if (distance > 1000)
            {
                this.ReleaseImage();
            }
        }

        private void ReleaseImage()
        {
            this.img.Source = null;
            Console.WriteLine();
            img.Margin = new Thickness(GameData.startPoint[eId].X, GameData.startPoint[eId].Y, 0, 0);
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            Console.WriteLine("eid:" + eId + " " + img.Name + " " + img.Margin.Left + " : " + img.Margin.Top);
            
            this.state = BallState.NONE;

        }

    }

    
}
