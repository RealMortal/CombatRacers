using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

// Define an Editor Overlay to appear in the Scene View, named "Junction Builder"
[Overlay(typeof(SceneView), "Junction Builder", true)]
public class JunctionBuilderOverlay : Overlay
{
    // Label to show current spline element selection info
    private Label selectionInfoLabel;

    // Create the UI panel content shown in the overlay
    public override VisualElement CreatePanelContent()
    {
        // Root container for the overlay UI elements
        VisualElement root = new VisualElement();
        // Add padding around the edges
        root.style.paddingTop = 6;
        root.style.paddingBottom = 6;
        root.style.paddingLeft = 6;
        root.style.paddingRight = 6;

        // Header label to identify the overlay
        Label headerLabel = new Label("Junction Builder Overlay");
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(headerLabel);

        // Label to display information about the current selection
        selectionInfoLabel = new Label("No selection.");
        root.Add(selectionInfoLabel);

        // Button: Reset all junctions on the currently selected SplineRoad
        Button resetButton = new Button(() => {
            var road = Selection.activeObject.GetComponent<SplineRoad>();
            if (road != null)
            {
                road.ResetJunctions();
                Debug.Log("Junctions reset");
            }
            else
            {
                Debug.LogError("No SplineRoad component found on active selection!");
            }
        })
        {
            text = "Reset Junctions"
        };
        root.Add(resetButton);

        // Button: Build mesh for the currently selected SplineRoad
        Button buildMesh = new Button(() => {
            var road = Selection.activeObject.GetComponent<SplineRoad>();
            if (road != null)
            {
                road.BuildMesh();
                Debug.Log("Build Mesh");
            }
            else
            {
                Debug.LogError("No SplineRoad component found on active selection!");
            }
        })
        {
            text = "Build Mesh"
        };
        root.Add(buildMesh);

        // Button: Build a junction from the currently selected spline knots/elements
        Button buildJunctionButton = new Button(OnBuildJunction)
        {
            text = "Build Junction"
        };
        root.Add(buildJunctionButton);

        // Button: Manually refresh the selection info label
        Button refreshButton = new Button(UpdateSelectionInfo)
        {
            text = "Refresh Selection Info"
        };
        root.Add(refreshButton);

        // Subscribe to spline selection changes to update info automatically
        SplineSelection.changed += UpdateSelectionInfo;

        // Initial update of selection info
        UpdateSelectionInfo();

        return root;
    }

    // Called when "Build Junction" button is clicked
    private void OnBuildJunction()
    {
        Debug.Log("Build Junction button clicked.");

        // Get current spline elements selected in the editor
        List<SelectedSplineElementInfo> selection = SplineEditorToolbarExtension.GetSelection();

        Debug.Log($"Creating intersection with {selection.Count} selected elements");

        // Create a new Intersection to hold junction info
        Intersection intersection = new Intersection();

        // Loop through all selected spline elements (knots)
        foreach (SelectedSplineElementInfo info in selection)
        {
            // Get the spline container and specific spline from selection info
            SplineContainer container = info.target as SplineContainer;

            // Get the spline from the container using target index
            Spline spline = container.Splines[info.targetIndex];
            // Get the specific knot (point) on the spline by index
            BezierKnot knot = spline[info.knotIndex];

            // Add junction info for this knot and spline into the intersection
            intersection.AddJunction(info.targetIndex, info.knotIndex, spline, spline.Knots.ToList());
            
            Debug.Log($"Added junction for spline {info.targetIndex}, knot {info.knotIndex}, total junctions: {intersection.GetJunctions().Count()}");
        }

        // Send the constructed intersection with all junctions to the SplineRoad component of the selected object
        Debug.Log($"Sending intersection with {intersection.GetJunctions().Count()} junctions to SplineRoad");
        Selection.activeObject.GetComponent<SplineRoad>().AddJunction(intersection);
    }

    // Clear the selection info label text
    private void ClearSelectionInfo()
    {
        if (selectionInfoLabel != null)
        {
            selectionInfoLabel.text = "No selection.";
        }
    }

    // Updates the selection info label with current spline knot selections
    private void UpdateSelectionInfo()
    {
        if (selectionInfoLabel == null)
            return;

        // Retrieve the list of selected spline elements
        List<SelectedSplineElementInfo> infos = SplineEditorToolbarExtension.GetSelection();

        // Show message if nothing is selected
        if (infos.Count == 0)
        {
            selectionInfoLabel.text = "No spline elements selected.";
            return;
        }

        // Display detailed info about selected spline knots
        selectionInfoLabel.text = "Selected Elements:\n";
        foreach (SelectedSplineElementInfo element in infos)
        {
            selectionInfoLabel.text += $"- Spline Index: {element.targetIndex}, Knot Index: {element.knotIndex}\n";
        }
    }

    // Cleanup event subscription when overlay is destroyed or editor shuts down
    public override void OnWillBeDestroyed()
    {
        base.OnWillBeDestroyed();
        // Unsubscribe from selection changed event to prevent memory leaks
        SplineSelection.changed -= UpdateSelectionInfo;
    }
}
