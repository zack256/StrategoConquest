using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MapNode
{
    public GameLevel (string levelName, GameObject nodeObj, string speakerImgFileName) : base(levelName, nodeObj, speakerImgFileName) {
    }

    public void BeatLevel () {
        SetAccess(2);   // defetated
        RefreshNodeColor();
    }
}
