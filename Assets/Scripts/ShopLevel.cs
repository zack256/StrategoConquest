using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLevel : MapNode
{

    private List<PieceObj> piecesStock;
    private List<ItemObj> itemsStock;

    public ShopLevel (string levelName, GameObject nodeObj, string speakerImgFileName) : base(levelName, nodeObj, speakerImgFileName, 1) {
    }

    public override string GetHoverLabelMessage () {
        return GetName() + " (shop)";
    }

    string GetTeamImagesPath (string team) {    // temporary
        string assetsDir = Application.dataPath;
        string piecesTeamsPath = assetsDir + "/Files/Images/Pieces/";
        return piecesTeamsPath + team + "/";
    }

    void InitPieceObjForStock (PieceObj po, ValueLabel valueLabelScript, TextureScript textureScript) {
        GameObject quadFront = po.GetFront();
        GameObject quadBack = po.GetBack();
        valueLabelScript.PositionLabel(quadFront);
        valueLabelScript.RenameLabel(quadFront, po.GetValue());
        Texture2D tex = textureScript.CreateTexture(GetTeamImagesPath("good") + po.GetValue() + ".png");
        quadFront.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public void InitStock (Utils utilsScript, ValueLabel valueLabelScript, TextureScript textureScript, GameObject pieceObjTemplate) {
        string stockPath = Application.dataPath + "/Files/Shops/Stocks/" + GetName() + ".csv";;
        string fileData = File.ReadAllText(stockPath);
        string[] lines = fileData.Split('\n');
        string[] cells;
        GameObject nodeObj = GetNodeObj();
        piecesStock = new List<PieceObj>();
        itemsStock = new List<ItemObj>();
        for (int i = 0; i < lines.Length; i++) {
            cells = utilsScript.FormatCSVLine(lines[i]);
            if (cells[1][0] == '~') {   // a piece
                int quan = Int32.Parse(cells[3]);
                string pieceVal = cells[1].Substring(1);
                for (int j = 0; j < quan; j++) {
                    GameObject pObj = GameObject.Instantiate(pieceObjTemplate, new Vector3(0, 0, 0), Quaternion.identity);
                    pObj.SetActive(false);
                    pObj.transform.parent = nodeObj.transform;
                    PieceObj po = new PieceObj(pObj, 0, pieceVal, true);
                    InitPieceObjForStock(po, valueLabelScript, textureScript);
                    piecesStock.Add(po);
                }
                
            } else {    // item

            }
        }
    }

    public override List<PieceObj> GetPiecesStock () {
        return piecesStock;
    }
    public override List<ItemObj> GetItemsStock () {
        return itemsStock;
    }

}
