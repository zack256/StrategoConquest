using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private GameObject obj;
    private int accessLevel;

    public MapNode (GameObject obj, int accessLevel) {
        this.obj = obj;
        this.accessLevel = accessLevel;
    }


}
