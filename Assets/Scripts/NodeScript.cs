using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NodeScript : MonoBehaviour
{

    public GameObject nodeTemplate;

    void CreateNodes () {
        string nodeCSVPath = Application.dataPath + "/Files/nodes.csv";
        string fileData = File.ReadAllText(nodeCSVPath);
        string[] lines = fileData.Split('\n');
        int numLines = lines.Length;
        //MapNode[] mapNodes = new MapNode[numLines / 3]; // data for each node comprises 3 lines.
        string[] cells, andReqsLine, orReqsLine;
        string levelName;
        float xCoord, yCoord;
        GameLevel gameLvl;
        for (int i = 0; i < numLines / 3; i++) {
            cells = lines[3 * i].Split(',');
            levelName = cells[0];
            xCoord = float.Parse(cells[1]);
            yCoord = float.Parse(cells[2]);
            Instantiate(nodeTemplate, new Vector3(xCoord, yCoord, 0), Quaternion.identity);
            gameLvl = new GameLevel(levelName);
            andReqsLine = lines[3 * i + 1].Split(',');
            gameLvl.LoadANDReqs(andReqsLine);
            orReqsLine = lines[3 * i + 2].Split(',');
            gameLvl.LoadORReqs(orReqsLine);
        }
    }

    void Start()
    {
        CreateNodes();
    }

    void Update()
    {
        
    }
}
