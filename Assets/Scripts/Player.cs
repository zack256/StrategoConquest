using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int cash;
    private Dictionary<string, int> pieceAmts;
    private List<string> levelsBeaten = new List<string>();

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


}
