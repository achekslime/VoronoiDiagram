namespace VoronoiDiagramLib
{
    public class SiteEvent : Event
    {
        public Site site;
        public SiteEvent(Point point) : base(point)
        {
            this.site = new Site(point);
        }
    }
}