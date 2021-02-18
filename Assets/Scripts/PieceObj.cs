using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObj
{
    private GameObject obj;
    private static float turnSpeed = 0.75f;    // deg per frame
    private int turnFramesLeft = 0;
    private bool facingForward;  // false means back is showing.
    private static int totalTurnFrames = (int) Math.Ceiling(180 / turnSpeed);
    private int team;
    private string value;

    public PieceObj (GameObject obj, int team, string value, bool facingForward) {
        this.obj = obj;
        this.team = team;
        this.value = value;
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
    public GameObject GetLabel () { // front
        return obj.transform.GetChild(0).GetChild(0).gameObject;
    }

    public int GetTeam () {
        return team;
    }
    public string GetValue () {
        return value;
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
        turnFramesLeft = totalTurnFrames;
    }

    public void StopTurning () {
        TemporaryVerticalShift(true);
    }

    public void ToggleLabel () {
        GameObject label = GetLabel();
        label.SetActive(!label.activeSelf);
    }

    public void ToggleLabel (bool enable) {
        GetLabel().SetActive(enable);
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
        if (turnFramesLeft <= totalTurnFrames / 2) {
            if (facingForward) {
                ToggleLabel(false);
            } else {
                ToggleLabel(true);
            }
        }
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