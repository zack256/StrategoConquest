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

    bool IsMapNode (GameObject obj) {
        return obj.transform.parent.gameObject == nodeParent;
    }
    
    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if (IsMapNode(obj)) {
            GameLevel gl = nodeMap[obj];
            Debug.Log(gl.GetName());
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {

    }

    void CreateNodeMap () {
        GameLevel gl;
        nodeMap = new Dictionary<GameObject, GameLevel>();
        for (int i = 0; i < gameLevels.Length; i++) {
            gl = gameLevels[i];
            Debug.Log(gl.GetName() + i);
            nodeMap[gl.GetNodeObj()] = gl;
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
        bool justClicked = Input.GetMouseButtonDown(0);
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
