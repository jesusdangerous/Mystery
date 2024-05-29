using System;
using System.Collections.Generic;

namespace MysteryWorld.Models
{
    public sealed class EdgeModel : IEquatable<EdgeModel>
    {
        public readonly PointModel p;
        public readonly PointModel q;

        internal EdgeModel(PointModel p, PointModel q)
        {
            this.p = p;
            this.q = q;
        }

        public bool Equals(EdgeModel other)
        {
            if (other != null)
                return p.Equals(other.p) && q.Equals(other.q) || q.Equals(other.p) && p.Equals(other.q);
            return false;
        }

        public bool IsInList(List<EdgeModel> edges)
        {
            foreach (var edge in edges)
                if (new EdgeModel(p, q).Equals(edge) || Equals(edge)) return true;
            return false;
        }

        internal bool IsInArray(IEnumerable<EdgeModel> edges)
        {
            foreach (var edge in edges)
                if (Equals(edge) || Equals(new EdgeModel(edge.q, edge.p))) return true;
            return false;
        }
    }
}