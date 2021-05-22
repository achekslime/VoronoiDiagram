using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using PriorityQueue;
namespace VoronoiDiagramLib
{
    public class VoronoiDiagram
    {
        // Список вершин диаграммы Вороного.
        private List<Vertex> Vertices;
        
        //Список граней диаграмы Вороного.
        private List<Site> Sites;
      
        // Спиоск ребер диаграмы Вороного.
        public List<HalfEdge> Edges;
        
        public VoronoiDiagram()
        {
            Vertices = new List<Vertex>();
            Sites = new List<Site>();
            Edges = new List<HalfEdge>();
        }


        public Vertex CreateVertex(Point curPoint)
        {
            Vertex newVert = new Vertex(curPoint);
            Vertices.Add(newVert);
            return newVert;
        }
        public HalfEdge CreateHalfEdge(Site curSite)
        {
            HalfEdge newEdge = new HalfEdge(curSite);
            if (curSite.outerEdge == null)
                curSite.outerEdge = newEdge;
            Edges.Add(newEdge);
            return newEdge;
        }
        
    }
}