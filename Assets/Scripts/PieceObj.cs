using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObj
{
    private GameObject obj;

    public PieceObj (GameObject obj) {
        this.obj = obj;
    }

    public GameObject GetMain () {
        return obj;
    }

    public GameObject GetFront () {
        return obj.transform.GetChild(0).gameObject;
    }

    public GameObject GetBack () {
        return obj.transform.GetChild(1).gameObject;
    }

    public GameObject GetLabel () {
        return obj.transform.GetChild(0).GetChild(0).gameObject;
    }

    public void ToggleMeshCollider (bool enable) {
        obj.GetComponent<MeshCollider>().enabled = enable;
    }

    public static implicit operator bool(PieceObj po) {
        return po != null;
    }
}