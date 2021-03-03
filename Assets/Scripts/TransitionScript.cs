using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    public GameObject gameParent;
    public GameObject mapParent;
    public GameObject boardObj;
    public GameObject mapObj;

    public void TransitionToMap (GameLevel gl, int winner) {
        gameParent.SetActive(false);
        mapParent.SetActive(true);
        if (winner == 0) {  // human won
            mapObj.GetComponent<MapScript>().UpdateLevelAccesses(gl);
        }
    }

    public void TransitionToGame (GameLevel gl) {
        mapParent.SetActive(false);
        gameParent.SetActive(true);
        boardObj.GetComponent<BoardScript>().InitSetupPhase(gl);
    }
}
