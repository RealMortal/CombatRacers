#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour
{
    public SplineContainer m_splineContainer;

    [SerializeField] private int m_splineIndex;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float m_time;
    
    [Tooltip("Half-width of the road")]
    [SerializeField] private float m_halfWidth = 3f; 

    private float3 position;
    private float3 forward;
    private float3 upVector;

    public float GetKnotTime()
    {
        return m_time;
    }

    private void Update()
    {
        if (m_splineContainer == null) return;

        // Evaluate spline at current index and time
        m_splineContainer.Evaluate(m_splineIndex, m_time, out position, out forward, out upVector);

        // Calculate perpendicular direction (right vector)
        float3 right = math.normalize(math.cross(forward, upVector));

        float3 p1 = position + (right * m_halfWidth);   // Right edge of the road
        float3 p2 = position - (right * m_halfWidth);   // Left edge of the road

        // Draw debug lines to visualize road edges
        Debug.DrawLine(position, p1, Color.green);
        Debug.DrawLine(position, p2, Color.red);
    }

    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        if (m_splineContainer == null) return;

        // Evaluate spline so we have the position for gizmos
        m_splineContainer.Evaluate(m_splineIndex, m_time, out float3 gizmoPos, out _, out _);

        Handles.color = Color.green;
        Handles.SphereHandleCap(0, gizmoPos, Quaternion.identity, 1f, EventType.Repaint);
    #endif
    }

    /// <summary>
    /// Samples points offset by half the width to each side of the spline at given t
    /// </summary>
    /// <param name="splineIndex"></param>
    /// <param name="t"></param>
    /// <param name="width">Full width of the road</param>
    /// <param name="p1">Right edge point output</param>
    /// <param name="p2">Left edge point output</param>
    public void SampleSplineWidth(int splineIndex, float t, float width, out Vector3 p1, out Vector3 p2)
    {
        if (m_splineContainer == null)
        {
            p1 = p2 = Vector3.zero;
            return;
        }

        m_splineContainer.Evaluate(splineIndex, t, out float3 position, out float3 forward, out float3 up);
        float3 right = math.normalize(math.cross(forward, up));
        float3 offset = right * width * 0.5f;

        p1 = position + offset;
        p2 = position - offset;
    }
}
