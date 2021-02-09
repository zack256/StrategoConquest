using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    bool IsAdjacent (int[] pos1, int[] pos2) {
        return Math.Abs(pos1[0] - pos2[0]) + Math.Abs(pos1[1] - pos2[1]) == 1;
    }

    public bool IsValidMove (GameObject[,] board, Dictionary<GameObject, Dictionary<string, string>> pieceData, int[] startPos, int[] destPos, GameObject mover) {
        // No fight, just move.
        if (startPos.SequenceEqual(destPos)) {
            return false;
        }
        //return !board[destPos[1], destPos[0]];
        return IsAdjacent(startPos, destPos);
    }

    public int FightResult (GameObject[,] board, Dictionary<GameObject, Dictionary<string, string>> pieceData, int[] startPos, int[] destPos, GameObject attacker) {
        // -1 : Invalid. 0 = Attacker captures. 1 = Defender captures. 2 = Both captured.
        if (!IsValidMove(board, pieceData, startPos, destPos, attacker)) {
            return -1;
        }
        //GameObject attacker = board[startPos[1], startPos[0]];
        GameObject defender = board[destPos[1], destPos[0]];
        //Debug.Log(pieceData[attacker]);
        string attackerVal = pieceData[attacker]["value"];
        string defenderVal = pieceData[defender]["value"];
        if (pieceData[attacker]["team"] == pieceData[defender]["team"]) {
            return -1;
        }
        if (attackerVal == defenderVal) {
            return 2;   // tie, both captured.
        }
        if (defenderVal == "B") {
            if (attackerVal == "3") {
                return 0;   // 3s disarm bombs.
            } else {
                return 2;   // Bombs explode, getting both.
            }
        }
        if ((defenderVal == "S") || (defenderVal == "F")) {
            return 0;   // both these pieces are captured by any attacker (exc. S-S)
        }
        if (attackerVal == "S") {
            if (defenderVal == "10") {
                return 0;
            } else {
                return 1;
            }
        }
        int aVInt = Int32.Parse(attackerVal);
        int dVInt = Int32.Parse(defenderVal);
        if (aVInt == dVInt) {
            return 2;
        } else if (aVInt > dVInt) {
            return 0;
        } else {
            return 1;
        }
    }
}
