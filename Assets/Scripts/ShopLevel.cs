using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLevel : MapNode
{
    public ShopLevel (string levelName, GameObject nodeObj, string speakerImgFileName) : base(levelName, nodeObj, speakerImgFileName) {
    }

    public override string GetHoverLabelMessage () {
        return GetName() + " (shop)";
    }

}
