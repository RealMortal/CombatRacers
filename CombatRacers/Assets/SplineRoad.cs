/*
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEditor.Splines.Intersection;


[ExecuteInEditMode()]
public class SplineRoad : MonoBehaviour
{
    [SerializeField] private int resolution = 50;
    [SerializeField] private float m_width = 1;
    [SerializeField] private float uvOffset = 1;
    [SerializeField] private SplineSampler m_splineSampler;
    [SerializeField] private MeshFilter m_meshFilter;
    private List<Vector3> m_vertsP1;
    private List<Vector3> m_vertsP2;
    [SerializeField] private MeshCollider m_meshCollider;
    private bool m_needsColliderUpdate = false;

    [SerializeField]
    private List<JunctionInfo> m_junctions = new List<JunctionInfo>();
    [SerializeField]
    private List<Intersection> intersections = new List<Intersection>();

    public void AddJunction(Intersection intersection)
    {

        foreach (JunctionInfo junction in intersection.GetJunctions())
        {
            m_junctions.Add(junction);
        }

        intersections.Add(intersection);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetVerts();

    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            GetVerts();
            BuildMesh();

            if (m_needsColliderUpdate)
            {
                UpdateCollider();
                m_needsColliderUpdate = false;
            }
        }

    }

    private void GetVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        float step = 1f / resolution;
        Vector3 p1, p2;

        int numSplines = m_splineSampler.m_splineContainer.Splines.Count;

        for (int splineIndex = 0; splineIndex < numSplines; splineIndex++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                float t = j * step;
                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out p1, out p2);

                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }

        }
    }




    private void OnDrawGizmos()
    {
        if (m_vertsP1 == null || m_vertsP2 == null)
            return;

        int count = Mathf.Min(m_vertsP1.Count, m_vertsP2.Count);

        for (int i = 0; i < count; i++)
        {

            Vector3 p1 = m_vertsP1[i]; // Right
            Vector3 p2 = m_vertsP2[i]; // Left

            // Draw spheres
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(p1, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p2, 0.2f);

            // Draw line between them
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p1, p2);
        }
        if (m_junctions != null)
        {
            Gizmos.color = Color.yellow;

            foreach (var junction in m_junctions)
            {
                float t = m_splineSampler.GetKnotTime(); // You may need to implement this if you don't have a time lookup yet

                m_splineSampler.SampleSplineWidth(junction.splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);

                Gizmos.DrawWireSphere(p1, 0.2f); // Right edge
                Gizmos.DrawWireSphere(p2, 0.2f); // Left edge
            }
        }
        // In OnDrawGizmos, add special visualization for intersection points
        Gizmos.color = Color.yellow;
        foreach (var intersection in intersections)
        {
            List<Vector3> debugPoints = new List<Vector3>();
            Vector3 center = Vector3.zero;
            int pointCount = 0;

            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = (junction.knotIndex == 0) ? 0f : 1f;

                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);
                debugPoints.Add(p1);
                debugPoints.Add(p2);
                center += p1 + p2;
                pointCount += 2;
            }

            center /= pointCount;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(center, 0.3f);

            Gizmos.color = Color.magenta;
            foreach (var point in debugPoints)
            {
                Gizmos.DrawSphere(point, 0.2f);
                Gizmos.DrawLine(center, point);
            }
        }
    }
    public void ResetJunctions()
    {
        // Clear both junction and intersection lists
        if (m_junctions != null)
        {
            m_junctions.Clear();
        }
        else
        {
            m_junctions = new List<JunctionInfo>();
        }

        if (intersections != null)
        {
            intersections.Clear();
        }
        else
        {
            intersections = new List<Intersection>();
        }

        Debug.Log("All junctions have been reset");

        // Rebuild the mesh without junctions
        Rebuild();
    }
    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    public void BuildMesh()
    {

        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<int> tris = new List<int>();

        int vertsPerSpline = resolution + 1;
        int splineCount = m_splineSampler.m_splineContainer.Splines.Count;



        for (int splineIndex = 0; splineIndex < splineCount; splineIndex++)
        {
            int baseIndex = splineIndex * vertsPerSpline;
            for (int i = 0; i < resolution; i++)
            {
                int currentIndex = baseIndex + i;
                if (currentIndex + 1 >= m_vertsP1.Count || currentIndex + 1 >= m_vertsP2.Count)
                {

                    continue;
                }

                Vector3 p1 = m_vertsP1[currentIndex];
                Vector3 p2 = m_vertsP2[currentIndex];
                Vector3 p3 = m_vertsP1[currentIndex + 1];
                Vector3 p4 = m_vertsP2[currentIndex + 1];

                int vertIndex = verts.Count;
                verts.AddRange(new List<Vector3> {
                transform.InverseTransformPoint(p1),
                transform.InverseTransformPoint(p2),
                transform.InverseTransformPoint(p3),
                transform.InverseTransformPoint(p4)
                });


                tris.Add(vertIndex + 0);
                tris.Add(vertIndex + 2);
                tris.Add(vertIndex + 3);

                tris.Add(vertIndex + 3);
                tris.Add(vertIndex + 1);
                tris.Add(vertIndex + 0);


                float distance = Vector3.Distance(p1, p3) / 4f;
                float uvDistance = uvOffset + distance;
                uvs.AddRange(new List<Vector2>
                {
                    new Vector2(uvOffset,0),new Vector2(uvOffset,1),new Vector2(uvDistance,0),new Vector2(uvDistance,1)
                });

                uvOffset += distance;
            }
        }

        // Junction Mesh




        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];

            List<Vector3> points = new List<Vector3>();
            Vector3 center = Vector3.zero;
            int pointCount = 0;

            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = (junction.knotIndex == 0) ? 0f : 1f;

                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);

                points.Add(p1);
                points.Add(p2);

                center += p1 + p2;
                pointCount += 2;


            }

            if (points.Count < 3)
            {

                continue;
            }

            center /= pointCount;
            Vector3 localCenter = transform.InverseTransformPoint(center);
            Vector3 centerXZ = new Vector3(localCenter.x, 0, localCenter.z);

            points.Sort((a, b) => {
                Vector3 aLocal = transform.InverseTransformPoint(a);
                Vector3 bLocal = transform.InverseTransformPoint(b);
                Vector3 aXZ = new Vector3(aLocal.x, 0, aLocal.z) - centerXZ;
                Vector3 bXZ = new Vector3(bLocal.x, 0, bLocal.z) - centerXZ;

                float angleA = Mathf.Atan2(aXZ.x, aXZ.z);
                float angleB = Mathf.Atan2(bXZ.x, bXZ.z);

                return angleA.CompareTo(angleB);
            });

            int centerVertIndex = verts.Count;
            verts.Add(localCenter);

            List<int> pointIndices = new List<int>();
            foreach (Vector3 point in points)
            {
                pointIndices.Add(verts.Count);
                verts.Add(transform.InverseTransformPoint(point));
            }

            for (int j = 0; j < pointIndices.Count; j++)
            {
                int nextIdx = (j + 1) % pointIndices.Count;
                tris.Add(centerVertIndex);
                tris.Add(pointIndices[j]);
                tris.Add(pointIndices[nextIdx]);
            }


        }

        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.SetUVs(0, uvs);
        m_meshFilter.mesh = m;
        m_needsColliderUpdate = true;


    }

    private void UpdateCollider()
    {
        if (m_meshCollider != null)
        {
            DestroyImmediate(m_meshCollider.sharedMesh);
            m_meshCollider.sharedMesh = m_meshFilter.sharedMesh;
        }
    }
    private void Rebuild()
    {
        GetVerts();
        BuildMesh();
    }
}
*/