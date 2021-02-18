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

    bool IsStraightLine (int[] pos1, int[] pos2) {  // hopover bug
        return (pos1[0] == pos2[0]) ^ (pos1[1] == pos2[1]);
    }

    public bool IsValidMove (PieceObj[,] board, int[] startPos, int[] destPos, PieceObj mover) {
        // No fight, just move.
        if (startPos.SequenceEqual(destPos)) {
            return false;
        }
        if (mover.GetValue() == "2") {
            return IsStraightLine(startPos, destPos);
        }
        return IsAdjacent(startPos, destPos);
    }

    public int FightResult (PieceObj[,] board, int[] startPos, int[] destPos, PieceObj attacker) {
        // -1 : Invalid. 0 = Attacker captures. 1 = Defender captures. 2 = Both captured.
        if (!IsValidMove(board, startPos, destPos, attacker)) {
            return -1;
        }
        PieceObj defender = board[destPos[1], destPos[0]];
        string attackerVal = attacker.GetValue();
        string defenderVal = defender.GetValue();
        if (attacker.GetTeam() == defender.GetTeam()) {
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
