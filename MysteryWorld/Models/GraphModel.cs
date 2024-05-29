using System;
using System.Collections.Generic;
using System.Linq;

namespace MysteryWorld.Models
{
    public sealed class GraphModel
    {
        private readonly List<PointModel> points;
        internal readonly List<EdgeModel> edges;
        private double minX;
        private double minY;
        private double maxX;
        private double maxY;

        internal GraphModel(List<PointModel> points, List<EdgeModel> edges)
        {
            this.points = points;
            this.edges = edges;
            minX = double.PositiveInfinity;
            minY = double.PositiveInfinity;
            maxX = double.NegativeInfinity;
            maxY = double.NegativeInfinity;
        }

        private void CalculateBounds()
        {
            foreach (var point in points)
            {
                if (point.X < minX) { minX = point.X; }
                if (point.Y < minY) { minY = point.Y; }
                if (point.X > maxX) { maxX = point.X; }
                if (point.Y > maxY) { maxY = point.Y; }
            }
        }

        private TriangleModel CalculateSuperTriangle()
        {
            CalculateBounds();
            return new TriangleModel(new PointModel(minX - 1, minY - 1), new PointModel(minX + 2 * (maxX - minX) + 2, minY - 1), new PointModel(minX - 1, minY + 2 * (maxY - minY) + 2));
        }

        internal List<TriangleModel> BowyerWatson()
        {
            var triangulation = new List<TriangleModel>();
            var change = new List<TriangleModel>();
            var result = new List<TriangleModel>();
            var superTriangle = CalculateSuperTriangle();
            triangulation.Add(superTriangle);
            var superPoints = new List<PointModel>() { superTriangle.PointA, superTriangle.PointB, superTriangle.PointC };

            foreach (var point in points)
            {
                var badTriangles = triangulation.Where(triangle => triangle.InCircle(point)).ToList();

                var polygon = new List<EdgeModel>();
                foreach (var triangle in badTriangles)
                    foreach (var edge in triangle.Edges)
                    {
                        var addEdge = true;
                        foreach (var unused in badTriangles.Where(triangle2 => !triangle.Equals(triangle2)).Where(triangle2 => edge.IsInArray(triangle2.Edges)))
                            addEdge = false;
                        if (addEdge) polygon.Add(edge);
                    }

                foreach (var triangle in triangulation)
                    if (!triangle.IsInList(badTriangles))
                        change.Add(triangle);

                triangulation = change;
                change = new List<TriangleModel>();

                triangulation.AddRange(polygon.Select(edge => new TriangleModel(edge.p, edge.q, point)));
            }
            foreach (var triangle in triangulation)
            {
                var keep = true;
                foreach (var unused in superPoints.Where(spp => spp.IsInArray(triangle.Points)))
                    keep = false;

                if (keep) result.Add(triangle);
            }
            return result;
        }

        internal static GraphModel GetListToGraph(List<TriangleModel> bwList)
        {
            var points = new List<PointModel>();
            var edges = new List<EdgeModel>();
            foreach (var triangle in bwList)
            {
                foreach (var point in triangle.Points)
                    if (!point.IsInList(points)) points.Add(point);
                foreach (var edge in triangle.Edges)
                    if (!edge.IsInList(edges)) edges.Add(edge);
            }
            return new GraphModel(points, edges);
        }

        private double[,] ToAdjacencyMatrix()
        {
            var outMatrix = new double[points.Count, points.Count];
            var indexA = 0;
            foreach (var pointA in points)
            {
                var indexB = 0;
                foreach (var pointB in points)
                {
                    if (new EdgeModel(pointA, pointB).IsInList(edges) || new EdgeModel(pointB, pointA).IsInList(edges))
                        outMatrix[indexA, indexB] = pointA.Distance(pointB);
                    else outMatrix[indexA, indexB] = 0;
                    indexB++;
                }
                indexA++;
            }
            return outMatrix;
        }

        private List<EdgeModel> PrimUnconstrained()
        {
            var outEdges = new List<EdgeModel>();
            var graphMatrix = ToAdjacencyMatrix();
            var selected = new List<bool>();

            for (var a = 0; a < graphMatrix.GetLength(0); a++)
                selected.Add(false);

            selected[0] = true;
            var nrEdges = 0;
            while (nrEdges < points.Count - 1)
            {
                var minimum = double.PositiveInfinity;
                var x = 0;
                var y = 0;
                for (var i = 0; i < points.Count; i++)
                {
                    if (!selected[i]) continue;
                    for (var j = 0; j < points.Count; j++)
                    {
                        if (selected[j] || graphMatrix[i, j] == 0) continue;
                        if (minimum <= graphMatrix[i, j]) continue;

                        minimum = graphMatrix[i, j];
                        x = i;
                        y = j;
                    }
                }
                outEdges.Add(new EdgeModel(points[x], points[y]));
                selected[y] = true;
                nrEdges++;
            }
            return outEdges;
        }

        public GraphModel ExtendedSpanningTree(int addPercent, Random random)
        {
            if (addPercent is < 0 or > 100) return null;

            var spanEdges = PrimUnconstrained();
            foreach (var edge in edges.Where(edge => !edge.IsInList(spanEdges)).Where(i => random.Next(101) <= addPercent))
                spanEdges.Add(edge);
            return new GraphModel(points, spanEdges);
        }
    }
}