using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshScript : MonoBehaviour
{
    
    Vector2[] CalcNewUVs (Vector3[] newVertices) {
        int numVertices = newVertices.Length;
        Vector2[] newUVs = new Vector2[numVertices];
        for (int i = 0; i < numVertices; i++) {
            newUVs[i] = new Vector2(newVertices[i].x, newVertices[i].y);
        }
        return newUVs;
    }

    public void RedoMesh (GameObject obj, Vector3[] newVertices, int[] newTriangles, bool includeCollider = true) {
        Vector2[] newUVs = CalcNewUVs(newVertices);
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUVs;
        mesh.RecalculateNormals();
        if (includeCollider) {
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
    }
}
