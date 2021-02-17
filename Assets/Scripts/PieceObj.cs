using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObj
{
    private GameObject obj;
    private float turnSpeed = 0.75f;    // deg per frame
    private int turnFramesLeft = 0;
    private bool facingForward = true;  // false means back is showing.

    public PieceObj (GameObject obj) {
        this.obj = obj;
    }

    public PieceObj (GameObject obj, bool facingForward) {
        this.obj = obj;
        this.facingForward = facingForward;
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

    public bool IsTurning () {
        return this.turnFramesLeft > 0;
    }

    public void ToggleMeshCollider (bool enable) {
        obj.GetComponent<MeshCollider>().enabled = enable;
    }

    public void MoveToPos (Vector3 newPos) {
        obj.transform.position = newPos;
    }

    public void MoveToPos (GameObject destObj) {
        MoveToPos(destObj.transform.position);
    }

    public void TemporaryVerticalShift (bool isDown) {
        int sign = isDown ? 1 : -1;
        float shiftDistance = 1f;
        MoveToPos(new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + sign * shiftDistance));
    }

    public void StartTurning () {
        TemporaryVerticalShift(false);
        this.turnFramesLeft = (int) Math.Ceiling(180 / this.turnSpeed);
    }

    public void StopTurning () {
        TemporaryVerticalShift(true);
    }

    void FinishTurn () {
        if (facingForward) {
            obj.transform.rotation = Quaternion.Euler(0, 180, 0);
        } else {
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        facingForward = !facingForward;
    }

    public void UpdateTurn () {
        this.turnFramesLeft--;
        if (this.turnFramesLeft == 0) {
            FinishTurn();
        } else {
            obj.transform.Rotate(0, turnSpeed, 0);
        }
    }

    public static implicit operator bool(PieceObj po) {
        return po != null;
    }
}