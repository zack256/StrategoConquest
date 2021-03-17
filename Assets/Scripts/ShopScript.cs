using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject backgroundObj;

    void MouseHitGameObject (GameObject obj, bool justClicked, bool mouseDown, Vector3 point) {

    }

    void MouseHitNoGameObject (bool justClicked, bool mouseDown, Vector3 point) {
    }

    public void SetUpShop (MapNode sl) {
    }

    void Start()
    {
        
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
