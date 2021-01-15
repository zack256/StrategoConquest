using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightTile : MonoBehaviour
{
    public Material defaultMaterial;
    public Material mouseOverMaterial;
    public Material mouseClickMaterial;

    public void MouseHover () {
        ChangeColor(mouseOverMaterial);
    }

    public void MouseClicking () {
        ChangeColor(mouseClickMaterial);
    }

    public void ResetColor () {
        ChangeColor(defaultMaterial);
    }

    void ChangeColor (Material mat) {
        gameObject.GetComponent<Renderer>().material = mat;
    }
}
