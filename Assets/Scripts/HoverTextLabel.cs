using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTextLabel : MonoBehaviour
{

    Vector2 GetRectTransformWorldDims (RectTransform rt) {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        Vector3 bottomLeft = v[0];
        Vector3 topRight = v[2];
        float width = Math.Abs(topRight.x - bottomLeft.x);
        float height = Math.Abs(topRight.y - bottomLeft.y);
        return new Vector2(width, height);
    }

    void WrapTextWithBackground () {
        GameObject backgroundObj = gameObject.transform.GetChild(0).gameObject;
        GameObject textObj = gameObject.transform.GetChild(1).GetChild(0).gameObject;
        RectTransform rt = textObj.GetComponent<RectTransform>();
        if (rt.rect.width == 0) {  // not finished loading?
            return;
        }
        Vector3 backgroundDims = backgroundObj.GetComponent<Renderer>().bounds.size;
        Vector2 rectDims = GetRectTransformWorldDims(rt);
        float fracX = ((float) rectDims.x) / backgroundDims.x;
        float fracY = ((float) rectDims.y) / backgroundDims.y;
        backgroundObj.transform.localScale = new Vector3(backgroundObj.transform.localScale.x * fracX, backgroundObj.transform.localScale.y * fracY, backgroundObj.transform.localScale.z);
    }

    void Start()
    {
        
    }

    void Update()
    {
        WrapTextWithBackground();
    }
}
