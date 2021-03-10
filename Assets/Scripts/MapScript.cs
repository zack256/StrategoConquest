using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapScript : MonoBehaviour
{

    public GameObject scriptMaster;
    public GameObject cameraObject;
    public GameObject backgroundObj;
    public GameObject nodeParent;
    public GameObject nodeTextLabel;
    public GameObject dialogueParent;
    public GameObject quadImgTemplate;

    private GameLevel[] gameLevels;
    private Dictionary<GameObject, GameLevel> nodeMap;
    private GameObject overNode = null;
    private Player player;
    private int mapMode = 0;
    private GameObject dialogueTextObj;
    private GameObject dialogueRect;
    private string[] dialoguePages;
    private GameLevel goingToLvl;

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

    void SetUpSpeakerImage (string filename) {
        Transform speakerTransform = dialogueParent.transform.GetChild(0);
        Texture2D tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(filename);
        GameObject newQuadImg = Instantiate(quadImgTemplate, speakerTransform.position, Quaternion.identity);
        scriptMaster.GetComponent<ValueLabel>().RemoveLabel(newQuadImg);
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(newQuadImg, speakerTransform.gameObject);
        newQuadImg.transform.parent = speakerTransform;
        newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
    }

    void StartDialogue () {
        mapMode = 1;
        dialogueParent.SetActive(true);
        SetUpSpeakerImage(goingToLvl.GetSpeakerImgFilePath());
        
        string dialogueText = File.ReadAllText(goingToLvl.GetDialoguePath(true));
        string[] dialoguePages = dialogueText.Split('\n');
        dialogueTextObj.GetComponent<DialogueScript>().LoadDialogue(dialoguePages);
    }

    void NextDialogue () {
        bool done = dialogueTextObj.GetComponent<DialogueScript>().NextDialogue();
        if (done) {
            scriptMaster.GetComponent<TransitionScript>().TransitionToGame(goingToLvl);
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
                    nodeTextLabel.SetActive(false);
                    goingToLvl = gl;
                    StartDialogue();
                }
            }
        } else {
            ResetNodeHighlight(overNode);
            if ((obj == dialogueRect) && (mapMode == 1) && (justClicked)) {
                NextDialogue();
            }
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
        dialogueRect = dialogueParent.transform.GetChild(1).gameObject;
        dialogueTextObj = dialogueRect.transform.GetChild(0).GetChild(0).gameObject;
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
