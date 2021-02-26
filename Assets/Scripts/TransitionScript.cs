using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    public GameObject gameParent;
    public GameObject mapParent;
    public GameObject boardObj;

    public void TransitionToMap () {
        gameParent.SetActive(false);
        mapParent.SetActive(true);
    }

    public void TransitionToGame () {
        boardObj.GetComponent<BoardScript>().CleanUpBeforeGameStart();
        mapParent.SetActive(false);
        gameParent.SetActive(true);
    }
}
