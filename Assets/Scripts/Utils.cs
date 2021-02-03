﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public void FisherYatesShuffle<T> (T[] arr) {
        System.Random rand = new System.Random();
        int r;
        T temp;
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++) {
            r = rand.Next(i, n);
            temp = arr[r];
            arr[r] = arr[i];
            arr[i] = temp;
        }
    }
}
