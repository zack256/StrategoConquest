using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObj
{
    private GameObject obj;
    private static float turnSpeed = 0.75f;    // deg per frame
    private int turnFramesLeft = 0;
    private static int totalTurnFrames = (int) Math.Ceiling(180 / turnSpeed);

    private int fadeFramesLeft = 0;
    private static int totalFadeFrames = totalTurnFrames;
    private static float fadeSpeed = 256f / totalFadeFrames;

    private bool facingForward;  // false means back is showing.
    private int team;
    private string value;
    private GameObject transparentSlider;

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
    public bool IsFaceUp() {
        return facingForward;
    }
    public void SetTransparentSlider (GameObject slider) {
        transparentSlider = slider;
    }
    public void RemoveTransparentSlider () {
        GameObject.Destroy(transparentSlider);
        transparentSlider = null;
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
    public bool IsFading () {
        return this.fadeFramesLeft > 0;
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

    public void StartFading () {
        fadeFramesLeft = totalFadeFrames;
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

    void FinishFade () {
        Color32 col = transparentSlider.GetComponent<Renderer>().material.GetColor("_Color");
        col.a = 255;
        transparentSlider.GetComponent<Renderer>().material.SetColor("_Color", col);
    }

    public void UpdateFade () {
        this.fadeFramesLeft--;
        if (this.fadeFramesLeft == 0) {
            FinishFade();
        } else {
            Color32 col = transparentSlider.GetComponent<Renderer>().material.GetColor("_Color");
            col.a = (byte) ((int) ((totalFadeFrames - fadeFramesLeft) * fadeSpeed));
            transparentSlider.GetComponent<Renderer>().material.SetColor("_Color", col);
        }
    }

    public static implicit operator bool(PieceObj po) {
        return po != null;
    }
}