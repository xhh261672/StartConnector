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

        public static bool InCatchScope(TimeSpan ts, double lowerBound, double upperBound)
        {
            return (ts.TotalSeconds > lowerBound && ts.TotalSeconds < upperBound);
        }

        public static bool TimeLimitExsit(TimeSpan ts, double limit)
        {
            return (ts.TotalSeconds >= limit);
        }

        //public static void Clock(int seconds)
        //{
        //    int endTickCount = System.Environment.TickCount + (seconds * 1000);
        //    while (System.Environment.TickCount < endTickCount) ;
        //}

        //public static Point[] velocities = new Point[]
        //{
        //    new Point(10,  4),
        //    new Point(8,   8),
        //    new Point(0,   7),
        //    new Point(-6,  6),
        //    new Point(-10, 4),

        //};

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



        public static double CalcRate()
        {
            return CalcPercentage(getBallCount, totalCount);
        }
        public static double CalcPercentage(double dividend, double divisor)
        {
            return Math.Round(dividend * 100 / divisor, 1);
        }

        public static double CalcPercentage(double number)
        {
            return Math.Round(number, 1);
        }

        //public static double yourRate = 0.0;
        
        public static int bestScore = 0;
        //public static int bestCombos = 0;
        
        public static int getScore = 0;
        public static int getCombos = 0;

        public static int comboCount = 0; // current combo
        public static int maxComboCount = 0; // best combo in current game
        public static int bestCombos = 0;// best combo in history

        public static int totalCount = 0;
        public static int getBallCount = 0;
    }
}
