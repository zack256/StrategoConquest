﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{

    public GameObject scriptMaster;
    public GameObject cameraObject;
    public GameObject backgroundObj;
    public GameObject nodeParent;
    public GameObject nodeTextLabel;

    private GameLevel[] gameLevels;
    private Dictionary<GameObject, GameLevel> nodeMap;
    private GameObject overNode = null;
    private Player player;

    bool IsMapNode (GameObject obj) {
        return obj.transform.parent.gameObject == nodeParent;
    }
    
    void HighlightMapNode (GameObject nodeObj, bool mouseDown, Vector3 point) {
        overNode = nodeObj;
        GameLevel gl = nodeMap[nodeObj];
        nodeObj.GetComponent<HighlightNode>().MouseOverNode(mouseDown);
        //nodeTextLabel.GetComponent<HoverTextLabel>().ChangeText(gl.GetName());
        //nodeTextLabel.GetComponent<HoverTextLabel>().MoveToPos(point);
        nodeTextLabel.GetComponent<HoverTextLabel>().UpdateLabel(gl.GetName(), point);
        nodeTextLabel.SetActive(true);
    }

    void ResetNodeHighlight (GameObject nodeObj) {
        if (nodeObj) {
            nodeObj.GetComponent<HighlightNode>().ResetMaterial();
            overNode = null;
            nodeTextLabel.SetActive(false);
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if (IsMapNode(obj)) {
            if (obj != overNode) {
                ResetNodeHighlight(overNode);
            } 
            HighlightMapNode(obj, mouseDown, point);
            if (justClicked) {
                GameLevel gl = nodeMap[obj];
                if (gl.GetAccess() != 0) {
                    scriptMaster.GetComponent<TransitionScript>().TransitionToGame(gl);
                }
            }
        } else {
            ResetNodeHighlight(overNode);
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
        ResetNodeHighlight(overNode);
    }

    void CreateNodeMap () {
        GameLevel gl;
        nodeMap = new Dictionary<GameObject, GameLevel>();
        for (int i = 0; i < gameLevels.Length; i++) {
            gl = gameLevels[i];
            nodeMap[gl.GetNodeObj()] = gl;
        }
    }

    public void UpdateLevelAccesses (GameLevel newlyBeaten) {
        GameLevel gl;
        string lvlName = newlyBeaten.GetName();
        for (int i = 0; i < gameLevels.Length; i++) {
            gl = gameLevels[i];
            gl.AccountForBeatenLevel(lvlName);
            gl.TryToUnlockLevel();
        }
    }
    
    public Player GetPlayer () {
        return player;
    }

    void Start () {
        gameLevels = gameObject.GetComponent<NodeScript>().CreateGameLevels();
        CreateNodeMap();
        player = new Player(1000);
    }

    void Update () {
        Ray ray = cameraObject.GetComponent<CameraScript>().GetCameraRay();
        RaycastHit hit;
        //bool justClicked = Input.GetMouseButtonDown(0);
        bool justClicked = Input.GetMouseButtonUp(0);
        bool mouseDown = Input.GetMouseButton(0);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == backgroundObj){
                MouseHitNoGameObject(justClicked, mouseDown, hit.point);
            } else {
                MouseHitGameObject(hit.transform.gameObject, justClicked, mouseDown, hit.point);
            }
        }
    }
}
