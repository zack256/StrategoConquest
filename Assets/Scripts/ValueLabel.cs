using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueLabel : MonoBehaviour
{

    public float labelWidthPercentage;

    public void PositionLabel (GameObject imgQuad) {
        float labelWidthDecimal = labelWidthPercentage / 100f;
        GameObject labelQuad = imgQuad.transform.GetChild(0).gameObject;
        gameObject.GetComponent<ScaleScript>().ScaleGameObject(labelQuad, imgQuad);
        gameObject.GetComponent<ScaleScript>().ScaleByScalar(labelQuad, labelWidthDecimal);
        Vector3 imgQuadSize = imgQuad.GetComponent<Renderer>().bounds.size;
        Vector3 imgQuadPos = imgQuad.transform.position;
        Vector3 labelQuadSize = labelQuad.GetComponent<Renderer>().bounds.size;
        labelQuad.transform.position = new Vector3 (imgQuadPos.x - imgQuadSize.x / 2f + labelQuadSize.x / 2f, imgQuadPos.y + imgQuadSize.y / 2f - labelQuadSize.y / 2f, labelQuad.transform.position.z);
    }

    public void RenameLabel (GameObject imgQuad, string newLabel) {
        GameObject txt = imgQuad.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        txt.GetComponent<Text>().text = newLabel;
    }

    public void RemoveLabel (GameObject imgQuad) {
        GameObject.Destroy(imgQuad.transform.GetChild(0).gameObject);
    }
}
