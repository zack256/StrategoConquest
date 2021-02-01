using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureScript : MonoBehaviour
{
    public Texture2D CreateTexture (string filePath) {
        byte[] imageData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);
        return tex;
    }
}
