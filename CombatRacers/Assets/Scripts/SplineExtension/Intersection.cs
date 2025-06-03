using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace UnityEditor.Splines
{
    // Serializable class representing an intersection composed of multiple junctions
    [System.Serializable]
    public class Intersection 
    {
        // List of junctions involved in this intersection
        public List<JunctionInfo> junctions;

        // Adds a new junction to the intersection with the provided spline and knot info
        public void AddJunction(int splineIndex, int knotIndex, Spline spline, List<BezierKnot> knots)
        {
            // Initialize junctions list if null
            if (junctions == null)
                junctions = new List<JunctionInfo>();

            // Add a new JunctionInfo to the list
            junctions.Add(new JunctionInfo(splineIndex, knotIndex, spline, knots));
        }

        // Returns an enumerable of all junctions in the intersection
        public IEnumerable<JunctionInfo> GetJunctions() 
        { 
            return junctions; 
        }

        // Serializable struct to hold detailed info about a junction point
        [System.Serializable]
        public struct JunctionInfo
        {
            public int splineIndex;       // Index of the spline in a collection
            public int knotIndex;         // Index of the knot within the spline
            public Spline spline;         // Reference to the spline itself
            public List<BezierKnot> knots; // List of BezierKnots associated with this junction

            // Constructor to initialize all fields
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
