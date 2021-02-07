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

    public void InitPieceQuads (string team, GameObject piecesParent, GameObject tileToScale, Dictionary<string, GameObject[]> piecesDict, Dictionary<GameObject, Dictionary<string, string>> pieceData) {
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(quadImgTemplate, tileToScale);
        string assetsDir = Application.dataPath;
        string piecesTeamsPath = assetsDir + "/Files/Images/Pieces/";
        string localDirPath = piecesTeamsPath + team + "/";
        Texture2D tex;
        GameObject newQuadImg;
        foreach(KeyValuePair<string, int> item in pieceQuantities) {
            tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(localDirPath + item.Key + ".png");
            piecesDict[item.Key] = new GameObject[item.Value];
            for (int i = 0; i < item.Value; i++) {
                newQuadImg = Instantiate(quadImgTemplate, new Vector3(0, -20, -0.003f), Quaternion.identity);
                scriptMaster.GetComponent<ValueLabel>().PositionLabel(newQuadImg);
                scriptMaster.GetComponent<ValueLabel>().RenameLabel(newQuadImg, item.Key);
                newQuadImg.transform.parent = piecesParent.transform;
                newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
                piecesDict[item.Key][i] = newQuadImg;
                pieceData[newQuadImg] = new Dictionary<string, string>();
                pieceData[newQuadImg]["value"] = item.Key;
                pieceData[newQuadImg]["team"] = "0";
            }
        }
    }

    public void InitEnemyPieces (GameObject piecesParent, GameObject[,] board, string[,] enemyValues, Dictionary<GameObject, Dictionary<string, string>> pieceData, GameObject boardObj) {
        string backImgPath = Application.dataPath + "/Files/Images/Pieces/back.png";
        Texture2D tex = scriptMaster.GetComponent<TextureScript>().CreateTexture(backImgPath);
        GameObject newQuadImg;
        int x, y;
        int numRows = 10, numCols = 10;

        for (int i = 0; i < enemyValues.GetLength(0); i++) {
            y = numRows - i - 1;   // algo places as if in human pos
            for (int j = 0; j < enemyValues.GetLength(1); j++) {
                x = numCols - j - 1;
                newQuadImg = Instantiate(quadImgTemplate, boardObj.GetComponent<BoardScript>().GetTilePos(x, y), Quaternion.identity);
                scriptMaster.GetComponent<ValueLabel>().RemoveLabel(newQuadImg);
                newQuadImg.transform.parent = piecesParent.transform;
                newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
                pieceData[newQuadImg] = new Dictionary<string, string>();
                pieceData[newQuadImg]["value"] = enemyValues[i, j];
                pieceData[newQuadImg]["team"] = "1";
                board[y, x] = newQuadImg;
            }
        }
    }
}
