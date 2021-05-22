using System;

namespace VoronoiDiagramLib
{
    public class Site : IComparable<Site>
    {
        public HalfEdge outerEdge;
        public Point basePoint;

        public Site(Point point)
        {
            basePoint = point;
        }
        

        public int CompareTo(Site other)
        {
            return basePoint.CompareTo(other.basePoint);
        }
    }
}