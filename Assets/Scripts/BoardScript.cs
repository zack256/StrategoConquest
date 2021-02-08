using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public GameObject randomPiecesBtn;

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
    private GameObject[,] boardTiles;
    private int numRows;
    private int numCols;
    private bool pieceScrollDisabled = false;
    private Dictionary<GameObject, Dictionary<string, string>> pieceData;
    private int playMode;
    private int[] lastPos = new int[] {-1, -1};

    private Vector2 boardDims;
    private Vector2 tileDims;
    private Vector2 lowerLeft;

    bool ObjectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    bool ObjectIsGoodPieceOnSelector (GameObject obj) {
        return obj.transform.parent == goodPiecesParent.transform;
    }

    void MoveScreenOverPiece (GameObject piece, bool showBorder = true, bool showScreen = true) {
        pieceHighlightScreen.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, pieceHighlightScreen.transform.position.z);
        pieceHighlightScreen.transform.GetChild(1).gameObject.SetActive(showBorder);   // 2nd child is border.
        pieceHighlightScreen.transform.GetChild(0).gameObject.SetActive(showScreen);    // 1st child is screen.
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
        if (playMode == 0) {
            grabbedOriginalPosBool = true;
            grabbedOriginalPos = obj.transform.position;
        }
    }

    void ResetGrabbedPiece () {
        if (playMode == 0) {    // resetting to selector.
            grabbedPiece.GetComponent<MeshCollider>().enabled = true;
            if (grabbedOriginalPosBool) {
                grabbedPiece.transform.position = grabbedOriginalPos;
            } else {
                grabbedPiece.SetActive(false);
            }
        } else if (playMode == 1) { // resetting to lastpos
            GameObject lastTile = boardTiles[lastPos[1], lastPos[0]];
            grabbedPiece.transform.position = lastTile.transform.position;
            board[lastPos[1], lastPos[0]] = grabbedPiece; 
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

    public Vector3 GetTilePos (int x, int y, float zPos = -0.0001f) {
        return new Vector3(lowerLeft[0] + (x + 0.5f) * tileDims[0], lowerLeft[1] + (y + 0.5f) * tileDims[1], zPos);
    }

    void ResetPiecesToSelector () {
        GameObject piece;
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if (board[i, j]) {
                    piece = board[i, j];
                    goodPiecesOnBoard[pieceData[piece]["value"]]--;
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

    void HideSetupObjs () {
        pieceMenuRect.transform.parent.gameObject.SetActive(false);
        confirmPiecesBtn.transform.parent.gameObject.SetActive(false);
    }

    void RandomizeGoodPieces () {
        // Randomizes the remaining pieces to the remaining squares.
        int numRemaining = goodPiecesParent.transform.childCount;
        GameObject[] remainingPieces = new GameObject[numRemaining];
        for (int i = 0; i < numRemaining; i++) {
            remainingPieces[i] = goodPiecesParent.transform.GetChild(i).gameObject;
        }
        scriptMaster.GetComponent<Utils>().FisherYatesShuffle(remainingPieces);
        int z = 0;
        string pieceVal;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 10; j++) {
                if (!board[i, j]) {
                    remainingPieces[z].transform.parent = boardPiecesParent.transform;
                    board[i, j] = remainingPieces[z];
                    remainingPieces[z].transform.position = GetTilePos(j, i);
                    pieceVal = pieceData[remainingPieces[z]]["value"];
                    if (goodPiecesOnBoard.ContainsKey(pieceVal)) {
                        goodPiecesOnBoard[pieceVal]++;
                    } else {
                        goodPiecesOnBoard[pieceVal] = 1;
                    }
                    remainingPieces[z].GetComponent<MeshCollider>().enabled = false;
                    remainingPieces[z].SetActive(true);
                    z++;
                }
            }
        }
    }

    void ChangeLastPos (int[] newPos) {
        lastPos = newPos;
        HighlightLastPos();
    }

    void HighlightLastPos () {
        if (IsLastPosLit()) {
            boardTiles[lastPos[1], lastPos[0]].GetComponent<Highlight2D>().MarkLastPos();
        }
    }

    bool IsLastPosLit () {
        return lastPos[0] != -1;
    }

    void ResetLastPos () {
        if (IsLastPosLit()) {
            boardTiles[lastPos[1], lastPos[0]].GetComponent<Highlight2D>().ResetColor();
        }
        lastPos = new int[2] {-1, -1};
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if ((!grabbedPiece) && (obj.transform.parent == confirmPiecesBtn.transform)) {
            if (mouseDown) {
                bool canConfirm = goodPiecesParent.transform.childCount == 0;
                confirmPiecesBtn.GetComponent<ControlBtns>().Highlight(!canConfirm);
                if (justClicked && canConfirm) {
                    HideSetupObjs();
                    string[,] enemyValues = scriptMaster.GetComponent<InitEnemy>().InitPieces();
                    gameObject.GetComponent<PiecesScript>().InitEnemyPieces(boardPiecesParent, board, enemyValues, pieceData, gameObject);
                    playMode = 1;
                }
            } else {
                confirmPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
            }
        } else {
            confirmPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
        }
        if ((!grabbedPiece) && (obj.transform.parent == resetPiecesBtn.transform)) {
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
        if ((!grabbedPiece) && (obj.transform.parent == randomPiecesBtn.transform)) {
            if (mouseDown) {
                if (justClicked) {
                    RandomizeGoodPieces();
                    pieceScrollDisabled = true;
                }
                randomPiecesBtn.GetComponent<ControlBtns>().Highlight();
            } else {
                randomPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
            }
        } else {
            randomPiecesBtn.GetComponent<ControlBtns>().ResetHighlight();
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
                        grabbedPiece.transform.position = obj.transform.position;
                        if (playMode == 0) {
                            grabbedPiece.transform.parent = boardPiecesParent.transform;
                            string pieceVal = pieceData[grabbedPiece]["value"];
                            if (goodPiecesOnBoard.ContainsKey(pieceVal)) {
                                goodPiecesOnBoard[pieceVal]++;
                            } else {
                                goodPiecesOnBoard[pieceVal] = 1;
                            }
                            grabbedOriginalPosBool = false;
                        }
                        grabbedPiece = null;
                    }
                } else {
                    ResetGrabbedPiece();
                }
                if (playMode == 0) {
                    ShowPiecesOnSelector();
                }
                HidePieceScreen();
                ResetLastPos();
            }
        } else {
            if (ObjectIsGoodPieceOnSelector(obj)) {
                // grabbing piece off selector.
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
            tileLoc = GetTileLoc(obj);
            if (mouseDown) {
                if (grabbedPiece) {
                    if ((board[tileLoc[1], tileLoc[0]]) || ((playMode == 0) && (!IsValidLocForGood(tileLoc)))) {
                        obj.GetComponent<Highlight2D>().MouseClicking();
                    } else {
                        if (!((IsLastPosLit()) && (lastPos.SequenceEqual(tileLoc)))) {
                            obj.GetComponent<Highlight2D>().MouseWillDrop();
                        }
                    }
                } else {
                    if ((board[tileLoc[1], tileLoc[0]]) && (pieceData[board[tileLoc[1], tileLoc[0]]]["team"] == "0")) {
                        // grabbing piece off board.
                        grabbedPiece = board[tileLoc[1], tileLoc[0]];
                        board[tileLoc[1], tileLoc[0]] = null;
                        if (playMode == 0) {
                            goodPiecesOnBoard[pieceData[grabbedPiece]["value"]]--;
                            grabbedPiece.transform.parent = goodPiecesParent.transform;
                        }
                        MoveScreenOverPiece(grabbedPiece);  // unnecessary, will do next frame.
                        if (playMode == 1) {
                            ChangeLastPos(tileLoc);
                        }
                        //ChangeLastPos(tileLoc);
                    } else {
                        obj.GetComponent<Highlight2D>().MouseClicking();
                    }
                }
            } else {
                // moving over tiles without clicking
                //if ((board[tileLoc[1], tileLoc[0]]) && (pieceData[board[tileLoc[1], tileLoc[0]]]["team"] == "0")) {
                    //MoveScreenOverPiece(board[tileLoc[1], tileLoc[0]], showScreen: false);
                //}
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
        HighlightLastPos();
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
                if (playMode == 0) {
                    ShowPiecesOnSelector();
                }
                HidePieceScreen();
                ResetLastPos();
            }
        } else {
            HidePieceScreen();
        }
        HighlightLastPos();
    }

    void ResizeTileTemplate () {
        Vector3 templateSize = boardSquareTemplate.GetComponent<Renderer>().bounds.size;
        float fracX = 1 / (templateSize.x / tileDims.x);
        float fracY = 1 / (templateSize.y / tileDims.y);
        boardSquareTemplate.transform.localScale = new Vector3(boardSquareTemplate.transform.localScale.x * fracX, boardSquareTemplate.transform.localScale.y * fracY, boardSquareTemplate.transform.localScale.z);
    }

    void InitBoardTiles () {

        ResizeTileTemplate();
        boardTiles = new GameObject[numRows, numCols];

        GameObject newTile;
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                newTile = Instantiate(boardSquareTemplate, GetTilePos(j, i), Quaternion.identity);
                newTile.transform.parent = tileParent.transform;
                boardTiles[i, j] = newTile;
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
        piecesDict = new Dictionary<string, GameObject[]>();
        pieceData = new Dictionary<GameObject, Dictionary<string, string>>();
        gameObject.GetComponent<PiecesScript>().InitPieceQuads("Good", goodPiecesParent, tileToScale, piecesDict, pieceData);
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
        FindNextPieceOrderIdx(isDown);
        pieceOrderIdx = Mod(pieceOrderIdx, pieceOrder.Length);
    }

    void TogglePieceLabels () {
        foreach(KeyValuePair<GameObject, Dictionary<string, string>> item in pieceData) {
            if (item.Value["team"] == "0") {
                scriptMaster.GetComponent<ValueLabel>().ToggleLabel(item.Key);
            }
        }
    }

    int Mod (int a, int b) {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    void InitBoardVars () {
        numRows = 10;
        numCols = 10;
        float boardWidth = gameObject.GetComponent<Renderer>().bounds.size[0];
        float boardHeight = gameObject.GetComponent<Renderer>().bounds.size[1];
        boardDims = new Vector2(boardWidth, boardHeight);
        float tileWidth = boardWidth / numCols;
        float tileHeight = boardHeight / numRows;
        tileDims = new Vector2(tileWidth, tileHeight);
        lowerLeft = new Vector2(gameObject.transform.position.x - boardWidth / 2, gameObject.transform.position.y - boardHeight / 2);
    }

    void Start()
    {
        playMode = 0;
        InitBoardVars();
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
        if (Input.GetKeyUp(KeyCode.L)) {
            TogglePieceLabels();
        }
    }
}
