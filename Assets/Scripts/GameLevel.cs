using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel
{
    private string levelName;
    private int accessLevel = 0;    // 0 = locked. 1 = unbeaten. 2 = beaten.
    private HashSet<string> andReqs = new HashSet<string>();
    private List<string> orReqs = new List<string>();
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
            andReqs.Add(row[i]);
        }
    }

    public void LoadORReqs (string[] row) {
        for (int i = 1; i < row.Length; i++) {  // first col is "A"
            orReqs.Add(row[i]);
        }
    }
}
