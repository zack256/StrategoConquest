using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangleMeshes : MonoBehaviour
{

    public bool isTop;

    void CreateTopTriangleMesh () {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        //Vector3 center = gameObject.transform.position;
        //Vector3 dims = gameObject.GetComponent<Renderer>().bounds.size;
        
        Vector3[] newVertices = new Vector3[3];
        //float bHeight = (float) Math.Sqrt(Math.Pow(dims.x, 2) - Math.Pow(dims.x / 2, 2));
        newVertices[0] = new Vector3(-1 / 2f, -1 / 2f, 0);
        //newVertices[1] = new Vector3(0, 1 / 2f, 0);
        newVertices[1] = new Vector3(0, -1 / 2f + (float) Math.Sqrt(3) / 2f, 0);
        newVertices[2] = new Vector3(1 / 2f, -1 / 2f, 0);

        int[] newTriangles = new int [] { 0, 1, 2 };

        Vector2[] newUVs = new Vector2[3];
        for (int i = 0; i < 3; i++) {
            newUVs[i] = new Vector2(newVertices[i].x, newVertices[i].y);
        }

        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUVs;
        mesh.RecalculateNormals();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

    }

    void CreateBottomTriangleMesh () {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        
        Vector3[] newVertices = new Vector3[3];
        newVertices[0] = new Vector3(-1 / 2f, 1 / 2f, 0);
        newVertices[1] = new Vector3(0, 1 / 2f - (float) Math.Sqrt(3) / 2f, 0);
        newVertices[2] = new Vector3(1 / 2f, 1 / 2f, 0);

        int[] newTriangles = new int [] { 0, 2, 1 };

        Vector2[] newUVs = new Vector2[3];
        for (int i = 0; i < 3; i++) {
            newUVs[i] = new Vector2(newVertices[i].x, newVertices[i].y);
        }

        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUVs;
        mesh.RecalculateNormals();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    void Start () {
        if (isTop) {
            CreateTopTriangleMesh();
        } else {
            CreateBottomTriangleMesh();
        }
    }


}
