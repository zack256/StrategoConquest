using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InitEnemy : MonoBehaviour
{

    public GameObject boardObj;

    void FisherYatesShuffle (string[] arr) {
        System.Random rand = new System.Random();
        int r;
        string temp;
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++) {
            r = rand.Next(i, n);
            temp = arr[r];
            arr[r] = arr[i];
            arr[i] = temp;
        }
    }

    void RandomSetup () {
        string[] arr = new string[10 * 4];
        Dictionary<string, int> pieceQuantities = boardObj.GetComponent<PiecesScript>().pieceQuantities;
        int z = 0;
        foreach(KeyValuePair<string, int> item in pieceQuantities) {
            for (int i = 0; i < item.Value; i++) {
                arr[z] = item.Key;
                z++;
            }
        }
        FisherYatesShuffle(arr);
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 10; j++) {
                //boardObj.GetComponent<BoardScript>().board[10 - i - 1, j] = arr[i * 10 + j];
                Debug.Log(i + " , " + j + " , " + arr[i * 10 + j]);
            }
        }
    }

    public void InitPieces (string algo = null) {
        if (algo == "other") {
        } else {
            RandomSetup();
        }
    }

    void Start () {
        InitPieces("random");
    }
}
