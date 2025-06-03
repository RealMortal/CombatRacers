
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace UnityEditor.Splines
{
    [System.Serializable]
    public class Intersection 
    {
        public List<JunctionInfo> junctions;

        public void AddJunction(int splineIndex, int knotIndex, Spline spline, List<BezierKnot> knots)
        {
            if (junctions == null)
                junctions = new List<JunctionInfo>();

            junctions.Add(new JunctionInfo(splineIndex, knotIndex, spline, knots));
        }

        public IEnumerable<JunctionInfo> GetJunctions() { return junctions; }


        [System.Serializable]
        public struct JunctionInfo
        {
            public int splineIndex;
            public int knotIndex;
            public Spline spline;
            public List<BezierKnot> knots; // list instead of one knot

            public JunctionInfo(int splineIndex, int knotIndex, Spline spline, List<BezierKnot> knots)
            {
                this.splineIndex = splineIndex;
                this.knotIndex = knotIndex;
                this.spline = spline;
                this.knots = knots;
            }
        }
    }


   
}
