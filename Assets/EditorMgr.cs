using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMgr : MonoBehaviour
{

    SelectRect selectRect;
    GridMgr gridMgr;
    ColorMgr colorMgr;

	// Use this for initialization
	void Awake ()
    {
        selectRect = FindObjectOfType<SelectRect>();
        gridMgr = FindObjectOfType<GridMgr>();
        colorMgr = FindObjectOfType<ColorMgr>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LockSelectRect()
    {
        selectRect.enabled = false;
    }

    public void UnlockSelectRect()
    {
        selectRect.enabled = true;
    }

    public void createArmor(string type = "Body")
    {
        /// <summary>
        /// "Body","Strengthened","PowerCharged"
        /// </summary>
        Color color;
        switch(type)
        {
            case "Body":
                color = colorMgr.ColorArmorBody;
                break;
            default:
                print("Error: bad type");
                return;
        }
        gridMgr.FillSelectedGrids(type, color);
    }
}
