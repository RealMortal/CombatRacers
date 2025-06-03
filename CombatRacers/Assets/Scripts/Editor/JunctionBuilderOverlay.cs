/*
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
[Overlay(typeof(SceneView), "Junction Builder", true)]
public class JunctionBuilderOverlay : Overlay
{


    private Label selectionInfoLabel;

    public override VisualElement CreatePanelContent()
    {
        // Root container
        VisualElement root = new VisualElement();
        root.style.paddingTop = 6;
        root.style.paddingBottom = 6;
        root.style.paddingLeft = 6;
        root.style.paddingRight = 6;

        // Header label
        Label headerLabel = new Label("Junction Builder Overlay");
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(headerLabel);

        // Selection info label
        selectionInfoLabel = new Label("No selection.");
        root.Add(selectionInfoLabel);

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

        Button buildJunctionButton = new Button(OnBuildJunction)
        {
            text = "Build Junction"
        };
        root.Add(buildJunctionButton);

        // Refresh button for manual update
        Button refreshButton = new Button(UpdateSelectionInfo)
        {
            text = "Refresh Selection Info"
        };
        root.Add(refreshButton);

        // Subscribe to selection change
        SplineSelection.changed += UpdateSelectionInfo;

        // Initial update
        UpdateSelectionInfo();

        return root;
    }


    private void OnBuildJunction()
    {
        Debug.Log("Build Junction button clicked.");

        List<SelectedSplineElementInfo> selection = SplineEditorToolbarExtension.GetSelection();

        Debug.Log($"Creating intersection with {selection.Count} selected elements");

        Intersection intersection = new Intersection();

        foreach (SelectedSplineElementInfo info in selection)
        {
            SplineContainer container = info.target as SplineContainer;

            Spline spline = container.Splines[info.targetIndex];
            BezierKnot knot = spline[info.knotIndex];

            intersection.AddJunction(info.targetIndex, info.knotIndex, spline, spline.Knots.ToList());
            Debug.Log($"Added junction for spline {info.targetIndex}, knot {info.knotIndex}, total junctions: {intersection.GetJunctions().Count()}");

        }
        Debug.Log($"Sending intersection with {intersection.GetJunctions().Count()} junctions to SplineRoad");

        Selection.activeObject.GetComponent<SplineRoad>().AddJunction(intersection);
    }


    private void ClearSelectionInfo()
    {
        if (selectionInfoLabel != null)
        {
            selectionInfoLabel.text = "No selection.";
        }
    }

    private void UpdateSelectionInfo()
    {
        if (selectionInfoLabel == null)
            return;

        List<SelectedSplineElementInfo> infos = SplineEditorToolbarExtension.GetSelection();

        if (infos.Count == 0)
        {
            selectionInfoLabel.text = "No spline elements selected.";
            return;
        }

        selectionInfoLabel.text = "Selected Elements:\n";
        foreach (SelectedSplineElementInfo element in infos)
        {
            selectionInfoLabel.text += $"- Spline Index: {element.targetIndex}, Knot Index: {element.knotIndex}\n";
        }
    }

    // Clean up callback when overlay is removed or editor shuts down
    public override void OnWillBeDestroyed()
    {
        base.OnWillBeDestroyed();
        SplineSelection.changed -= UpdateSelectionInfo;
    }
}
*/