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

    public GameLevel (string levelName, GameObject nodeObj) {
        this.levelName = levelName;
        this.nodeObj = nodeObj;
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
