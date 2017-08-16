using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用来绘制选框的脚本
/// </summary>

public class SelectRect : MonoBehaviour
{

    Color selectRectColor;

    private UnityEngine.Mesh mesh;
    private GameObject meshObj;
    private float startX, startY, endX, endY;
    private bool triggered = false;

    private Camera mainCamera;
    private GridMgr gridMgr;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        gridMgr = FindObjectOfType<GridMgr>();
        ColorMgr colorMgr = FindObjectOfType<ColorMgr>();
        selectRectColor = colorMgr.ColorSelectRect;
        resetXY();

        meshObj = new GameObject();
        meshObj.name = "SelectRectangle";
        meshObj.transform.position =new Vector3(0,0,-5);

        meshObj.AddComponent<MeshFilter>();
        meshObj.AddComponent<MeshRenderer>();

        mesh = new UnityEngine.Mesh();
        meshObj.GetComponent<MeshFilter>().mesh = mesh;

        meshObj.GetComponent<MeshRenderer>().material.color = selectRectColor;
        meshObj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");
    }


    void Update()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetMouseButton(0))
        {
            if (!triggered)
            {
                triggered = true;
                startX = endX = pos.x;
                startY = endY = pos.y;
                gridMgr.UpdateSelectInfo(endX, endY, init:true);
            }
            else
            {
                endX = pos.x;
                endY = pos.y;
                gridMgr.UpdateSelectInfo(endX, endY);
            }
        }
        else
        {
            if (triggered)
            {
                triggered = false;
                endX = pos.x;
                endY = pos.y;
                gridMgr.UpdateSelectInfo(endX, endY, init: false, finished: true);
                resetXY();
            }
        }
        //print(new Vector2(endX, endY));
        refreshMesh();
    }

    private void resetXY()
    {
        startX = 0;
        startY = 0;
        endX = 0;
        endY = 0;
    }

    private void refreshMesh()
    {
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(startX,startY,0),
            new Vector3(startX,endY,0),
            new Vector3(endX,startY,0),
            new Vector3(endX,endY,0),
        };
        mesh.vertices = vertices;
        int[] triangles;
        if ((startX - endX) * (startY - endY) < 0)
        {
            triangles = new int[2 * 3]
            {
                0,3,1,
                0,2,3
            };
        }
        else
        {
            triangles = new int[2 * 3]
            {
                1,2,0,
                1,3,2
            };
        }
        mesh.triangles = triangles;
    }
}
