using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SetImageTexture : MonoBehaviour
{

    public Material highlightMaterial;

    private Material defaultMat;
    private Texture2D tex;

    private int matMode;

    public void ToggleMatMode(int newMode) {
        matMode = newMode;
    }

    void SaveTexture () {
        defaultMat = gameObject.GetComponent<Renderer>().material;
        tex = defaultMat.mainTexture as Texture2D;
        matMode = 0;
    }

    void SetTexture () {
        Material mat;
        if (matMode == 0) {
            mat = defaultMat;
        } else {
            mat = highlightMaterial;
        }
        //mat.mainTexture = tex;
        gameObject.GetComponent<Renderer>().material = mat;
        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
    }

    void Start()
    {
        SaveTexture();
    }

    void Update()
    {
        SetTexture();   // needed every frame? (maybe change to onwillrenderobj) (maybe material resets every frame?)
    }
}
