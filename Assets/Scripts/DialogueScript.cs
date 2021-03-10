using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{

    //public int charsPerFrame = 1;     // +
    public int framesPerChar;       // ++

    private int charIdx = 0;
    private string msg = "";
    private int deltaFrame;

    void ClearText () {
        gameObject.GetComponent<Text>().text = "";
    }

    public void LoadMsg (string newMsg) {
        ClearText();
        msg = newMsg;
        charIdx = 0;
        deltaFrame = 0;
    }

    void UpdateText () {
        if (!PageFinished()) {
            if (deltaFrame % framesPerChar == 0) {
                gameObject.GetComponent<Text>().text += msg[charIdx];
                charIdx++;
            }
            deltaFrame++;
        }
    }

    public bool PageFinished () {
        return charIdx >= msg.Length;
    }

    void Update()
    {
        UpdateText();
    }
}
