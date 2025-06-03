using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deform : MonoBehaviour
{
    [Range(0, 10)]
    public float deformRadius = 0.2f; // How close a vertex must be to the collision point to be deformed.



    [Range(0, 10)]
    public float maxDeform = 0.1f; // Maximum amount a vertex is allowed to move from its original position.


    [Range(0, 1)]
    public float damageFalloff = 1; // How much the deformation strength drops off with distance.


    [Range(0, 10)]
    public float damageMultiplier = 1; // Multiplies how much vertices get deformed.

    [Range(0, 100000)]
    public float minDamage = 1; // Minimum collision strength needed to cause deformation.

    public AudioClip[] collisionSounds; // List of sounds to randomly play when a collision happens.

    public MeshFilter filter; 
    public Rigidbody physics;
    public MeshCollider coll;
    private Mesh originalMesh;

    private Vector3[] startingVerticies; // Array to store the original, undeformed vertices.

    private Vector3[] meshVerticies; // Array to store and modify the deformed vertices.
    private Vector3[] collisionVertices; // Array to store and modify the deformed vertices.

    void Start()
    {
        // Clone the original mesh so we can reset to it later
        startingVerticies = filter.mesh.vertices;
        meshVerticies = filter.mesh.vertices;

        // Optional: Store a full copy of the original mesh
        originalMesh = Instantiate(filter.mesh); // <- new line

        coll.sharedMesh = filter.mesh; // ensure collider starts synced

    }

    void OnCollisionEnter(Collision collision)
    {
        // We decide how strong the collision was
        float collisionPower = collision.impulse.magnitude;

        // Only continue if the collision was powerful enough.
        if (collisionPower > minDamage)
        {
           
            // For every contact point where the object touched another surface
            foreach (ContactPoint point in collision.contacts)
            {
                // Check every vertex in the mesh.
                for (int i = 0; i < meshVerticies.Length; i++)
                {
                    Vector3 vertexPosition = meshVerticies[i]; // Current vertex position (local space).

                    // We get the collision point relative to the object's local space.
                    Vector3 pointPosition = transform.InverseTransformPoint(point.point);

                    // How far is the vertex from the collision point?
                    float distanceFromCollision = Vector3.Distance(vertexPosition, pointPosition);

                    // How far has the vertex already moved from its starting position?
                    float distanceFromOriginal = Vector3.Distance(startingVerticies[i], vertexPosition);

                    // We will only deform vertices that are:
                    // 1. Close enough to the collision point
                    // 2. Not already deformed too much
                    if (distanceFromCollision < deformRadius && distanceFromOriginal < maxDeform)
                    {
                        // Calculate how much the deformation should weaken with distance.
                        float falloff = 1 - (distanceFromCollision / deformRadius) * damageFalloff;

                        // Amount to deform on each axis (X, Y, Z).
                        float xDeform = pointPosition.x * falloff;
                        float yDeform = pointPosition.y * falloff;
                        float zDeform = pointPosition.z * falloff;

                        // We will clamp the deformation to make sure it doesn’t go past the maximum allowed deformation.
                        xDeform = Mathf.Clamp(xDeform, 0, maxDeform);
                        yDeform = Mathf.Clamp(yDeform, 0, maxDeform);
                        zDeform = Mathf.Clamp(zDeform, 0, maxDeform);


                        // Make a deformation vector from the X, Y, Z deformations.
                        Vector3 deform = new Vector3(xDeform, yDeform, zDeform);

                        // Move the vertex slightly inward by subtracting the deformation, multiplied by the damage multiplier.
                        meshVerticies[i] -= deform * damageMultiplier;
                    }
                }
            }

            // After deforming vertices, update the mesh to show the changes.
            UpdateMeshVerticies();
        }
    }

    void UpdateMeshVerticies()
    {
        // Apply the new vertex positions to the mesh.
        filter.mesh.vertices = meshVerticies;

        // Update the collider mesh so collision detection stays accurate.
        coll.sharedMesh = filter.mesh;
    }

    public void ResetDeformation()
    {
        // Deep copy original mesh data back
        meshVerticies = originalMesh.vertices;
        filter.mesh.vertices = meshVerticies;
        filter.mesh.normals = originalMesh.normals;
        filter.mesh.triangles = originalMesh.triangles;
        filter.mesh.uv = originalMesh.uv;

        // Update the collider
        coll.sharedMesh = null; // Force Unity to refresh
        coll.sharedMesh = filter.mesh;
    }

}
