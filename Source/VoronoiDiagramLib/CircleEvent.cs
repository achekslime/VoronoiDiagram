
namespace VoronoiDiagramLib
{
    public class CircleEvent : Event
    {
        public Arc baseArc;
        public Point circlePoint;
        public CircleEvent(Point point, Point circlePoint, ref Arc baseArc) : base(point)
        {
            this.baseArc = baseArc;
            this.circlePoint = circlePoint;
        }
    }
}