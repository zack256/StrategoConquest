using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightNode : MonoBehaviour
{
    public Material[] lockedMaterial = new Material[3];
    public Material[] unbeatenMaterial = new Material[3];
    public Material[] beatenMaterial = new Material[3];

    //private Material defaultMaterial;

    private int accessLevel;

    void SetMaterial (Material mat) {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public void ResetMaterial () {
        MouseOverNode(false, false);
    }

    public void MouseOverNode (bool mouseDown, bool mouseOver = true) {
        int idx = 0;
        if (mouseDown) {
            idx = 2;
        } else if (mouseOver) {
            idx = 1;
        }
        if (accessLevel == 0) {
            SetMaterial(lockedMaterial[idx]);
        } else if (accessLevel == 1) {
            SetMaterial(unbeatenMaterial[idx]);
        } else {
            SetMaterial(beatenMaterial[idx]);
        }
    }

    public void SetAccessLevel (GameLevel gl) {
        accessLevel = gl.GetAccess();
    }

    void Start () {
        ResetMaterial();
    }
}
