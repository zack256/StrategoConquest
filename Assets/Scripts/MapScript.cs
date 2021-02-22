using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{

    public GameObject gameParent;
    public GameObject mapParent;

    public void TransitionToMap () {
        gameParent.SetActive(false);
        mapParent.SetActive(true);
    }
}
