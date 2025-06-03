using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Splines
{
    // Static helper class for interacting with the spline selection in the editor
    public static class SplineEditorToolbarExtension
    {
        // Returns true if there is an active spline selection, false otherwise
        public static bool HasSelection()
        {
            return SplineSelection.HasActiveSplineSelection();
        }

        // Retrieves the currently selected spline elements as a list of SelectedSplineElementInfo structs
        public static List<SelectedSplineElementInfo> GetSelection()
        {
            // Get the current selection list of selectable spline elements from the editor
            List<SelectableSplineElement> elements = SplineSelection.selection;

            // Prepare a new list to hold the processed selection info
            List<SelectedSplineElementInfo> infos = new List<SelectedSplineElementInfo>();

            // Iterate over each selectable spline element and convert it into SelectedSplineElementInfo
            foreach (SelectableSplineElement element in elements)
            {
                infos.Add(new SelectedSplineElementInfo(element.target, element.targetIndex, element.knotIndex));
            }

            return infos;
        }
    }

    // Struct representing the essential info about a selected spline element in the editor
    public struct SelectedSplineElementInfo
    {
        public Object target;      // The Unity Object that owns the spline (usually SplineContainer)
        public int targetIndex;    // Index of the spline within the container
        public int knotIndex;      // Index of the knot within the spline

        // Constructor to initialize the selected element info fields
        public SelectedSplineElementInfo(Object target, int targetIndex, int knotIndex)
        {
            this.target = target;
            this.targetIndex = targetIndex;
            this.knotIndex = knotIndex;
        }
    }
}
