﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NodeScript : MonoBehaviour
{

    public GameObject gameLevelNodeTemplate;
    public GameObject shopLevelNodeTemplate;
    public GameObject nodesParent;
    public GameObject scriptMaster;
    public GameObject pieceObjTemplate;

    private Vector3 lowerLeft;
    private Vector3 mapDims;
    private Utils utilsScript;

    private void CalculateMapProps () {
        Vector3 pos = gameObject.transform.position;
        mapDims = gameObject.GetComponent<Renderer>().bounds.size;
        float x = pos.x - mapDims.x / 2f;
        float y = pos.y - mapDims.y / 2f;
        lowerLeft = new Vector3(x, y, -0.006f);
    }

    private Vector3 GetNodePos (float xCoord, float yCoord) {
        // Given coords are btwn 0-100, returns actual coordinates.
        return new Vector3(lowerLeft.x + xCoord * (mapDims.x / 100f), lowerLeft.y + yCoord * (mapDims.y / 100f), lowerLeft.z);
    }

    public GameLevel[] CreateGameLevels () {
        string nodeCSVPath = Application.dataPath + "/Files/Map/game_levels.csv";
        string fileData = File.ReadAllText(nodeCSVPath);
        string[] lines = fileData.Split('\n');
        int numLines = lines.Length;
        GameLevel[] levels = new GameLevel[numLines / 3];
        string[] cells, andReqsLine, orReqsLine;
        string levelName;
        float xCoord, yCoord;
        GameLevel gameLvl;
        GameObject node;
        string speakerImgFileName;
        for (int i = 0; i < numLines / 3; i++) {
            cells = utilsScript.FormatCSVLine(lines[3 * i]);
            levelName = cells[0];
            xCoord = float.Parse(cells[1]);
            yCoord = float.Parse(cells[2]);
            speakerImgFileName = cells[3];
            node = Instantiate(gameLevelNodeTemplate, GetNodePos(xCoord, yCoord), Quaternion.identity);
            node.transform.parent = nodesParent.transform;
            gameLvl = new GameLevel(levelName, node, speakerImgFileName);
            levels[i] = gameLvl;
            andReqsLine = utilsScript.FormatCSVLine(lines[3 * i + 1]);
            gameLvl.LoadANDReqs(andReqsLine);
            orReqsLine = utilsScript.FormatCSVLine(lines[3 * i + 2]);
            gameLvl.LoadORReqs(orReqsLine);
            gameLvl.TryToUnlockLevel();
            gameLvl.RefreshNodeColor();
        }
        return levels;
    }

    public ShopLevel[] CreateShopLevels () {
        string nodeCSVPath = Application.dataPath + "/Files/Map/shops.csv";
        string fileData = File.ReadAllText(nodeCSVPath);
        string[] lines = fileData.Split('\n');
        int numLines = lines.Length;
        ShopLevel[] levels = new ShopLevel[numLines / 3];
        string[] cells, andReqsLine, orReqsLine;
        string levelName;
        float xCoord, yCoord;
        ShopLevel shopLvl;
        GameObject node;
        string speakerImgFileName;
        ValueLabel valueLabelScript = scriptMaster.GetComponent<ValueLabel>();
        TextureScript textureScript = scriptMaster.GetComponent<TextureScript>();
        for (int i = 0; i < numLines / 3; i++) {
            cells = utilsScript.FormatCSVLine(lines[3 * i]);
            levelName = cells[0];
            xCoord = float.Parse(cells[1]);
            yCoord = float.Parse(cells[2]);
            speakerImgFileName = cells[3];
            node = Instantiate(shopLevelNodeTemplate, GetNodePos(xCoord, yCoord), Quaternion.identity);
            node.transform.parent = nodesParent.transform;
            shopLvl = new ShopLevel(levelName, node, speakerImgFileName);
            shopLvl.InitStock(utilsScript, valueLabelScript, textureScript, pieceObjTemplate);
            levels[i] = shopLvl;
            andReqsLine = utilsScript.FormatCSVLine(lines[3 * i + 1]);
            shopLvl.LoadANDReqs(andReqsLine);
            orReqsLine = utilsScript.FormatCSVLine(lines[3 * i + 2]);
            shopLvl.LoadORReqs(orReqsLine);
            shopLvl.TryToUnlockLevel();
            shopLvl.RefreshNodeColor();
        }
        return levels;
    }

    void Awake () {
        CalculateMapProps();
    }

    void Start () {
        utilsScript = scriptMaster.GetComponent<Utils>();
    }
}
