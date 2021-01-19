using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SetImageTexture : MonoBehaviour
{

    private Material mat;
    private Texture2D tex;

    void CreateTexture () {
        string assetsDir = Application.dataPath;
        string filePath = assetsDir + "/Files/Images/Pieces/Good/5.png";
        byte[] imageData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);
        mat = new Material(Shader.Find("Specular"));
    }

    void SetTexture () {
        mat.mainTexture = tex;
        gameObject.GetComponent<Renderer>().material = mat;
    }

    void Start()
    {
        CreateTexture();
    }

    void Update()
    {
        SetTexture();   // needed every frame? (maybe change to onwillrenderobj)
    }
}
