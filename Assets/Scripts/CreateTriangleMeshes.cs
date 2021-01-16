using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangleMeshes : MonoBehaviour
{

    public bool isTop;
    public GameObject meshMaster;

    int[] CreateTopTriangleMesh (Vector3[] nvs) {
        
        nvs[0] = new Vector3(-1 / 2f, -1 / 2f, 0);
        nvs[1] = new Vector3(0, -1 / 2f + (float) Math.Sqrt(3) / 2f, 0);
        nvs[2] = new Vector3(1 / 2f, -1 / 2f, 0);

        return new int[] {0, 1, 2};
    }    

    int[] CreateBottomTriangleMesh (Vector3[] nvs) {
        
        nvs[0] = new Vector3(-1 / 2f, 1 / 2f, 0);
        nvs[1] = new Vector3(0, 1 / 2f - (float) Math.Sqrt(3) / 2f, 0);
        nvs[2] = new Vector3(1 / 2f, 1 / 2f, 0);

        return new int[] {0, 2, 1};
    }    

    void Start () {
        Vector3[] newVertices = new Vector3[3];
        int[] newTriangles;
        if (isTop) {
            newTriangles = CreateTopTriangleMesh(newVertices);
        } else {
            newTriangles = CreateBottomTriangleMesh(newVertices);
        }
        meshMaster.GetComponent<MeshScript>().RedoMesh(gameObject, newVertices, newTriangles);
    }

}
