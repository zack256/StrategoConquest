using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject backgroundObj;
    //public GameObject descWindow;
    public GameObject scriptMaster;

    private MapNode shopLevel;
    private Utils utilsScript;

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {

    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
    }

    string[] FormatCSVLine (string line) {  // probably should combine these into a utils file
        string[] res = line.Trim().Split(',');
        for (int i = 0; i < res.Length; i++) {
            res[i] = res[i].Trim();
        }
        return res;
    }

    public void SetUpShop (MapNode sl) {
        gameObject.SetActive(true);
        //descWindow.SetActive(true);
        shopLevel = sl;
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
