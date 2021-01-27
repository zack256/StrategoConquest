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
    public GameObject scriptMaster;
    public GameObject pieceHighlightScreen;
    public GameObject backgroundObj;

    private GameObject currentlyOver;
    private GameObject currentPiece;
    private Dictionary<string, int> goodPiecesOnBoard;
    private string[] pieceOrder = new string[12] {"2", "3", "4", "5", "6", "7", "8", "9", "10", "S", "B", "F"};
    private int pieceOrderIdx = 0;
    private Dictionary<string, GameObject[]> piecesDict;
    private GameObject tileToScale;
    private GameObject grabbedPiece;
    private Vector3 grabbedOriginalPos;
    private GameObject[,] board;
    private int numRows;
    private int numCols;

    bool ObjectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    bool ObjectIsPiece (GameObject obj) {
        return obj.transform.parent == goodPiecesParent.transform;
    }

    void MoveScreenOverPiece (GameObject piece) {
        pieceHighlightScreen.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, pieceHighlightScreen.transform.position.z);
        pieceHighlightScreen.SetActive(true);
    }

    void HidePieceScreen () {
        pieceHighlightScreen.SetActive(false);
    }

    void ResetHighlighted (GameObject keep = null) {
        if ((currentlyOver != null) && ((keep == null) || (currentlyOver != keep))) {
            currentlyOver.GetComponent<Highlight2D>().ResetColor();
        }
    }

    void SetGrabbedPiece (GameObject obj) {
        obj.GetComponent<MeshCollider>().enabled = false;
        grabbedPiece = obj;
        grabbedOriginalPos = obj.transform.position;
    }

    void ResetGrabbedPiece () {
        grabbedPiece.GetComponent<MeshCollider>().enabled = true;
        grabbedPiece.transform.position = grabbedOriginalPos;
        grabbedPiece = null;
    }

    int[] GetTileLoc (GameObject obj) {
        float boardWidth = gameObject.GetComponent<Renderer>().bounds.size[0];
        float boardHeight = gameObject.GetComponent<Renderer>().bounds.size[1];
        float tileWidth = boardWidth / numCols;
        float tileHeight = boardHeight / numRows;
        float x = obj.transform.position.x - (gameObject.transform.position.x - boardWidth / 2);
        float y = obj.transform.position.y - (gameObject.transform.position.y - boardHeight / 2);
        return new int[2] {(int) (x / tileWidth), (int) (y / tileHeight)};
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if (grabbedPiece) {
            if (mouseDown) {
                grabbedPiece.transform.position = point;
                MoveScreenOverPiece(grabbedPiece);
                if (!ObjectIsTile(obj)) {
                    return;
                }
            } else {
                if (ObjectIsTile(obj)) {
                    int[] tileLoc = GetTileLoc(obj);
                    board[tileLoc[1], tileLoc[0]] = grabbedPiece;
                    grabbedPiece.transform.position = obj.transform.position;
                    grabbedPiece = null;
                } else {
                    ResetGrabbedPiece();
                }
                HidePieceScreen();
            }
        } else {
            if (ObjectIsPiece(obj)) {
                if (justClicked) {
                    //grabbedPiece = obj;
                    SetGrabbedPiece(obj);
                }
                MoveScreenOverPiece(obj);
            } else {
                HidePieceScreen();
            }
        }
        if ((ObjectIsTile(obj)) || (obj == pieceMenuUpArrow) || (obj == pieceMenuDownArrow)) {
            ResetHighlighted(obj);
            currentlyOver = obj;
            if (mouseDown) {
                if (grabbedPiece) {
                    currentlyOver.GetComponent<Highlight2D>().MouseWillDrop();
                } else {
                    currentlyOver.GetComponent<Highlight2D>().MouseClicking();
                }
            } else {
                currentlyOver.GetComponent<Highlight2D>().MouseHover();
            }
            if ((obj == pieceMenuUpArrow) && justClicked) {
                PieceSelectorScroll(true);  // up/down..
                ShowPiecesOnSelector();
            } else if ((obj == pieceMenuDownArrow) && justClicked) {
                PieceSelectorScroll(false);
                ShowPiecesOnSelector();
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
        ResetHighlighted();

        if (grabbedPiece) {
            if (mouseDown) {
                grabbedPiece.transform.position = point;
                MoveScreenOverPiece(grabbedPiece);
            } else {
                ResetGrabbedPiece();
                HidePieceScreen();
            }
        } else {
            HidePieceScreen();
        }
    }

    void ResizeTileTemplate (float tileWidth, float tileHeight) {
        Vector3 templateSize = boardSquareTemplate.GetComponent<Renderer>().bounds.size;
        float fracX = 1 / (templateSize[0] / tileWidth);
        float fracY = 1 / (templateSize[1] / tileHeight);
        boardSquareTemplate.transform.localScale = new Vector3(boardSquareTemplate.transform.localScale.x * fracX, boardSquareTemplate.transform.localScale.y * fracY, boardSquareTemplate.transform.localScale.z);
    }

    void InitBoardTiles () {
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

    void InitBoard () {
        board = new GameObject[numRows, numCols];
    }

    void ShowPiecesOnSelector () {
        string[] show = GetPiecesShownOnSelector();
        Vector3 newPos;
        Vector3 rectDims = pieceMenuRect.GetComponent<Renderer>().bounds.size;
        float pieceHeight = tileToScale.GetComponent<Renderer>().bounds.size.y;
        float marginBtwnPieces = (rectDims.y - 4 * pieceHeight) / 5;
        for (int i = 0; i < show.Length; i++) {
            newPos = new Vector3(pieceMenuRect.transform.position.x, pieceMenuRect.transform.position.y + rectDims.y / 2 - marginBtwnPieces - pieceHeight / 2 - i * (pieceHeight + marginBtwnPieces), -0.002f);
            piecesDict[show[i]][0].transform.position = newPos;
            piecesDict[show[i]][0].SetActive(true);
        }
    }

    void InitPieceSelector () {
        pieceOrderIdx = 0;
        tileToScale = tileParent.transform.GetChild(0).gameObject;
        piecesDict = gameObject.GetComponent<PiecesScript>().InitPieceQuads("Good", goodPiecesParent, tileToScale);
        goodPiecesOnBoard = new Dictionary<string, int>();
        ShowPiecesOnSelector();
    }

    string[] GetPiecesShownOnSelector () {
        string piece;
        string[] res = new string[4];
        int got = 0;
        int lenPieceOrder = pieceOrder.Length;
        for (int i = 0; i < lenPieceOrder; i++) {
            piece = pieceOrder[(pieceOrderIdx + i) % lenPieceOrder];
            if ((!goodPiecesOnBoard.ContainsKey(piece)) || (goodPiecesOnBoard[piece] < piecesDict[piece].Length)) {
                res[got] = piece;
                got++;
                if (got == 4) {
                    break;
                }
            }
        }
        return res;
    }

    void PieceSelectorScroll (bool isDown) {
        string[] shown = GetPiecesShownOnSelector();
        string pieceToRemove;
        if (isDown) {
            pieceToRemove = shown[0];
            pieceOrderIdx++;
        } else {
            pieceToRemove = shown[3];
            pieceOrderIdx--;
        }
        piecesDict[pieceToRemove][0].SetActive(false);
        pieceOrderIdx = Mod(pieceOrderIdx, pieceOrder.Length);
    }

    int Mod (int a, int b) {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    void Start()
    {
        numRows = 10;
        numCols = 10;
        InitBoardTiles();
        InitBoard();
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
            if (hit.transform.gameObject == backgroundObj){
                MouseHitNoGameObject(justClicked, mouseDown, hit.point);
            } else {
                MouseHitGameObject(hit.transform.gameObject, justClicked, mouseDown, hit.point);
            }
        }
        else
        {
            //MouseHitNoGameObject(justClicked, mouseDown);
        }
    }
}
