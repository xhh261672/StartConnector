using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    using System.Windows;
    class GameData
    {
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
            new Point(20, 8),
            new Point(21, 24),
            new Point(0, 15),
            new Point(-21, 24),
            new Point(-20, 8)

        };

        public static Point startPoint = new Point(0, 0);
        public static int totalCount = 0;
        public static int getScore = 0;
    }
}
