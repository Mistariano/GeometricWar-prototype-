using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputMgr : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Output(List<GameObject> objList,string name="NewCreation")
    {
        GameObject outputResult = new GameObject(name);
        foreach(GameObject obj in objList)
        {
            obj.transform.SetParent(outputResult.transform);
        }
    }
}
