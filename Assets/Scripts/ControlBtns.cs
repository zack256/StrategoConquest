using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBtns : MonoBehaviour
{

    public string btnName;
    public GameObject quadImgTemplate;
    public GameObject scriptMaster;
    public Material checkMaterial;
    public Material resetMaterial;

    private Material defaultMaterial;
    private bool notReset;

    void InitImages () {
        string path = Application.dataPath + "/Files/Images/Control/" + btnName + ".png";
        Texture2D tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(path);
        GameObject newQuadImg = Instantiate(quadImgTemplate, gameObject.transform.position, Quaternion.identity);
        scriptMaster.GetComponent<ValueLabel>().RemoveLabel(newQuadImg);
        newQuadImg.transform.parent = gameObject.transform;
        newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public void Highlight (bool opposite = false) {
        if (notReset ^ opposite) {
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
        notReset = btnName != "close";
        defaultMaterial = gameObject.GetComponent<Renderer>().material;
        InitImages();
    }
}
