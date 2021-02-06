using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScript : MonoBehaviour
{
    public void ScaleGameObject (GameObject objToScale, GameObject referenceObj) {
        Vector3 referenceObjDims = referenceObj.GetComponent<Renderer>().bounds.size;
        Vector3 objDims = objToScale.GetComponent<Renderer>().bounds.size;
        float fracX = 1 / (objDims.x / referenceObjDims.x);
        float fracY = 1 / (objDims.y / referenceObjDims.y);
        objToScale.transform.localScale = new Vector3(objToScale.transform.localScale.x * fracX, objToScale.transform.localScale.y * fracY, objToScale.transform.localScale.z);
    }

    public void ScaleByScalar (GameObject objToScale, float scalar) {
        objToScale.transform.localScale *= scalar;
    }
}
