/*
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using NUnit.Framework;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour
{
    

    public SplineContainer m_splineContainer;
    [SerializeField] private int m_splineIndex;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float m_time;
    [SerializeField] private float m_width = 3f; // half-width of road
    float3 position;
    float3 forward;
    float3 upVector;


    public float GetKnotTime()
    {
        return m_time;
    }

    private void Update()
    {
       
        m_splineContainer.Evaluate(m_splineIndex, m_time, out position, out forward, out upVector);

        // Calculate perpendicular direction (right vector)
        float3 right = math.normalize(math.cross(forward, upVector));

        float3 p1 = position + (right * m_width);   // Right edge of the road
        float3 p2 = position - (right * m_width);   // Left edge of the road

        // Optional: draw debug lines
        Debug.DrawLine(position, p1, Color.green);
        Debug.DrawLine(position, p2, Color.red);
    }


    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        
        Handles.color = Color.green;
        Handles.SphereHandleCap(0, position, Quaternion.identity, 1f, EventType.Repaint);
    #endif
    }

    public void SampleSplineWidth(int splineIndex, float t, float width, out Vector3 p1, out Vector3 p2)
    {
        m_splineContainer.Evaluate(splineIndex, t, out float3 position, out float3 forward, out float3 up);
        float3 right = math.normalize(math.cross(forward, up));
        float3 offset = right * width * 0.5f;

        p1 =position + offset;
        p2 = position - offset;
    }





}
*/