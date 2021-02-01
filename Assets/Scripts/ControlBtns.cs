﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBtns : MonoBehaviour
{

    public bool isCheck;
    public GameObject quadImgTemplate;
    public GameObject scriptMaster;
    public Material checkMaterial;
    public Material resetMaterial;

    private Material defaultMaterial;

    void InitImages () {
        string path = Application.dataPath + "/Files/Images/Control/";
        if (isCheck) {
            path += "check.png";
        } else {
            path += "close.png";
        }
        Texture2D tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(path);
        GameObject newQuadImg = Instantiate(quadImgTemplate, gameObject.transform.position, Quaternion.identity);
        newQuadImg.transform.parent = gameObject.transform;
        newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public void Highlight () {
        if (isCheck) {
            gameObject.GetComponent<Renderer>().material = checkMaterial;
        } else {
            gameObject.GetComponent<Renderer>().material = resetMaterial;
        }
    }

    public void ResetHighlight () {
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = gameObject.GetComponent<Renderer>().material;
        InitImages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
