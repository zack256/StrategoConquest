using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void OnTimeUpdateHack (RectTransform rt) {
        // Updates the text layout dimensions before frames drawn, workaround. (???)
        MoveToPos(new Vector3(100, 0, 0));
        gameObject.SetActive(true);
        rt.ForceUpdateRectTransforms(); // forces update bc text might have changed.
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void WrapTextWithBackground () {
        GameObject backgroundObj = gameObject.transform.GetChild(0).gameObject;
        GameObject textObj = gameObject.transform.GetChild(1).GetChild(0).gameObject;
        RectTransform rt = textObj.GetComponent<RectTransform>();
        OnTimeUpdateHack(rt);

        //if (rt.rect.width == 0) {  // not finished loading?
            //return;
        //}
        Vector3 backgroundDims = backgroundObj.GetComponent<Renderer>().bounds.size;
        Vector2 rectDims = GetRectTransformWorldDims(rt);
        Debug.Log(rectDims.x + " , " + rectDims.y);
        float fracX = ((float) rectDims.x) / backgroundDims.x;
        float fracY = ((float) rectDims.y) / backgroundDims.y;
        backgroundObj.transform.localScale = new Vector3(backgroundObj.transform.localScale.x * fracX, backgroundObj.transform.localScale.y * fracY, backgroundObj.transform.localScale.z);
    }

    void ChangeText (string newText) {
        GameObject textObj = gameObject.transform.GetChild(1).GetChild(0).gameObject;
        textObj.GetComponent<Text>().text = newText;
    }

    void MoveToPos (Vector3 newPos) {
        // (center) gameObject.transform.position = new Vector3(newPos.x, newPos.y, gameObject.transform.position.z);
        Vector3 dims = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.size;
        gameObject.transform.position = new Vector3(newPos.x + dims.x / 2f, newPos.y + dims.y / 2f, gameObject.transform.position.z);
    }

    public void UpdateLabel (string newText, Vector3 newPos) {
        ChangeText(newText);
        WrapTextWithBackground();
        MoveToPos(newPos);
    }
}
