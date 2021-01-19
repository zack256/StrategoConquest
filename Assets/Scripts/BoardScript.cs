using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject boardSquareTemplate;
    public GameObject tileParent;
    public GameObject pieceMenuUpArrow;
    public GameObject pieceMenuDownArrow;
    public GameObject pieceMenuRect;
    public GameObject goodPiecesParent;

    private GameObject currentlyOver;

    bool objectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    void ResetHighlighted (GameObject keep = null) {
        if ((currentlyOver != null) && ((keep == null) || (currentlyOver != keep))) {
            currentlyOver.GetComponent<Highlight2D>().ResetColor();
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown) {
        if ((objectIsTile(obj)) || (obj == pieceMenuUpArrow) || (obj == pieceMenuDownArrow)) {
            ResetHighlighted(obj);
            currentlyOver = obj;
            if (mouseDown) {
                currentlyOver.GetComponent<Highlight2D>().MouseClicking();
            } else {
                currentlyOver.GetComponent<Highlight2D>().MouseHover();
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown) {
        ResetHighlighted();
    }

    void ResizeTileTemplate (float tileWidth, float tileHeight) {
        Vector3 templateSize = boardSquareTemplate.GetComponent<Renderer>().bounds.size;
        float fracX = 1 / (templateSize[0] / tileWidth);
        float fracY = 1 / (templateSize[1] / tileHeight);
        boardSquareTemplate.transform.localScale = new Vector3(boardSquareTemplate.transform.localScale.x * fracX, boardSquareTemplate.transform.localScale.y * fracY, boardSquareTemplate.transform.localScale.z);
    }

    void InitBoardTiles (int numCols, int numRows) {
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

    void InitPieceSelector () {
        Dictionary<string, GameObject[]> piecesDict = gameObject.GetComponent<PiecesScript>().InitPieceQuads("Good", goodPiecesParent);
        string[] show = new string[] {"2", "3", "4", "5"};
        Vector3 newPos;
        float margin = 0.75f;
        Vector3 rectDims = pieceMenuRect.GetComponent<Renderer>().bounds.size;
        for (int i = 0; i < show.Length; i++) {
            newPos = new Vector3(pieceMenuRect.transform.position.x, pieceMenuRect.transform.position.y + rectDims.y / 2 - margin - i * rectDims.y / show.Length, 0);
            piecesDict[show[i]][0].transform.position = newPos;
        }
    }

    void Start()
    {
        InitBoardTiles(10, 10);
        InitPieceSelector();
    }

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
