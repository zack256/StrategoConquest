using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PiecesScript : MonoBehaviour
{

    public GameObject quadImgTemplate;

    private Dictionary<string, int> pieceQuantities = new Dictionary<string, int> {
        {"2", 8},
        {"3", 5},
        {"4", 4},
        {"5", 4},
        {"6", 4},
        {"7", 3},
        {"8", 2},
        {"9", 1},
        {"10", 1},
        {"B", 1},
        {"S", 1},
        {"F", 1},
    };

    Texture2D CreateTexture (string filePath) {
        byte[] imageData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);
        return tex;
    }

    public Dictionary<string, GameObject[]> InitPieceQuads (string team, GameObject piecesParent) {
        string assetsDir = Application.dataPath;
        string piecesTeamsPath = assetsDir + "/Files/Images/Pieces/";
        string localDirPath = piecesTeamsPath + team + "/";
        Texture2D tex;
        GameObject newQuadImg;
        Dictionary<string, GameObject[]> piecesDict = new Dictionary<string, GameObject[]>();
        foreach(KeyValuePair<string, int> item in pieceQuantities) {
            tex = CreateTexture(localDirPath + item.Key + ".png");
            piecesDict[item.Key] = new GameObject[item.Value];
            for (int i = 0; i < item.Value; i++) {
                newQuadImg = Instantiate(quadImgTemplate, new Vector3(0, -20, 0), Quaternion.identity);
                newQuadImg.transform.parent = piecesParent.transform;
                newQuadImg.GetComponent<Renderer>().material.mainTexture = tex;
                piecesDict[item.Key][i] = newQuadImg;
            }
        }
        return piecesDict;
    }
}
