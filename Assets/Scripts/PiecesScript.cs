using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PiecesScript : MonoBehaviour
{

    public GameObject quadImgTemplate;
    public GameObject scriptMaster;

    public Dictionary<string, int> pieceQuantities = new Dictionary<string, int> {
        {"2", 8},
        {"3", 5},
        {"4", 4},
        {"5", 4},
        {"6", 4},
        {"7", 3},
        {"8", 2},
        {"9", 1},
        {"10", 1},
        {"B", 6},
        {"S", 1},
        {"F", 1},
    };

    string GetTeamImagesPath (string team) {
        string assetsDir = Application.dataPath;
        string piecesTeamsPath = assetsDir + "/Files/Images/Pieces/";
        return piecesTeamsPath + team + "/";
    }

    public void InitGoodPieces (string team, GameObject piecesParent, GameObject tileToScale, Dictionary<string, GameObject[]> piecesDict, Dictionary<GameObject, PieceObj> pOMap, Player player) {
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(quadImgTemplate.transform.GetChild(0).gameObject, tileToScale);
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(quadImgTemplate.transform.GetChild(1).gameObject, tileToScale);
        string localDirPath = GetTeamImagesPath(team);
        Texture2D tex;
        GameObject newQuadImg;
        PieceObj po;
        GameObject quadFront;
        GameObject quadBack;
        Dictionary<string, int> pieceQuantities = player.GetPieceAmts();
        foreach(KeyValuePair<string, int> item in pieceQuantities) {
            tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(localDirPath + item.Key + ".png");
            piecesDict[item.Key] = new GameObject[item.Value];
            for (int i = 0; i < item.Value; i++) {
                newQuadImg = Instantiate(quadImgTemplate, new Vector3(0, -20, -0.003f), Quaternion.identity);
                po = new PieceObj(newQuadImg, 0, item.Key, true);
                quadFront = po.GetFront();
                quadBack = po.GetBack();
                scriptMaster.GetComponent<ValueLabel>().PositionLabel(quadFront);
                scriptMaster.GetComponent<ValueLabel>().RenameLabel(quadFront, item.Key);
                newQuadImg.transform.parent = piecesParent.transform;
                quadFront.GetComponent<Renderer>().material.mainTexture = tex;
                piecesDict[item.Key][i] = newQuadImg;
                pOMap[po.GetMain()] = po;
            }
        }
    }

    public void InitEnemyPieces (GameObject piecesParent, PieceObj[,] board, string[,] enemyValues, GameObject boardObj, string team) {
        string backImgPath = Application.dataPath + "/Files/Images/Pieces/back.png";
        string localDirPath = GetTeamImagesPath(team);
        Texture2D backTex = scriptMaster.GetComponent<TextureScript>().CreateTexture(backImgPath);
        Texture2D frontTex;
        GameObject newQuadImg;
        int x, y;
        int numRows = 10, numCols = 10;
        GameObject quadFront;
        GameObject quadBack;
        string val;
        for (int i = 0; i < enemyValues.GetLength(0); i++) {
            y = numRows - i - 1;   // algo places as if in human pos
            for (int j = 0; j < enemyValues.GetLength(1); j++) {
                x = numCols - j - 1;
                val = enemyValues[i, j];
                newQuadImg = Instantiate(quadImgTemplate, boardObj.GetComponent<BoardScript>().GetTilePos(x, y), Quaternion.Euler(0, 180, 0));
                PieceObj po = new PieceObj(newQuadImg, 1, val, false);
                quadFront = po.GetFront();
                quadBack = po.GetBack();
                scriptMaster.GetComponent<ValueLabel>().PositionLabel(quadFront);
                scriptMaster.GetComponent<ValueLabel>().RenameLabel(quadFront, val);
                po.ToggleMeshCollider(false);
                newQuadImg.transform.parent = piecesParent.transform;
                quadBack.GetComponent<Renderer>().material.mainTexture = backTex;
                frontTex = scriptMaster.GetComponent<TextureScript>().CreateTexture(localDirPath + val + ".png");
                quadFront.GetComponent<Renderer>().material.mainTexture = frontTex;
                board[y, x] = po;
                po.ToggleLabel(false);
                if (val == "F") {
                    Debug.Log(y + " ll " + x);
                }
            }
        }
    }
}
