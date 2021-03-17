using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private string levelName;
    private int accessLevel = 0;    // 0 = locked. 1 = unbeaten. 2 = beaten.
    private HashSet<string> andReqs = new HashSet<string>();
    private HashSet<string> orReqs = new HashSet<string>();
    private GameObject nodeObj;
    private string speakerImgFilePath;
    private int levelType;

    public MapNode (string levelName, GameObject nodeObj, string speakerImgFileName, int levelType) {
        this.levelName = levelName;
        this.nodeObj = nodeObj;
        SetSpeakerImgPath(speakerImgFileName);
        this.levelType = levelType;
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

    public int GetLevelType () {
        return levelType;
    }
    public string GetName () {
        return this.levelName;
    }
    public int GetAccess () {
        return this.accessLevel;
    }
    public void SetAccess (int newAccess) {
        this.accessLevel = newAccess;
    }
    public GameObject GetNodeObj () {
        return this.nodeObj;
    }
    public string GetSpeakerImgFilePath () {
        return this.speakerImgFilePath;
    }
    public string GetDialoguePath (string which) {
        return Application.dataPath + "/Files/Dialogue/Levels/" + GetName() + "/" + which + ".txt";
    }
    public virtual string GetHoverLabelMessage () {
        return GetName();
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
            SetAccess(1);
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

    public bool CanVisit () {
        return accessLevel != 0;
    }

    public virtual void BeatLevel () {  // eh...
    }

    public static implicit operator bool(MapNode gl) {
        return gl != null;
    }
}
