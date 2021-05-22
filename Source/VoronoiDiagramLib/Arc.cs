using VoronoiDiagramLib;
using PriorityQueue;

namespace VoronoiDiagramLib
{
    public class Arc
    {
        public enum Color
        {
            RED,
            BLACK
        }

        // BeachLine.
        public Arc next;
        public Arc prev;
        public Arc parent;
        public Arc left;
        public Arc right;
        public Color color;
        // Voronoi.
        public Site site;
        public HalfEdge leftHalfEdge;
        public HalfEdge rightHalfEdge;
        public Node<Event> circleEvent;


        public Arc(Site curSite)
        {
            site = curSite;
            color = Color.RED;
            left = new Arc();
            right = new Arc();
            parent = new Arc();
            prev = new Arc();
            next = new Arc();
        }

        public Arc()
        {
            color = Color.BLACK;
        }
    }
}