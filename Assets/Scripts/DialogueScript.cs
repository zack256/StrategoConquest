using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{

    //public int charsPerFrame = 1;     // +
    public int framesPerChar;       // ++

    private int charIdx = 0;
    private int deltaFrame;
    private int currentPage = 0;
    private string[] lines;

    void ClearText () {
        gameObject.GetComponent<Text>().text = "";
    }

    void LoadMsg () {
        ClearText();
        charIdx = 0;
        deltaFrame = 0;
    }

    public void LoadDialogue (string[] dLines) {
        lines = dLines;
        currentPage = 0;
        LoadMsg();
    }

    public bool NextDialogue () {
        if (PageFinished()) {
            currentPage++;
            LoadMsg();
        } else {
            FinishPage();
        }
        return DialogueFinished();
    }

    void UpdateText () {
        if ((!DialogueFinished() && (!PageFinished()))) {
            if (deltaFrame % framesPerChar == 0) {
                gameObject.GetComponent<Text>().text += lines[currentPage][charIdx];
                charIdx++;
            }
            deltaFrame++;
        }
    }

    void FinishPage () {
        gameObject.GetComponent<Text>().text = lines[currentPage];
        charIdx = lines[currentPage].Length;
    }

    bool PageFinished () {
        return charIdx >= lines[currentPage].Length;
    }

    bool DialogueFinished () {
        return currentPage >= lines.Length;
    }

    void Update()
    {
        UpdateText();
    }
}
