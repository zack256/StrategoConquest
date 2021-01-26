using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight2D : MonoBehaviour
{
    public Material defaultMaterial;
    public Material mouseOverMaterial;
    public Material mouseClickMaterial;
    public Material mouseWillDropMaterial;

    public void MouseHover () {
        ChangeColor(mouseOverMaterial);
    }

    public void MouseClicking () {
        ChangeColor(mouseClickMaterial);
    }

    public void MouseWillDrop () {
        ChangeColor(mouseWillDropMaterial);
    }

    public void ResetColor () {
        ChangeColor(defaultMaterial);
    }

    void ChangeColor (Material mat) {
        gameObject.GetComponent<Renderer>().material = mat;
    }
}
