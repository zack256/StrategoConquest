using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    public GameObject gameParent;
    public GameObject mapParent;
    public GameObject boardObj;
    public GameObject mapObj;
    public GameObject shopParent;
    public GameObject shopObj;

    public void TransitionToMap (int winner) {
        gameParent.SetActive(false);
        mapParent.SetActive(true);
        mapObj.GetComponent<MapScript>().HandleBattleEnd(winner);
    }

    public void TransitionToGame (MapNode mn) {
        mapParent.SetActive(false);
        gameParent.SetActive(true);
        boardObj.GetComponent<BoardScript>().InitSetupPhase(mn);
    }
    public void TransitionToShop (MapNode mn) {
        mapParent.SetActive(false);
        shopParent.SetActive(true);
        shopObj.GetComponent<ShopScript>().SetUpShop(mn);
    }
}
