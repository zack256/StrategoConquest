using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObj
{
    private GameObject obj;
    private float turnSpeed;    // deg per sec
    private int turnFramesLeft;

    public PieceObj (GameObject obj, float turnSpeed) {
        this.obj = obj;
        this.turnSpeed = turnSpeed;
        this.turnFramesLeft = 0;
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

    public void UpdateTurn () {
        this.turnFramesLeft--;
        obj.transform.Rotate(0, turnSpeed, 0);
    }

    public static implicit operator bool(PieceObj po) {
        return po != null;
    }
}