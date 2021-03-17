using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MapNode
{
    public GameLevel (string levelName, GameObject nodeObj, string speakerImgFileName) : base(levelName, nodeObj, speakerImgFileName, 0) {
    }

    public override void BeatLevel () {
        SetAccess(2);   // defeated
        RefreshNodeColor();
    }
}
