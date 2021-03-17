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
    private ShopLevel[] shopLevels;
    //private Dictionary<GameObject, MapNode> nodeMap;  // maybe try and figure out later :)
    private Dictionary<GameObject, GameLevel> gameLevelnodeMap;
    private Dictionary<GameObject, ShopLevel> shopLevelnodeMap;
    private GameObject overNode = null;
    private Player player;
    private int mapMode = 0;
    private GameObject dialogueTextObj;
    private GameObject dialogueRect;
    private string[] dialoguePages;
    private GameLevel goingToLvl;
    private GameObject speakerImgQuad;

    bool IsMapNode (GameObject obj) {
        return obj.transform.parent.gameObject == nodeParent;
    }
    
    void HighlightMapNode (GameObject nodeObj, bool mouseDown, Vector3 point) {
        overNode = nodeObj;
        //GameLevel gl = nodeMap[nodeObj];
        MapNode mn;
        if (gameLevelnodeMap.ContainsKey(nodeObj)) {
            mn = gameLevelnodeMap[nodeObj];
        } else {
            mn = shopLevelnodeMap[nodeObj];
        }
        nodeObj.GetComponent<HighlightNode>().MouseOverNode(mouseDown);
        //nodeTextLabel.GetComponent<HoverTextLabel>().ChangeText(gl.GetName());
        //nodeTextLabel.GetComponent<HoverTextLabel>().MoveToPos(point);
        //nodeTextLabel.GetComponent<HoverTextLabel>().UpdateLabel(gl.GetName(), point);
        nodeTextLabel.GetComponent<HoverTextLabel>().UpdateLabel(mn.GetHoverLabelMessage(), point);
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
        Texture2D tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(filename);
        speakerImgQuad.GetComponent<Renderer>().material.mainTexture = tex;
    }

    bool StartDialogue (string whichDialogue) {
        string dialoguePath = goingToLvl.GetDialoguePath(whichDialogue);
        if (!File.Exists(dialoguePath)) {   // dialogues arent required
            return false;
        }
        dialogueParent.SetActive(true);
        SetUpSpeakerImage(goingToLvl.GetSpeakerImgFilePath());
        string dialogueText = File.ReadAllText(dialoguePath);
        string[] dialoguePages = dialogueText.Split('\n');
        dialogueTextObj.GetComponent<DialogueScript>().LoadDialogue(dialoguePages);
        return true;
    }

    void TransitionToGame() {
        scriptMaster.GetComponent<TransitionScript>().TransitionToGame(goingToLvl);
    }

    void NextDialogue () {
        bool done = dialogueTextObj.GetComponent<DialogueScript>().NextDialogue();
        if (done) {
            if (mapMode == 1) { // dialogue before battle
                TransitionToGame();
            } else if (mapMode == 2) {  // after battle
                dialogueParent.SetActive(false);
                mapMode = 0;
            }
        }
    }

    public void HandleBattleEnd (int winner) {
        string winRes;
        if (winner == 0) {  // human won
            goingToLvl.BeatLevel();
            UpdateLevelAccesses(goingToLvl);
            winRes = "victory";
        } else if (winner == 1) {   // lost
            winRes = "defeat";
        } else {    // tie
            winRes = "tie";
        }
        bool dialogueStarted = StartDialogue(winRes);
        if (dialogueStarted) {
            mapMode = 2;
        } else {
            dialogueParent.SetActive(false);
            mapMode = 0;
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if (IsMapNode(obj)) {
            if (obj != overNode) {
                ResetNodeHighlight(overNode);
            } 
            HighlightMapNode(obj, mouseDown, point);
            if (justClicked) {
                if (gameLevelnodeMap.ContainsKey(obj)) {
                    GameLevel gl = gameLevelnodeMap[obj];
                    if (gl.GetAccess() != 0) {
                        nodeTextLabel.SetActive(false);
                        goingToLvl = gl;
                        mapMode = 1;
                        bool dialogueStarted = StartDialogue("before");
                        if (!dialogueStarted) {
                            TransitionToGame();
                        }
                    }
                } else {
                    ShopLevel sl = shopLevelnodeMap[obj];
                    Debug.Log("@ shop " + sl.GetName());
                }
            }
        } else {
            ResetNodeHighlight(overNode);
            if ((obj == dialogueRect) && ((mapMode == 1) || (mapMode == 2)) && (justClicked)) {
                NextDialogue();
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
        ResetNodeHighlight(overNode);
    }

    void CreateNodeMap () {
        GameLevel gl;
        gameLevelnodeMap = new Dictionary<GameObject, GameLevel>();
        for (int i = 0; i < gameLevels.Length; i++) {
            gl = gameLevels[i];
            gameLevelnodeMap[gl.GetNodeObj()] = gl;
        }
        ShopLevel sl;
        shopLevelnodeMap = new Dictionary<GameObject, ShopLevel>();
        for (int i = 0; i < shopLevels.Length; i++) {
            sl = shopLevels[i];
            shopLevelnodeMap[sl.GetNodeObj()] = sl;
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
        ShopLevel sl;
        for (int i = 0; i < shopLevels.Length; i++) {
            sl = shopLevels[i];
            sl.AccountForBeatenLevel(lvlName);
            sl.TryToUnlockLevel();
        }
    }
    
    public Player GetPlayer () {
        return player;
    }

    void InitSpeakerSetup () {
        Transform speakerTransform = dialogueParent.transform.GetChild(0);
        speakerImgQuad = Instantiate(quadImgTemplate, speakerTransform.position, Quaternion.identity);
        speakerImgQuad.transform.parent = speakerTransform;
        scriptMaster.GetComponent<ValueLabel>().RemoveLabel(speakerImgQuad);
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(speakerImgQuad, dialogueParent.transform.GetChild(0).gameObject);
    }

    void Start () {
        gameLevels = gameObject.GetComponent<NodeScript>().CreateGameLevels();
        shopLevels = gameObject.GetComponent<NodeScript>().CreateShopLevels();
        CreateNodeMap();
        player = new Player(1000);
        dialogueRect = dialogueParent.transform.GetChild(1).gameObject;
        dialogueTextObj = dialogueRect.transform.GetChild(0).GetChild(0).gameObject;
        InitSpeakerSetup();
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
