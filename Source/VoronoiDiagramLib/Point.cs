using System;

namespace VoronoiDiagramLib
{
    public class Point : IComparable<Point>
    {
        public double x { get; private set; }
        public  double y { get; private set; }

        
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Point(){}
        

        public int CompareTo(Point other)
        {
            return y.CompareTo(other.y) == 0 ? other.x.CompareTo(x) : y.CompareTo(other.y);
        }
    }
}