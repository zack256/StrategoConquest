using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{

    public GameObject scriptMaster;
    public GameObject cameraObject;
    public GameObject backgroundObj;
    public GameObject nodeParent;

    private GameLevel[] gameLevels;
    private Dictionary<GameObject, GameLevel> nodeMap;
    private GameObject overNode = null;

    bool IsMapNode (GameObject obj) {
        return obj.transform.parent.gameObject == nodeParent;
    }
    
    void HighlightMapNode (GameObject nodeObj, bool mouseDown) {
        overNode = nodeObj;
        GameLevel gl = nodeMap[nodeObj];
        nodeObj.GetComponent<HighlightNode>().MouseOverNode(mouseDown);
    }

    void ResetNodeHighlight (GameObject nodeObj) {
        if (nodeObj) {
            nodeObj.GetComponent<HighlightNode>().ResetMaterial();
            overNode = null;
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if (IsMapNode(obj)) {
            if (obj != overNode) {
                ResetNodeHighlight(overNode);
            } 
            HighlightMapNode(obj, mouseDown);
            if (justClicked) {
                GameLevel gl = nodeMap[obj];
                if (gl.GetAccess() != 0) {
                    scriptMaster.GetComponent<TransitionScript>().TransitionToGame();
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

    void UpdateLevelAccesses (GameLevel newlyBeaten) {
        GameLevel gl;
        bool res;
        string lvlName = newlyBeaten.GetName();
        for (int i = 0; i < gameLevels.Length; i++) {
            gl = gameLevels[i];
            gl.AccountForBeatenLevel(lvlName);
            gl.TryToUnlockLevel();
        }
    }
    
    void Start () {
        //gameLevels = scriptMaster.GetComponent<NodeScript>().CreateGameLevels();
        gameLevels = gameObject.GetComponent<NodeScript>().CreateGameLevels();
        CreateNodeMap();
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
