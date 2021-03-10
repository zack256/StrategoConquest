using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel
{
    private string levelName;
    private int accessLevel = 0;    // 0 = locked. 1 = unbeaten. 2 = beaten.
    private HashSet<string> andReqs = new HashSet<string>();
    private HashSet<string> orReqs = new HashSet<string>();
    private GameObject nodeObj;
    private string speakerImgFilePath;
    private string[] dialoguePaths;

    public GameLevel (string levelName, GameObject nodeObj, string speakerImgFileName) {
        this.levelName = levelName;
        this.nodeObj = nodeObj;
        SetSpeakerImgPath(speakerImgFileName);
        InitDialoguePaths();
    }

    void InitDialoguePaths () {
        string dialoguesPath = Application.dataPath + "/Files/Dialogue/Levels/" + GetName();
        string beforeDialoguePath = dialoguesPath + "/before.txt";
        string afterDialoguePath = dialoguesPath + "/after.txt";    // 2 for now.
        dialoguePaths = new string[] {beforeDialoguePath, afterDialoguePath};
    }

    void SetSpeakerImgPath (string fileName) {
        string assetsDir = Application.dataPath;
        string imgsDir = assetsDir + "/Files/Images/";
        if (fileName[0] != '~') {
            this.speakerImgFilePath = imgsDir + "Dialogue/" + fileName + ".png";    // If no "~", uses a seperate image.
        } else {
            this.speakerImgFilePath = imgsDir + "Pieces/" + levelName + "/" + fileName.Substring(1) + ".png";    // If "~" in name, uses one of the pieces for dialogue
        }
    }

    public string GetName () {
        return this.levelName;
    }
    public int GetAccess () {
        return this.accessLevel;
    }
    public GameObject GetNodeObj () {
        return this.nodeObj;
    }
    public string GetSpeakerImgFilePath () {
        return this.speakerImgFilePath;
    }
    public string GetDialoguePath (bool isBefore) {
        int idx = isBefore ? 0 : 1;
        return dialoguePaths[idx];
    }


    public void LoadANDReqs (string[] row) {
        for (int i = 1; i < row.Length; i++) {  // first col is "A"
            if (row[i] != "") {
                andReqs.Add(row[i]);
            }
        }
    }
    public void LoadORReqs (string[] row) {
        for (int i = 1; i < row.Length; i++) {  // first col is "O"
            if (row[i] != "") {
                orReqs.Add(row[i]);
            }
        }
    }

    void UnlockLevel () {
        if (accessLevel == 0) {
            accessLevel = 1;
        }
        RefreshNodeColor();
    }

    void WorkOnANDReqs (string newlyBeaten) {
        andReqs.Remove(newlyBeaten);
    }
    void WorkOnORReqs (string newlyBeaten) {
        if (orReqs.Contains(newlyBeaten)) {
            orReqs = new HashSet<string>();
        }
    }
    bool ReqsCompleted () {
        return andReqs.Count + orReqs.Count == 0;
    }
    public void TryToUnlockLevel () {
        if (ReqsCompleted()) {
            UnlockLevel();
        }
    }
    public void AccountForBeatenLevel (string newlyBeaten) {
        WorkOnANDReqs(newlyBeaten);
        WorkOnORReqs(newlyBeaten);
    }
    
    public void RefreshNodeColor () {
        nodeObj.GetComponent<HighlightNode>().SetAccessLevel(this);
    }

    public static implicit operator bool(GameLevel gl) {
        return gl != null;
    }
}
