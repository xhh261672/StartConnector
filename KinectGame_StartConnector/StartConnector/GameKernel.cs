using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartConnector
{
    using Microsoft.Kinect;

    
    //int s=3;

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


    class GameKernel
    {
        //public static KinectSensor sensor;

        public static double CalcDistance(Point From, Point To)
        {
            return Math.Sqrt((From.X - To.X) * (From.X - To.X)
                + (From.Y - To.Y) * (From.Y - To.Y));
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

        public static Point[] startPoint = new Point[]
        {
            new Point(10,  150),
            new Point(213, 51),
            new Point(480, 25),
            new Point(737, 51),
            new Point(930, 150),            
        };

        public static Tuple<double, int>[] Angles = new Tuple<double, int>[]
        {
            
            new Tuple<double, int>(-72, 0),
            new Tuple<double, int>(-50, 1),
            new Tuple<double, int>(0,   2),
            new Tuple<double, int>(50,  3),
            new Tuple<double, int>(72,  4),
        };

        public static Point playerPoint = new Point(500, 320);

        public static int totalCount = 0;
        public static int getScore = 0;
    }
}
