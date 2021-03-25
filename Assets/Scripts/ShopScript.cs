using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject backgroundObj;
    //public GameObject descWindow;
    public GameObject scriptMaster;
    public GameObject stockRect;
    public GameObject stockMenuUpArrow;
    public GameObject stockMenuDownArrow;

    private MapNode shopLevel;
    private Utils utilsScript;

    private int scrollIndex;
    private List<PieceObj> pieceStock;
    private List<ItemObj> itemStock;

    private GameObject currentlyOver = null;
    private bool pieceScrollDisabled = false;

    void ResetHighlighted () {
        if (currentlyOver) {
            currentlyOver.GetComponent<Highlight2D>().ResetColor();
        }
    }

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {
        ResetHighlighted();
        if ((obj == stockMenuUpArrow) || (obj == stockMenuDownArrow)) {
            currentlyOver = obj;
            if (!pieceScrollDisabled) {
                if (mouseDown) {
                    currentlyOver.GetComponent<Highlight2D>().MouseClicking();
                } else {
                    currentlyOver.GetComponent<Highlight2D>().MouseHover();
                }
                if (justClicked) {
                }
            }
        }
    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
        ResetHighlighted();
    }

    string[] FormatCSVLine (string line) {  // probably should combine these into a utils file
        string[] res = line.Trim().Split(',');
        for (int i = 0; i < res.Length; i++) {
            res[i] = res[i].Trim();
        }
        return res;
    }

    void DisplayItemsOnScroll () {
        int index;
        int itemsLeftInStock = pieceStock.Count + itemStock.Count;
        if (itemsLeftInStock == 0) {
            return;
        }
        float stockRectHeight = stockRect.GetComponent<Renderer>().bounds.size.y;
        int index0 = Mod(scrollIndex, itemsLeftInStock);
        for (int i = 0; i < 4; i++) {
            index = Mod(scrollIndex + i, itemsLeftInStock);
            if ((i != 0) && (index == index0)) {
                break;
            }
            if (index < pieceStock.Count) { // bad positioning, need to redo.
                pieceStock[index].MoveToPos(new Vector3(stockRect.transform.position.x, stockRect.transform.position.y + (2 * i - 3) * (stockRectHeight / 8.0f), stockRect.transform.position.z));
                pieceStock[index].GetMain().SetActive(true);
            } else {

            }
        }
    }

    int Mod (int a, int b) {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    public void SetUpShop (MapNode sl) {
        //gameObject.SetActive(true);
        //descWindow.SetActive(true);
        shopLevel = sl;
        scrollIndex = 0;
        pieceStock = sl.GetPiecesStock();
        itemStock = sl.GetItemsStock();
        for (int i = 0; i < pieceStock.Count; i++) {
            pieceStock[i].GetMain().transform.parent = stockRect.transform;
        }
        for (int i = 0; i < itemStock.Count; i++) {
            //itemStock[i].GetMain().transform.parent = stockRect.transform;
        }
        DisplayItemsOnScroll();
    }

    void Start()
    {
        utilsScript = scriptMaster.GetComponent<Utils>();
        
    }

    void Update () {
        Ray ray = cameraObject.GetComponent<CameraScript>().GetCameraRay();
        RaycastHit hit;
        bool justClicked = Input.GetMouseButtonUp(0);
        bool mouseDown = Input.GetMouseButton(0);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == backgroundObj){
                MouseHitNoGameObject(justClicked, mouseDown, hit.point);
            } else {
                MouseHitGameObject(hit.transform.gameObject, justClicked, mouseDown, hit.point);
            }
        }
    }
}
