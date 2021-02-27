using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int cash;
    private Dictionary<string, int> pieceAmts;
    private List<string> levelsBeaten = new List<string>();

    public Player (int cash) {
        this.cash = cash;
        this.pieceAmts = new Dictionary<string, int> {
            {"2", 4},
            {"3", 4},
            {"4", 3},
            {"5", 3},
            {"6", 3},
            {"7", 3},
            {"8", 2},
            {"9", 1},
            {"B", 5},
            {"S", 1},
            {"F", 1},
         };
    }

    public Player (int cash, Dictionary<string, int> pieceAmts) {
        this.cash = cash;
        this.pieceAmts = pieceAmts;
    }

    public int GetCash () {
        return this.cash;
    }
    public Dictionary<string, int> GetPieceAmts () {
        return this.pieceAmts;
    }
    public List<string> GetLevelsBeaten () {
        return this.levelsBeaten;
    }

    public string[] CreatePieceOrder () {
        string[] res = new string[pieceAmts.Count];
        string[] fullOrder = new string[12] {"2", "3", "4", "5", "6", "7", "8", "9", "10", "S", "B", "F"};
        int z = 0;
        for (int i = 0; i < fullOrder.Length; i++) {
            if (pieceAmts.ContainsKey(fullOrder[i])) {
                res[z] = fullOrder[i];
                z++;
            }
        }
        return res;
    }


}
