using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMesh : MonoBehaviour
{

    public GameObject scriptMaster;

    public int sides;   // will make a regular [sides]-gon

    int[] CreateCircleMesh (Vector3[] vertices) {
        int[] triangles = new int[3 * sides];
        float sideAngle = (float) (2 * Math.PI / sides);
        vertices[0] = new Vector3(0, 0, 0);    // center
        float topAngle = (float) (Math.PI / 2);   // starts from top, going counterclockwise.
        float angle;
        for (int i = 0; i < sides; i++) {
            angle = topAngle + i * sideAngle;
            vertices[i + 1] = new Vector3((float) Math.Cos(angle), (float) Math.Sin(angle), 0); // radius will by default be half the side length of the quad (square)
        }
        for (int i = 0; i < sides; i++) {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 2;
            triangles[3 * i + 2] = i + 1;
        }
        triangles[3 * sides - 2] = 1;
        return triangles;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] vertices = new Vector3[sides + 1];
        int[] triangles;
        triangles = CreateCircleMesh(vertices);
        scriptMaster.GetComponent<MeshScript>().RedoMesh(gameObject, vertices, triangles);
    } 
}
