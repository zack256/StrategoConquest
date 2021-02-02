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
    public GameObject boardPiecesParent;
    public GameObject confirmPiecesBtn;
    public GameObject resetPiecesBtn;

    private GameObject currentlyOver;
    private GameObject currentPiece;
    private Dictionary<string, int> goodPiecesOnBoard;
    private string[] pieceOrder = new string[12] {"2", "3", "4", "5", "6", "7", "8", "9", "10", "S", "B", "F"};
    private int pieceOrderIdx = 0;
    private Dictionary<string, GameObject[]> piecesDict;
    private GameObject tileToScale;
    private GameObject grabbedPiece;
    private string grabbedPieceName;
    private Vector3 grabbedOriginalPos;
    private bool grabbedOriginalPosBool = true;
    public GameObject[,] board;
    private int numRows;
    private int numCols;
    private Dictionary<GameObject, string> pieceValues;
    private bool pieceScrollDisabled = false;

    bool ObjectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    bool ObjectIsPiece (GameObject obj) {
        return obj.transform.parent == goodPiecesParent.transform;
    }

    void MoveScreenOverPiece (GameObject piece, bool showBorder = true) {
        pieceHighlightScreen.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, pieceHighlightScreen.transform.position.z);
        pieceHighlightScreen.transform.GetChild(1).gameObject.SetActive(showBorder);   // 2nd child is border.
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
        grabbedOriginalPosBool = true;
        grabbedOriginalPos = obj.transform.position;
    }

    void ResetGrabbedPiece () {
        grabbedPiece.GetComponent<MeshCollider>().enabled = true;
        if (grabbedOriginalPosBool) {
            grabbedPiece.transform.position = grabbedOriginalPos;
        } else {
            grabbedPiece.SetActive(false);
        }
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

    void ResetPiecesToSelector () {
        GameObject piece;
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if (board[i, j]) {
                    piece = board[i, j];
                    goodPiecesOnBoard[pieceValues[piece]]--;
                    piece.transform.parent = goodPiecesParent.transform;
                    piece.GetComponent<MeshCollider>().enabled = true;
                    board[i, j] = null;
                }
            } 
        }
        ShowPiecesOnSelector();
    }

    bool IsValidLocForGood (int[] loc) {
        return loc[1] < 4;  // bottom 4 rows
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if ((!grabbedPiece) && (obj.transform.parent == confirmPiecesBtn.transform)) {
            MoveScreenOverPiece(obj, false);
            if (mouseDown) {
                bool canConfirm = goodPiecesParent.transform.childCount == 0;
                confirmPiecesBtn.GetComponent<ControlBtns>().Highlight(!canConfirm);
                if (justClicked && canConfirm) {
                    Debug.Log("hello :)");
                }
            } else {
                confirmPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
            }
        } else {
            confirmPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
        }
        if ((!grabbedPiece) && (obj.transform.parent == resetPiecesBtn.transform)) {
            MoveScreenOverPiece(obj, false);
            if (mouseDown) {
                if (justClicked) {
                    ResetPiecesToSelector();
                }
                resetPiecesBtn.GetComponent<ControlBtns>().Highlight();
            } else {
                resetPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
            }
        } else {
            resetPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
        }
        int[] tileLoc;
        if (grabbedPiece) {
            if (mouseDown) {
                grabbedPiece.transform.position = point;
                MoveScreenOverPiece(grabbedPiece);
                if (!ObjectIsTile(obj)) {
                    return;
                }
            } else {
                // releasing a grabbed tile.
                if (ObjectIsTile(obj)) {
                    tileLoc = GetTileLoc(obj);
                    if ((board[tileLoc[1], tileLoc[0]]) || (!IsValidLocForGood(tileLoc))) {
                        ResetGrabbedPiece();
                    } else {
                        // dropping piece on empty tile.
                        board[tileLoc[1], tileLoc[0]] = grabbedPiece;
                        grabbedPiece.transform.parent = boardPiecesParent.transform;
                        grabbedPiece.transform.position = obj.transform.position;
                        string pieceVal = pieceValues[grabbedPiece];
                        if (goodPiecesOnBoard.ContainsKey(pieceVal)) {
                            goodPiecesOnBoard[pieceVal]++;
                        } else {
                            goodPiecesOnBoard[pieceVal] = 1;
                        }
                        grabbedPiece = null;
                        grabbedOriginalPosBool = false;
                    }
                } else {
                    ResetGrabbedPiece();
                }
                ShowPiecesOnSelector();
                HidePieceScreen();
            }
        } else {
            if (ObjectIsPiece(obj)) {
                if (justClicked) {
                    SetGrabbedPiece(obj);
                }
                MoveScreenOverPiece(obj);
            } else {
                HidePieceScreen();
            }
        }
        if (ObjectIsTile(obj)) {
            ResetHighlighted(obj);
            currentlyOver = obj;
            if (mouseDown) {
                tileLoc = GetTileLoc(obj);
                if (grabbedPiece) {
                    if ((board[tileLoc[1], tileLoc[0]]) || (!IsValidLocForGood(tileLoc))) {
                        obj.GetComponent<Highlight2D>().MouseClicking();
                    } else {
                        obj.GetComponent<Highlight2D>().MouseWillDrop();
                    }
                } else {
                    if (board[tileLoc[1], tileLoc[0]]) {
                        // grabbing piece off board.
                        grabbedPiece = board[tileLoc[1], tileLoc[0]];
                        board[tileLoc[1], tileLoc[0]] = null;
                        goodPiecesOnBoard[pieceValues[grabbedPiece]]--;
                        grabbedPiece.transform.parent = goodPiecesParent.transform;
                        //cont.?
                    } else {
                        obj.GetComponent<Highlight2D>().MouseClicking();
                    }
                }
            } else {
                obj.GetComponent<Highlight2D>().MouseHover();
            }
        }
        if ((obj == pieceMenuUpArrow) || (obj == pieceMenuDownArrow)) {
            ResetHighlighted(obj);
            currentlyOver = obj;
            if (!pieceScrollDisabled) { // maybe fine if just lights up.
                if (mouseDown) {
                    currentlyOver.GetComponent<Highlight2D>().MouseClicking();
                } else {
                    currentlyOver.GetComponent<Highlight2D>().MouseHover();
                }
                if (justClicked) {
                    PieceSelectorScroll(obj == pieceMenuUpArrow);  // up/down..
                    ShowPiecesOnSelector();
                }
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
        confirmPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
        resetPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
        ResetHighlighted();

        if (grabbedPiece) {
            if (mouseDown) {
                grabbedPiece.transform.position = point;
                MoveScreenOverPiece(grabbedPiece);
            } else {
                ResetGrabbedPiece();
                ShowPiecesOnSelector();
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

    void HideSelectorPieces () {
        foreach (Transform t in goodPiecesParent.transform) {
            t.gameObject.SetActive(false);
        }
    }

    void ShowPiecesOnSelector () {
        HideSelectorPieces();
        string[] show = GetPiecesShownOnSelector();
        Vector3 newPos;
        Vector3 rectDims = pieceMenuRect.GetComponent<Renderer>().bounds.size;
        float pieceHeight = tileToScale.GetComponent<Renderer>().bounds.size.y;
        float marginBtwnPieces = (rectDims.y - 4 * pieceHeight) / 5;
        int idx;
        for (int i = 0; i < show.Length; i++) {
            if (show[i] != null) {
                newPos = new Vector3(pieceMenuRect.transform.position.x, pieceMenuRect.transform.position.y + rectDims.y / 2 - marginBtwnPieces - pieceHeight / 2 - i * (pieceHeight + marginBtwnPieces), -0.002f);
                idx = 0;
                if (!goodPiecesOnBoard.ContainsKey(show[i])) {
                } else {
                    //idx = goodPiecesOnBoard[show[i]];
                    while (true) {  // ;(
                        if (piecesDict[show[i]][idx].transform.parent == goodPiecesParent.transform) {
                            break;
                        }
                        idx++;
                    }
                }
                piecesDict[show[i]][idx].transform.position = newPos;
                piecesDict[show[i]][idx].SetActive(true);
            }
        }
    }

    void InitPieceSelector () {
        pieceOrderIdx = 0;
        tileToScale = tileParent.transform.GetChild(0).gameObject;
        //piecesDict = gameObject.GetComponent<PiecesScript>().InitPieceQuads("Good", goodPiecesParent, tileToScale);
        piecesDict = new Dictionary<string, GameObject[]>();
        pieceValues = new Dictionary<GameObject, string>();
        gameObject.GetComponent<PiecesScript>().InitPieceQuads("Good", goodPiecesParent, tileToScale, piecesDict, pieceValues);
        goodPiecesOnBoard = new Dictionary<string, int>();
        ShowPiecesOnSelector();
    }

    string[] GetPiecesShownOnSelector () {
        string piece;
        string[] res = new string[4];
        int got = 0;
        int lenPieceOrder = pieceOrder.Length;
        bool needToResetIdx = false;
        for (int i = 0; i < lenPieceOrder; i++) {
            piece = pieceOrder[(pieceOrderIdx + i) % lenPieceOrder];
            if ((!goodPiecesOnBoard.ContainsKey(piece)) || (goodPiecesOnBoard[piece] < piecesDict[piece].Length)) {
                got++;
                if (got == 5) {
                    break;
                }
                res[got - 1] = piece;
            } else if (i == 0) {
                needToResetIdx = true;
            }
        }
        if (needToResetIdx) {
            FindNextPieceOrderIdx(true);
        }
        pieceScrollDisabled = got <= 4;
        return res;
    }

    void FindNextPieceOrderIdx (bool isDown) {
        string piece;
        int sign = isDown ? 1 : -1;
        int originalIdx = -1;
        while (pieceOrderIdx != originalIdx) {
            if (originalIdx == -1) {
                originalIdx = pieceOrderIdx;
            }
            pieceOrderIdx += sign;
            pieceOrderIdx = Mod(pieceOrderIdx, pieceOrder.Length);
            piece = pieceOrder[pieceOrderIdx];
            if ((!goodPiecesOnBoard.ContainsKey(piece)) || (goodPiecesOnBoard[piece] < piecesDict[piece].Length)) {
                break;
            }
        }
    }

    void PieceSelectorScroll (bool isDown) {
        string[] shown = GetPiecesShownOnSelector();
        string pieceToRemove;
        if (isDown) {
            pieceToRemove = shown[0];
        } else {
            pieceToRemove = shown[3];
        }
        FindNextPieceOrderIdx(isDown);
        int idx;
        if (!goodPiecesOnBoard.ContainsKey(pieceToRemove)) {
            idx = 0;
        } else {
            idx = goodPiecesOnBoard[pieceToRemove];
        }
        piecesDict[pieceToRemove][idx].SetActive(false);
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
