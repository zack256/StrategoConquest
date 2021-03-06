﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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
    public GameObject continueBtn;
    public GameObject transparentSliderQuad;
    public GameObject gameOverText;
    public GameObject mapObj;

    private GameObject currentlyOver;
    private GameObject currentPiece;
    private Dictionary<string, int> goodPiecesOnBoard;
    //private string[] pieceOrder = new string[12] {"2", "3", "4", "5", "6", "7", "8", "9", "10", "S", "B", "F"};
    private string[] pieceOrder;
    private int pieceOrderIdx = 0;
    private Dictionary<string, GameObject[]> piecesDict;
    private GameObject tileToScale;
    private PieceObj grabbedPiece;
    private string grabbedPieceName;
    private Vector3 grabbedOriginalPos;
    private bool grabbedOriginalPosBool = true;
    public PieceObj[,] board;
    private GameObject[,] boardTiles;
    private int numRows;
    private int numCols;
    private bool pieceScrollDisabled = false;
    private int playMode;
    private int[] lastPos = new int[] {-1, -1};
    private Dictionary<GameObject, PieceObj> pOMap;
    private int[] currentlyTurning = new int[] {-1, -1};
    private int previousFightResult;
    private int[] combatantLoc;
    private int[] defenderLoc;  // ehh
    private int[,] currentlyFading = new int [1, 1];
    private bool isFading = false;
    private GameObject scaledTS;
    private Vector2 boardDims;
    private Vector2 tileDims;
    private Vector2 lowerLeft;
    private Player player;
    private MapNode currentLevel;
    private int winner;

    bool ObjectIsTile (GameObject obj) {
        return obj.transform.parent == tileParent.transform;
    }

    bool ObjectIsGoodPieceOnSelector (GameObject obj) {
        return obj.transform.parent == goodPiecesParent.transform;
    }

    PieceObj GetFromBoard (int[] loc) {
        if (loc[0] == -1) {
            return null;
        }
        return board[loc[1], loc[0]];
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

    void SetGrabbedPiece (PieceObj obj) {
        obj.ToggleMeshCollider(false);
        grabbedPiece = obj;
        if (playMode == 0) {
            grabbedOriginalPosBool = true;
            grabbedOriginalPos = obj.GetMain().transform.position;
        }
    }

    void ResetGrabbedPiece () {
        if (playMode == 0) {    // resetting to selector.
            grabbedPiece.ToggleMeshCollider(true);
            if (grabbedOriginalPosBool) {
                grabbedPiece.MoveToPos(grabbedOriginalPos);
            } else {
                grabbedPiece.GetMain().SetActive(false);
            }
        } else if (playMode == 1) { // resetting to lastpos
            GameObject lastTile = boardTiles[lastPos[1], lastPos[0]];
            grabbedPiece.MoveToPos(lastTile);
            board[lastPos[1], lastPos[0]] = grabbedPiece; 
        }
        grabbedPiece = null;
    }

    int[] GetTileLoc (GameObject obj) {
        float x = obj.transform.position.x - (gameObject.transform.position.x - boardDims[0] / 2);
        float y = obj.transform.position.y - (gameObject.transform.position.y - boardDims[1] / 2);
        return new int[2] {(int) (x / tileDims[0]), (int) (y / tileDims[1])};
    }

    public Vector3 GetTilePos (int x, int y, float zPos = -0.0001f) {
        return new Vector3(lowerLeft[0] + (x + 0.5f) * tileDims[0], lowerLeft[1] + (y + 0.5f) * tileDims[1], zPos);
    }

    void ResetPiecesToSelector () {
        PieceObj po;
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if (board[i, j]) {
                    po = board[i, j];
                    goodPiecesOnBoard[po.GetValue()]--;
                    po.GetMain().transform.parent = goodPiecesParent.transform;
                    po.ToggleMeshCollider(true);
                    board[i, j] = null;
                }
            } 
        }
        ShowPiecesOnSelector();
    }

    bool IsValidLocForGood (int[] loc) {
        return loc[1] < 4;  // bottom 4 rows
    }

    void ToggleSetupObjs (bool enable) {
        pieceMenuRect.transform.parent.gameObject.SetActive(enable);
        confirmPiecesBtn.transform.parent.gameObject.SetActive(enable);
    }

    void RandomizeGoodPieces () {
        // Randomizes the remaining pieces to the remaining squares.
        int numRemaining = goodPiecesParent.transform.childCount;
        PieceObj[] remainingPieces = new PieceObj[numRemaining];
        for (int i = 0; i < numRemaining; i++) {
            remainingPieces[i] = pOMap[goodPiecesParent.transform.GetChild(i).gameObject];
        }
        scriptMaster.GetComponent<Utils>().FisherYatesShuffle(remainingPieces);
        int z = 0;
        string pieceVal;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 10; j++) {
                if (z >= numRemaining) {
                    return;
                }
                if (!board[i, j]) {
                    remainingPieces[z].GetMain().transform.parent = boardPiecesParent.transform;
                    board[i, j] = remainingPieces[z];
                    remainingPieces[z].MoveToPos(GetTilePos(j, i));
                    pieceVal = remainingPieces[z].GetValue();
                    if (goodPiecesOnBoard.ContainsKey(pieceVal)) {
                        goodPiecesOnBoard[pieceVal]++;
                    } else {
                        goodPiecesOnBoard[pieceVal] = 1;
                    }
                    remainingPieces[z].ToggleMeshCollider(false);
                    remainingPieces[z].GetMain().SetActive(true);
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
    
    void MovePieceToTile (int[] newPos, GameObject newTile) {
        //board[lastPos[1], lastPos[0]] = null;
        board[newPos[1], newPos[0]] = grabbedPiece;
        grabbedPiece.MoveToPos(newTile);
        //grabbedPiece = null;
    }

    void RemovePieceOffBoard (int[] pos) {
        PieceObj piece = board[pos[1], pos[0]];
        board[pos[1], pos[0]] = null;
        piece.GetMain().SetActive(false);
    }

    void RemoveGrabbedPiece () {
        grabbedPiece.GetMain().SetActive(false);
    }

    bool CanGrabOffBoard (int[] pos) {
        if (playMode > 1) {
            return false;
        }
        PieceObj po;
        if (board[pos[1], pos[0]]) {
            po = GetFromBoard(pos);
        } else {
            return false;
        }
        return  (po.GetTeam() == 0) && ((playMode == 0) || ((po.GetValue() != "B") && (po.GetValue() != "F")));
    }

    void StartTurning (int[] turnerLoc) {
        currentlyTurning = turnerLoc;
        GetFromBoard(currentlyTurning).StartTurning();
    }

    void DeleteAllPieces () {
        foreach (Transform child in boardPiecesParent.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        if ((!grabbedPiece) && (obj.transform.parent == confirmPiecesBtn.transform)) {
            if (mouseDown) {
                bool canConfirm = goodPiecesParent.transform.childCount == 0;
                confirmPiecesBtn.GetComponent<ControlBtns>().Highlight(!canConfirm);
                if (justClicked && canConfirm) {
                    ToggleSetupObjs(false);
                    string[,] enemyValues = scriptMaster.GetComponent<InitEnemy>().InitPieces();
                    gameObject.GetComponent<PiecesScript>().InitEnemyPieces(boardPiecesParent, board, enemyValues, gameObject, currentLevel.GetName());
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
        if (obj.transform.parent == continueBtn.transform) {
            if (mouseDown) {
                if (justClicked) {
                    scriptMaster.GetComponent<TransitionScript>().TransitionToMap(winner);
                }
                continueBtn.GetComponent<ControlBtns>().Highlight();
            } else {
                continueBtn.GetComponent<ControlBtns>().ResetHighlight();
            }
        } else {
            continueBtn.GetComponent<ControlBtns>().ResetHighlight();
        }
        int[] tileLoc;
        if (grabbedPiece) { // playmode is either 0 or 1.
            if (mouseDown) {
                grabbedPiece.MoveToPos(point);
                MoveScreenOverPiece(grabbedPiece.GetMain());
                if (!ObjectIsTile(obj)) {
                    return;
                }
            } else {
                // releasing a grabbed tile.
                if (ObjectIsTile(obj)) {
                    tileLoc = GetTileLoc(obj);
                    if (playMode == 1) {
                        if (board[tileLoc[1], tileLoc[0]]) {
                            previousFightResult = scriptMaster.GetComponent<GameScript>().FightResult(board, lastPos, tileLoc, grabbedPiece);
                            ResetGrabbedPiece();
                            if (previousFightResult == -1) {        // invalid
                            } else {
                                combatantLoc = lastPos;
                                defenderLoc = tileLoc;
                                playMode = 3;   // fight animation
                                StartTurning(tileLoc);
                            }
                        } else {    // dropping on empty tile.
                            if (scriptMaster.GetComponent<GameScript>().IsValidMove(board, lastPos, tileLoc, grabbedPiece)) {
                                MovePieceToTile(tileLoc, obj);
                                BaseCPUTurn();
                            } else {
                                ResetGrabbedPiece();
                            }
                        }
                        grabbedPiece = null;
                    } else {
                        // dropping on occupied tile.
                        if ((board[tileLoc[1], tileLoc[0]]) || (!IsValidLocForGood(tileLoc))) {
                            ResetGrabbedPiece();
                        } else {
                            // dropping piece on empty tile.
                            board[tileLoc[1], tileLoc[0]] = grabbedPiece;
                            grabbedPiece.MoveToPos(obj);
                            grabbedPiece.GetMain().transform.parent = boardPiecesParent.transform;
                            string pieceVal = grabbedPiece.GetValue();
                            if (goodPiecesOnBoard.ContainsKey(pieceVal)) {
                                goodPiecesOnBoard[pieceVal]++;
                            } else {
                                goodPiecesOnBoard[pieceVal] = 1;
                            }
                            grabbedOriginalPosBool = false;
                            grabbedPiece = null;
                        }
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
                    SetGrabbedPiece(pOMap[obj]);
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
                    if (CanGrabOffBoard(tileLoc)) {
                        // grabbing piece off board.
                        grabbedPiece = board[tileLoc[1], tileLoc[0]];
                        board[tileLoc[1], tileLoc[0]] = null;
                        if (playMode == 0) {
                            goodPiecesOnBoard[grabbedPiece.GetValue()]--;
                            grabbedPiece.GetMain().transform.parent = goodPiecesParent.transform;
                        }
                        MoveScreenOverPiece(grabbedPiece.GetMain());  // unnecessary, will do next frame.
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
            if (!pieceScrollDisabled) {
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
                grabbedPiece.MoveToPos(point);
                MoveScreenOverPiece(grabbedPiece.GetMain());
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

        tileToScale = tileParent.transform.GetChild(0).gameObject;
    }

    void InitBoard () {
        board = new PieceObj[numRows, numCols];
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

    void CopyTo2DList (int [,] list2D, int[] list1D, int row) {
        for (int i = 0; i < list1D.Length; i++) {
            list2D[row, i] = list1D[i];
        }
    }

    void MoveTSlidersOverPieces () {
        PieceObj po;
        GameObject tslider;
        int[] loc;
        for (int i = 0; i < currentlyFading.GetLength(0); i++) {
            loc = new int[2] {currentlyFading[i, 0], currentlyFading[i, 1]};
            po = GetFromBoard(loc);
            tslider = Instantiate(scaledTS, po.GetMain().transform.position, Quaternion.identity);
            tslider.transform.position = new Vector3 (tslider.transform.position.x, tslider.transform.position.y, -0.03f);
            po.SetTransparentSlider(tslider);
            po.StartFading();
        }
    }

    void DisplayGameOverText () {
        string[] messages = new string[] {"Victory!", "Defeat!", "Tie!"};
        GameObject textObj = gameOverText.transform.GetChild(0).GetChild(0).gameObject;
        textObj.GetComponent<Text>().text = messages[winner];
        gameOverText.SetActive(true);
    }

    void SetUpGameOver () {   // 0 = human, 1 = cpu, 2 = tie (?+)
        playMode = 5;
        DisplayGameOverText();
        continueBtn.SetActive(true);
    }

    void BaseCPUTurn () {
        playMode = 2;   // cpu turn
        int[,] res = scriptMaster.GetComponent<GameScript>().AIRandomMove0(board);
        int[] startPos = new int[] {res[0, 1], res[0, 0]};  // yx -> xy
        int[] destPos = new int[] {res[1, 1], res[1, 0]};
        PieceObj attacker = GetFromBoard(startPos);
        PieceObj defender = GetFromBoard(destPos);
        if (!defender) {    // no attack
            board[startPos[1], startPos[0]] = null;
            board[destPos[1], destPos[0]] = attacker;
            attacker.MoveToPos(GetTilePos(destPos[0], destPos[1]));
            playMode = 1;
        } else {
            combatantLoc = startPos;
            defenderLoc = destPos;
            StartTurning(startPos);
        }
    }

    void ControlBattleResults (int stage, int fadingIdx = 0) {
        if (stage == 0) {   // enemy revealed, now:
            if (previousFightResult == 0) { // attacker wins
                currentlyFading = new int[1, 2];
                CopyTo2DList(currentlyFading, currentlyTurning, 0);
            } else if (previousFightResult == 1) {  // defender wins
                currentlyFading = new int[1, 2];
                CopyTo2DList(currentlyFading, combatantLoc, 0);
            } else {    // tie
                currentlyFading = new int[2, 2];
                CopyTo2DList(currentlyFading, combatantLoc, 0);
                CopyTo2DList(currentlyFading, currentlyTurning, 1);
            }
            MoveTSlidersOverPieces();
            currentlyTurning = new int[] {-1, -1};
            isFading = true;
        } else if (stage == 1) {    // piece(s) have faded
            PieceObj fadingObj = board[currentlyFading[fadingIdx, 1], currentlyFading[fadingIdx, 0]];
            fadingObj.GetMain().SetActive(false);
            fadingObj.RemoveTransparentSlider();
            if (previousFightResult == 0) {
                PieceObj combatant = GetFromBoard(combatantLoc);
                board[combatantLoc[1], combatantLoc[0]] = null;
                board[currentlyFading[0, 1], currentlyFading[0, 0]] = combatant;
                combatant.MoveToPos(fadingObj.GetMain());
            } else if (previousFightResult == 1) {
                board[currentlyFading[0, 1], currentlyFading[0, 0]] = null;
                StartTurning(defenderLoc);
            } else {
                board[currentlyFading[fadingIdx, 1], currentlyFading[fadingIdx, 0]] = null;
            }
            CopyTo2DList(currentlyFading, new int[] {-1, -1}, fadingIdx);
            fadingObj.MoveToPos(new Vector3(40, 0, 0)); // eh
            if (fadingObj.GetValue() == "F") {
                winner = 0;
                SetUpGameOver(); return;
            }
            if ((previousFightResult != 1) && (fadingIdx == currentlyFading.GetLength(0) - 1)) {
                BaseCPUTurn();
            }
        } else if (stage == 2) {    // victorious defender has turned back around
            currentlyTurning = new int[] {-1, -1};
            BaseCPUTurn();
        } else if (stage == 3) {    // enemy attacker has just turned around.
            previousFightResult = scriptMaster.GetComponent<GameScript>().FightResult(board, combatantLoc, defenderLoc, GetFromBoard(combatantLoc));
            if (previousFightResult == 0) { // attacker wins
                currentlyFading = new int[1, 2];
                CopyTo2DList(currentlyFading, defenderLoc, 0);
            } else if (previousFightResult == 1) {  // defender wins
                currentlyFading = new int[1, 2];
                CopyTo2DList(currentlyFading, combatantLoc, 0);
            } else {    // tie
                currentlyFading = new int[2, 2];
                CopyTo2DList(currentlyFading, combatantLoc, 0);
                CopyTo2DList(currentlyFading, defenderLoc, 1);
            }
            MoveTSlidersOverPieces();
            currentlyTurning = new int[] {-1, -1};
            isFading = true;
        } else if (stage == 4) {    // pieces have faded.
            PieceObj fadingObj = board[currentlyFading[fadingIdx, 1], currentlyFading[fadingIdx, 0]];
            fadingObj.GetMain().SetActive(false);
            fadingObj.RemoveTransparentSlider();
            if (previousFightResult == 0) {
                PieceObj combatant = GetFromBoard(combatantLoc);
                board[combatantLoc[1], combatantLoc[0]] = null;
                board[defenderLoc[1], defenderLoc[0]] = combatant;
                combatant.MoveToPos(fadingObj.GetMain());
                if (fadingObj.GetValue() == "F") {
                    winner = 1;
                    SetUpGameOver(); return;
                } else {
                    StartTurning(defenderLoc);  // cpu attacked and won, so turns now.
                }
            } else if (previousFightResult == 1) {
                board[currentlyFading[0, 1], currentlyFading[0, 0]] = null;
            } else {
                board[currentlyFading[fadingIdx, 1], currentlyFading[fadingIdx, 0]] = null;
            }
            fadingObj.MoveToPos(new Vector3(40, 0, 0)); // eh
            if (fadingIdx == currentlyFading.GetLength(0) - 1) {
                //currentlyFading = new int[] {-1, -1};
                if (previousFightResult != 0) {
                    playMode = 1;
                }
            }
        } else if (stage == 5) {    // enemy piece turned around after attacking.
            currentlyTurning = new int[] {-1, -1};
            playMode = 1;
        }
    }

    void InitPieceSelector () {
        pieceOrderIdx = 0;
        piecesDict = new Dictionary<string, GameObject[]>();
        pOMap = new Dictionary<GameObject, PieceObj>();
        player = mapObj.GetComponent<MapScript>().GetPlayer();
        gameObject.GetComponent<PiecesScript>().InitGoodPieces("Good", goodPiecesParent, tileToScale, piecesDict, pOMap, player);
        goodPiecesOnBoard = new Dictionary<string, int>();
        pieceOrder = player.CreatePieceOrder();
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
        PieceObj po;
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                po = board[i, j];
                if ((po) && (po.GetTeam() == 0)) {
                    po.ToggleLabel();
                }
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

    void InitTSlider () {
        scaledTS = Instantiate(transparentSliderQuad, new Vector3(100, 0, 0), Quaternion.identity);
        scriptMaster.GetComponent<ScaleScript>().ScaleGameObject(scaledTS, tileToScale);
        scaledTS.transform.parent = gameObject.transform.parent;
    }

    public void InitSetupPhase (MapNode gl) {   // gl should be a gamelevel :)
        currentLevel = gl;
        gameOverText.SetActive(false);
        continueBtn.SetActive(false);
        DeleteAllPieces();
        InitBoard();
        InitPieceSelector();
        ToggleSetupObjs(true);
        playMode = 0;
    }

    void Awake()
    {
        InitBoardVars();
        InitBoardTiles();
        //InitBoard();
        InitTSlider();
        //InitSetupPhase();
    }

    void Update()
    {
        Ray ray = cameraObject.GetComponent<CameraScript>().GetCameraRay();
        RaycastHit hit;
        bool justClicked = Input.GetMouseButtonDown(0);
        bool mouseDown = Input.GetMouseButton(0);

        PieceObj turnerObj = GetFromBoard(currentlyTurning);
        if (turnerObj) {
            if (!turnerObj.IsTurning()) {
                turnerObj.StopTurning();
                if (playMode == 2) {  // turning piece is attacking
                    if (turnerObj.IsFaceUp()) {
                        ControlBattleResults(3);    // starting attacking
                    } else {
                        ControlBattleResults(5);    // finished attacking
                    }
                } else {
                    if (turnerObj.IsFaceUp()) { // is defending
                        ControlBattleResults(0);
                    } else {
                        ControlBattleResults(2);
                    }
                }
            } else {
                turnerObj.UpdateTurn();
            }
        }

        PieceObj fadingObj;
        if (isFading) {
            for (int i = 0; i < currentlyFading.GetLength(0); i++) {
                fadingObj = board[currentlyFading[i, 1], currentlyFading[i, 0]];
                fadingObj.UpdateFade();
                if (!fadingObj.IsFading()) {
                    isFading = false;
                    if (playMode == 2) {
                        ControlBattleResults(4, i);
                    } else {
                        ControlBattleResults(1, i);
                    }
                }
            }
        }

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
