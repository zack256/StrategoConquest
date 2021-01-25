using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBorderMesh : MonoBehaviour
{
    public GameObject scriptMaster;
    public float marginDecimal;

    int[] CreateBorderMesh (Vector3[] nvs) {
        
        // Margin is outside, not inside.

        nvs[0] = new Vector3(-0.5f, 0.5f, 0);
        nvs[1] = new Vector3(0.5f, 0.5f, 0);
        nvs[2] = new Vector3(0.5f, -0.5f, 0);
        nvs[3] = new Vector3(-0.5f, -0.5f, 0);
        nvs[4] = new Vector3(-0.5f - marginDecimal, 0.5f + marginDecimal, 0);
        nvs[5] = new Vector3(0.5f + marginDecimal, 0.5f + marginDecimal, 0);
        nvs[6] = new Vector3(0.5f + marginDecimal, -0.5f - marginDecimal, 0);
        nvs[7] = new Vector3(-0.5f - marginDecimal, -0.5f - marginDecimal, 0);

        //return new int[] {0, 5, 4, 0, 1, 5, 1, 6, 5, 1, 2, 6, 2, 7, 6, 2, 3, 7, 3, 4, 7, 3, 0, 4};
        return new int[] {0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3, 3, 7, 4, 3, 4, 0};
    }    

    void Start () {
        Vector3[] newVertices = new Vector3[8];
        int[] newTriangles;
        newTriangles = CreateBorderMesh(newVertices);
        scriptMaster.GetComponent<MeshScript>().RedoMesh(gameObject, newVertices, newTriangles);
    }
}
