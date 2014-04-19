using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartConnector
{
    using Microsoft.Kinect;


    public struct GesturePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public DateTime T { get; set; }
        public override bool Equals(object obj)
        {
            var o = (GesturePoint)obj;
            return (X == o.X) && (Y == o.Y) && (Z == o.Z) && (T == o.T);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    

    public enum BallState
    {
        EQUE,
        DQUE,
        NONE
    }

    public enum GameStatus
    {
        STA_NULL,
        STA_START,
        STA_OVER
    };

    public enum ScoreStatus
    {
        SCO_CATCH,
        SCO_LOSE,
        SCO_NULL
    };


    class Kernel
    {
        //public static KinectSensor sensor;
        public static Joint leftHand, rightHand, head, hipCenter, spine, shoulderCenter, elbowLeft, elbowRight;

        public static double CalcDistance(Point From, Point To)
        {
            return Math.Sqrt((From.X - To.X) * (From.X - To.X)
                + (From.Y - To.Y) * (From.Y - To.Y));
        }

        public static bool inCatchScope(TimeSpan ts)
        {
            return (ts.TotalSeconds > 1.65 && ts.TotalSeconds < 1.83);
        }

        public static bool timeLimitExsit(TimeSpan ts)
        {
            return (ts.TotalSeconds >= 2.2);
        }

        public static void Clock(int seconds)
        {
            int endTickCount = System.Environment.TickCount + (seconds * 1000);
            while (System.Environment.TickCount < endTickCount) ;
        }

        public static Point[] velocities = new Point[]
        {
            new Point(10,  4),
            new Point(8,   8),
            new Point(0,   7),
            new Point(-6,  6),
            new Point(-10, 4),

        };

        //public static Point[] startPoint = new Point[]
        //{
        //    new Point(10,  150),
        //    new Point(213, 51),
        //    new Point(480, 25),
        //    new Point(737, 51),
        //    new Point(930, 150),            
        //};

        //public static Tuple<double, int>[] Angles = new Tuple<double, int>[]
        //{
            
        //    new Tuple<double, int>(-72, 0),
        //    new Tuple<double, int>(-50, 1),
        //    new Tuple<double, int>(0,   2),
        //    new Tuple<double, int>(50,  3),
        //    new Tuple<double, int>(72,  4),
        //};

        public static Point playerPoint = new Point(600, 340);
        public static Point zeroPoint = new Point(30, 120);
        public static Point middleEnd = new Point(30, 430);


        public static int comboCount = 0;
        public static int maxComboCount = 0;
        public static double hitRate = 0.0;
        public static int totalCount = 0;
        public static int getScore = 0;

    }
}
