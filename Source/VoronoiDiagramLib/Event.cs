using System;

namespace VoronoiDiagramLib
{
    public class Event : IComparable<Event>
    {
        public Point eventPoint;

        protected Event(Point point)
        {
            eventPoint = point;
        }

        public int CompareTo(Event other)
        {
            return eventPoint.CompareTo(other.eventPoint);
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}