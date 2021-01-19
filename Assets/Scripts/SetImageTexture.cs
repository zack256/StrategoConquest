using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SetImageTexture : MonoBehaviour
{

    private Material mat;
    private Texture2D tex;

    void SaveTexture () {
        mat = gameObject.GetComponent<Renderer>().material;
        tex = mat.mainTexture as Texture2D;
    }

    void SetTexture () {
        mat.mainTexture = tex;
        gameObject.GetComponent<Renderer>().material = mat;
    }

    void Start()
    {
        SaveTexture();
    }

    void Update()
    {
        SetTexture();   // needed every frame? (maybe change to onwillrenderobj)
    }
}
