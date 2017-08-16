using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏编辑器的控制脚本
/// </summary>

public class GridMgr : MonoBehaviour
{
    public int Length = 20;
    public int Width = 20;
    public float GridSize = 0.9f;

    Color BlankColor;
    Color AvailableColor;
    Color UnavalColor;

    OutputMgr outputMgr = new OutputMgr();

    GameObject fatherOfGrids;
    GameObject fatherOfMarks;
    
    readonly Color TransparentColor = new Color(0, 0, 0, 0);
    struct GridPos
    {
        public int x, y;
        public GridPos(int x,int y) { this.x = x; this.y = y; }
    }
    struct GridObj
    {
        public GameObject obj;
        public string type;
        public GridObj(GameObject obj) { this.obj = obj; type = "Blank"; }
    }
    //List<Grid> unavailableBlocks = new List<Grid>();
    List<GameObject> markMeshes = new List<GameObject>();
    Dictionary<GridPos, GridObj> gridObjects = new Dictionary<GridPos, GridObj>();

    GridPos selected1, selected2;

    EditorMgr editorMgr;


    void Awake()
    {
        editorMgr = FindObjectOfType<EditorMgr>();
        ColorMgr colorMgr = FindObjectOfType<ColorMgr>();
        BlankColor = colorMgr.ColorGridBlank;
        AvailableColor = colorMgr.ColorGridAvailable;
        UnavalColor = colorMgr.ColorGridUnaval;
        fatherOfGrids = new GameObject("Grids");
        fatherOfMarks = new GameObject("Marks");

        initGridMeshes();
    }

    GameObject newMesh(string name="New Mesh")
    {
        GameObject obj = new GameObject(name);

        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>();
        
        obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");

        return obj;
    }
    

    void initGridMeshes()
    {
        int cnt = 1;
        float size = GridSize / 2;
        for (int i = -Width / 2; i <= Width / 2; i++)
        {
            for (int j = -Length / 2; j <= Length / 2; j++)
            {
                GameObject obj = newMesh("Grid" + cnt.ToString()); cnt++;
                obj.GetComponent<MeshRenderer>().material.color = BlankColor;
                obj.transform.position = new Vector3(i, j, 0);
                obj.transform.SetParent(fatherOfGrids.transform);
                
                UnityEngine.Mesh mesh = new UnityEngine.Mesh();
                obj.GetComponent<MeshFilter>().mesh = mesh;
                mesh.vertices = new Vector3[4]
                {
                    new Vector3(size,size,0),
                    new Vector3(-size,size,0),
                    new Vector3(size,-size,0),
                    new Vector3(-size,-size,0)
                };
                mesh.triangles = new int[2 * 3]
                {
                    0,3,1,
                    0,2,3
                };

                GridObj gObj = new GridObj(obj);
                GridPos gPos = new GridPos(i, j);
                gridObjects.Add(gPos, gObj);
            }
        }

    }


    public void UpdateSelectInfo(float x,float y,bool init=false,bool finished=false)
    {
        /// 得到选框的终点位置
        /// 若刚刚建立选框，应当将init设为true
        if (init)
        {
            selected1 = selected2 = floatToGridPos(x, y);
        }
        else
        {
            selected2 = floatToGridPos(x, y);
        }
        if(finished)
        {
            editorMgr.LockSelectRect();
        }
        remarkGrids();
    }


    GridPos floatToGridPos(float x,float y)
    {
        int xi = (int)Mathf.Round(x);
        int yi =(int) Mathf.Round(y);
        return new GridPos(xi, yi);
    }

    IEnumerable<GridPos> getSelectedGridPoses()
    {
        int xMin = Mathf.Min(selected1.x, selected2.x);
        int xMax = Mathf.Max(selected1.x, selected2.x);
        int yMin = Mathf.Min(selected1.y, selected2.y);
        int yMax = Mathf.Max(selected1.y, selected2.y);
        
        for (int i = xMin; i <= xMax; i ++)
        {
            for (int j = yMin; j <= yMax; j ++)
            {
                GridPos gPos = new GridPos(i,j);
                yield return gPos;
            }
        }
    }

    void remarkGrids()
    {
        clearMarkMeshes();
        int p = 0;
        foreach(GridPos gPos in getSelectedGridPoses())
        { 
            if (p == markMeshes.Count) { createMarkMesh(); }
            GameObject curObj = markMeshes[p];
            Color color = gridObjects[gPos].type=="Blank" ? AvailableColor : UnavalColor;
            curObj.GetComponent<MeshRenderer>().material.color = color;
            curObj.transform.position = new Vector3(gPos.x, gPos.y, -4);
            p++;
        }
    }

    void createMarkMesh()
    {
        GameObject obj = newMesh();
        markMeshes.Add(obj);
        obj.transform.SetParent(fatherOfMarks.transform);
        UnityEngine.Mesh mesh = new UnityEngine.Mesh();
        mesh.vertices = new Vector3[4]
        {
            new Vector3(0.5f,0.5f,0),
            new Vector3(-0.5f,0.5f,0),
            new Vector3(0.5f,-0.5f,0),
            new Vector3(-0.5f,-0.5f,0),
        };
        mesh.triangles = new int[2 * 3] { 0, 3, 1, 0, 2, 3 };
        obj.GetComponent<MeshFilter>().mesh = mesh;
    }

    void clearMarkMeshes()
    {
        foreach (GameObject obj in markMeshes)
        {
            obj.GetComponent<MeshRenderer>().material.color = TransparentColor;
        }
    }

    public void FillSelectedGrids(string type,Color color)
    {
        foreach (GridPos gPos in getSelectedGridPoses())
        {
            GridObj gObj = gridObjects[gPos];
            gObj.type = type;
            gObj.obj.GetComponent<MeshRenderer>().material.color = color;
            gridObjects[gPos] = gObj;
        }
        editorMgr.UnlockSelectRect();
        clearMarkMeshes();
    }

    public void ResetSelectedGrids()
    {
        foreach (GridPos gPos in getSelectedGridPoses())
        {
            GridObj gObj = gridObjects[gPos];
            gObj.type = "Blank";
            gObj.obj.GetComponent<MeshRenderer>().material.color = BlankColor;
            gridObjects[gPos] = gObj;
        }
        editorMgr.UnlockSelectRect();
        clearMarkMeshes();
    }

    public void OutputCurrent()
    {
        List<GameObject> objList = new List<GameObject>();
        foreach(GridObj gObj in gridObjects.Values)
        {
            if(gObj.type!="Blank")
            {
                objList.Add(gObj.obj);
            }
            else
            {
                DestroyImmediate(gObj.obj);
            }
        }
        outputMgr.Output(objList);
    }
}
