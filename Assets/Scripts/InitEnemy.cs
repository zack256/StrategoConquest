﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InitEnemy : MonoBehaviour
{

    public GameObject boardObj;
    public GameObject scriptMaster;

    /**
    void FisherYatesShuffle2 (string[] arr) {
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
    **/

    string[,] RandomSetup () {
        int numCols = 10;
        int teamStartRows = 4;
        string[] arr = new string[numCols * teamStartRows];
        Dictionary<string, int> pieceQuantities = boardObj.GetComponent<PiecesScript>().pieceQuantities;
        int z = 0;
        foreach(KeyValuePair<string, int> item in pieceQuantities) {
            for (int i = 0; i < item.Value; i++) {
                arr[z] = item.Key;
                z++;
            }
        }
        scriptMaster.GetComponent<Utils>().FisherYatesShuffle(arr);
        //FisherYatesShuffle(arr);
        string[,] valueList = new string[teamStartRows, numCols];
        z = 0;
        for (int i = 0; i < teamStartRows; i++) {
            for (int j = 0; j < numCols; j++) {
                valueList[i, j] = arr[z];
                z++;
            }
        }
        return valueList;
    }

    public string[,] InitPieces (string algo = null) {
        string[,] valueList = null;
        if (algo == "other") {
        } else {
            valueList = RandomSetup();
        }
        return valueList;
    }

    void Start () {
        //InitPieces("random");
    }
}
