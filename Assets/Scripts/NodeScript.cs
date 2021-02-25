using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NodeScript : MonoBehaviour
{

    public GameObject nodeTemplate;
    public GameObject nodesParent;

    public GameLevel[] CreateGameLevels () {
        string nodeCSVPath = Application.dataPath + "/Files/nodes.csv";
        string fileData = File.ReadAllText(nodeCSVPath);
        string[] lines = fileData.Split('\n');
        int numLines = lines.Length;
        //MapNode[] mapNodes = new MapNode[numLines / 3]; // data for each node comprises 3 lines.
        GameLevel[] levels = new GameLevel[numLines / 3];
        string[] cells, andReqsLine, orReqsLine;
        string levelName;
        float xCoord, yCoord;
        GameLevel gameLvl;
        GameObject node;
        for (int i = 0; i < numLines / 3; i++) {
            cells = lines[3 * i].Split(',');
            levelName = cells[0];
            xCoord = float.Parse(cells[1]);
            yCoord = float.Parse(cells[2]);
            node = Instantiate(nodeTemplate, new Vector3(xCoord, yCoord, -0.006f), Quaternion.identity);
            node.transform.parent = nodesParent.transform;
            gameLvl = new GameLevel(levelName, node);
            levels[i] = gameLvl;
            andReqsLine = lines[3 * i + 1].Split(',');
            gameLvl.LoadANDReqs(andReqsLine);
            orReqsLine = lines[3 * i + 2].Split(',');
            gameLvl.LoadORReqs(orReqsLine);
        }
        return levels;
    }
    /**
    void Start()
    {
        //CreateNodes();
    }
    void Update()
    {
        
    }
    **/
}
