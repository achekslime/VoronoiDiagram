namespace VoronoiDiagramLib
{
    public class HalfEdge
    {
        public Vertex origin;
        public Vertex destionation;
        public HalfEdge twin;
        private Site face;
        public HalfEdge next;
        public HalfEdge prev;

        public HalfEdge(Site curSite)
        {
            face = curSite;
        }
    }
}