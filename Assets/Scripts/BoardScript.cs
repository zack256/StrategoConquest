using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject boardSquareTemplate;
    public GameObject tileParent;

    private GameObject currentTile;

    bool objectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    void ResetHighlightedTile (GameObject keepTile = null) {
        //Debug.Log(keepTile);
        //Debug.Log(currentTile);
        //Debug.Log(keepTile == currentTile);
        if ((currentTile != null) && ((keepTile == null) || (currentTile != keepTile))) {
            currentTile.GetComponent<HighlightTile>().ResetColor();
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown) {
        if (objectIsTile(obj)) {
            ResetHighlightedTile(obj);
            currentTile = obj;
            if (mouseDown) {
                currentTile.GetComponent<HighlightTile>().MouseClicking();
            } else {
                currentTile.GetComponent<HighlightTile>().MouseHover();
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown) {
        ResetHighlightedTile();
    }

    void ResizeTileTemplate (float tileWidth, float tileHeight) {
        Vector3 templateSize = boardSquareTemplate.GetComponent<Renderer>().bounds.size;
        float fracX = 1 / (templateSize[0] / tileWidth);
        float fracY = 1 / (templateSize[1] / tileHeight);
        boardSquareTemplate.transform.localScale = new Vector3(boardSquareTemplate.transform.localScale.x * fracX, boardSquareTemplate.transform.localScale.y * fracY, boardSquareTemplate.transform.localScale.z);
    }

    void InitTiles (int numCols, int numRows) {
        float boardWidth = gameObject.GetComponent<Renderer>().bounds.size[0];
        float boardHeight = gameObject.GetComponent<Renderer>().bounds.size[1];
        float tileWidth = boardWidth / numCols;
        float tileHeight = boardHeight / numRows;
        Vector2 lowerLeft = new Vector2(gameObject.transform.position.x - boardWidth / 2, gameObject.transform.position.y - boardHeight / 2);
        float xPos, yPos;
        float tileZ = -0.0001f;

        ResizeTileTemplate(tileWidth, tileHeight);

        GameObject newTile;
        for (int i = 0; i < numRows; i++) {
            yPos = lowerLeft[1] + (i + 0.5f) * tileHeight;
            for (int j = 0; j < numCols; j++) {
                xPos = lowerLeft[0] + (j + 0.5f) * tileWidth;
                newTile = Instantiate(boardSquareTemplate, new Vector3(xPos, yPos, tileZ), Quaternion.identity);
                newTile.transform.parent = tileParent.transform;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitTiles(10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cameraObject.GetComponent<CameraScript>().GetCameraRay();
        RaycastHit hit;
        bool justClicked = Input.GetMouseButtonDown(0);
        bool mouseDown = Input.GetMouseButton(0);
        if (Physics.Raycast(ray, out hit))
        {
            MouseHitGameObject(hit.transform.gameObject, justClicked, mouseDown);   // sidenote : apparently hit.collider.transform.gameObject is different - see https://forum.unity.com/threads/raycast-on-child-gives-parent-name-in-hit-transform-name.57172/ 
        }
        else
        {
            MouseHitNoGameObject(justClicked, mouseDown);
        }
    }
}
