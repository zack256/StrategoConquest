using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NodeScript : MonoBehaviour
{

    public GameObject nodeTemplate;
    public GameObject nodesParent;

    private Vector3 lowerLeft;
    private Vector3 mapDims;

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
            node = Instantiate(nodeTemplate, GetNodePos(xCoord, yCoord), Quaternion.identity);
            node.transform.parent = nodesParent.transform;
            gameLvl = new GameLevel(levelName, node);
            levels[i] = gameLvl;
            andReqsLine = lines[3 * i + 1].Trim().Split(',');
            gameLvl.LoadANDReqs(andReqsLine);
            orReqsLine = lines[3 * i + 2].Trim().Split(',');
            gameLvl.LoadORReqs(orReqsLine);
            gameLvl.TryToUnlockLevel();
            node.GetComponent<HighlightNode>().SetAccessLevel(gameLvl);
        }
        return levels;
    }

    void Awake () {
        CalculateMapProps();
    }

    void Start () {
        
    }
}
