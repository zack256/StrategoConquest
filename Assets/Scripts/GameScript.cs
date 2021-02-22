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

    bool IsStraightLine (int[] pos1, int[] pos2) {
        return (pos1[0] == pos2[0]) ^ (pos1[1] == pos2[1]);
    }

    bool IsClearStraightLine (PieceObj[,] board, int[] pos1, int[] pos2) {
        // every tile btwn is clear.
        if (!IsStraightLine(pos1, pos2)) {
            return false;
        }
        int[] start, finish;
        if (pos1[0] != pos2[0]) {
            if (pos1[0] > pos2[0]) {
                start = pos2; finish = pos1;
            } else {
                start = pos1; finish = pos2;
            }
            for (int i = start[0] + 1; i < finish[0]; i++) {
                if (board[start[1], i]) {
                    return false;
                }
            }
        } else {
            if (pos1[1] > pos2[1]) {
                start = pos2; finish = pos1;
            } else {
                start = pos1; finish = pos2;
            }
            for (int i = start[1] + 1; i < finish[1]; i++) {
                if (board[i, start[0]]) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsValidMove (PieceObj[,] board, int[] startPos, int[] destPos, PieceObj mover) {
        // No fight, just move.
        if (startPos.SequenceEqual(destPos)) {
            return false;
        }
        if (mover.GetValue() == "2") {
            return IsClearStraightLine(board, startPos, destPos);
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

    PieceObj GetFromBoard (int[] loc, PieceObj[,] board) {
        // confusing xy/ij
        return board[loc[0], loc[1]];
    }

    bool IsOnBoard (int[] loc, PieceObj[,] board) {
        return loc[0] >= 0 && loc[0] < board.GetLength(0) && loc[1] >= 0 && loc[1] < board.GetLength(1);
    }

    int[] GetAdjPos (int[] pos, int direction) {
        if (direction == 0) {
            return new int[] {pos[0] - 1, pos[1]};
        } else if (direction == 1) {
            return new int[] {pos[0], pos[1] + 1};
        } else if (direction == 2) {
            return new int[] {pos[0] + 1, pos[1]};
        } else {
            return new int[] {pos[0], pos[1] - 1};
        }
    }

    bool EnemyCanMoveHelper (int[] pos, int direction, PieceObj[,] board) {
        // NESW
        int[] dest = GetAdjPos(pos, direction);
        return IsOnBoard(dest, board) && ((!GetFromBoard(dest, board)) || (GetFromBoard(dest, board).GetTeam() != 1));
    }

    int Enemy2MoveHelper (PieceObj[,] board, int[] pos, int dx, int dy, int reach) {
        int[] destPos = new int[] {pos[0] + dy * reach, pos[1] + dx * reach};
        if (!IsOnBoard(destPos, board)) {
            return -1;
        }
        PieceObj po = GetFromBoard(destPos, board);
        if (!po) {
            return 1;
        }
        if (po.GetTeam() == 1) {
            return -1;
        } else {
            return 0;
        }
    }

    public int[,] AIRandomMove0 (PieceObj[,] board) {
        // selects a random piece that can move. then moves it to a random location.
        PieceObj po;
        int[] pos;
        string val;
        bool good;
        List<int[]> locations = new List<int[]>();
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                po = board[i, j];
                if (!po) {
                    continue;
                }
                if (po.GetTeam() != 1) {
                    continue;
                }
                val = po.GetValue();
                if ((val == "B") || (val == "F")) {
                    continue;
                }
                pos = new int[] {i, j};
                good = false;
                for (int d = 0; d < 4; d++) {
                    if (EnemyCanMoveHelper(pos, d, board)) {
                        good = true;
                        break;
                    }
                }
                if (good) {
                    locations.Add(pos);
                }
            }
        }
        if (locations.Count == 0) {
            Debug.Log("enemy cant move!");
        }
        System.Random rand = new System.Random();
        int idx = rand.Next(locations.Count);
        pos = locations[idx];
        po = GetFromBoard(pos, board);
        List<int[]> destinations = new List<int[]>();
        if (po.GetValue() != "2") {
            for (int d = 0; d < 4; d++) {
                if (EnemyCanMoveHelper(pos, d, board)) {
                    destinations.Add(GetAdjPos(pos, d));
                }
            }
        } else {
            int end, dx, dy;
            for (int d = 0; d < 4; d++) {
                if (d % 2 == 0) {
                    end = board.GetLength(0);
                } else {
                    end = board.GetLength(1);
                }
                if (d == 0) {
                    dx = 0; dy = 1;
                } else if (d == 1) {
                    dx = 1; dy = 0;
                } else if (d == 2) {
                    dx = 0; dy = -1;
                } else {
                    dx = -1; dy = 0;
                }
                int result;
                for (int reach = 1; reach < end; reach++) {
                    result = Enemy2MoveHelper(board, pos, dx, dy, reach);
                    if (result != -1) {
                        destinations.Add(new int[] {pos[0] + dy * reach, pos[1] + dx * reach});
                    }
                    if (result != 1) {
                        break;
                    }
                }
            }
        }
        int destIdx = rand.Next(destinations.Count);
        int[] dest = destinations[destIdx];
        int[,] res = new int[2, 2];
        res[0, 0] = pos[0];
        res[0, 1] = pos[1];
        res[1, 0] = dest[0];
        res[1, 1] = dest[1];
        return res;
    }
}
