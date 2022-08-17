using Assets.Script.Util;
using DG.Tweening;
using HotFix_Project;
using JHYJ_HotFix.Script.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject mButton;
    public GameObject mLine;
    public Transform mContent;
    public Transform PoolTran;
    public ScrollRect mMapScroll;
    public Message Message;
    private int XCount = 30;
    private int YCount = 30;
    // 按钮直接间距
    private int Space = 50;
    // 按钮大小
    private Vector2 ButSize = new Vector2(150, 80);
    // 连接线宽度
    private int LineWigth = 3;

    private ObjectPool RoomButtonPool;
    private ObjectPool MapLinePool;

    public Sprite But1Sprite;
    public Sprite But2Sprite;
    public Sprite But3Sprite;
    public Sprite But4Sprite;

    private GameObject SelectButton = null;
    // 地图连接线（连接，取消连接时展示）
    private GameObject MoveLine = null;
    // 移动到的按钮，避免重复进入线条展示
    private GameObject MoveBut = null;

    private JArray RoomIds = new JArray();

    /// <summary>
    /// 储存所有地图连接线
    /// </summary>
    private JArray MapLine = new JArray();

    //当前打开地图名称
    public string MapName;
    //当前打开地图id
    public string MapId;
    //当前打开地图保存文件夹
    public string MapDir;

    void Start()
    {
        ObjectPool.PoolTran = PoolTran;
        RoomButtonPool = new ObjectPool("RoomButtonPool", mButton, XCount * YCount);
        MapLinePool = new ObjectPool("MapLinePool", mLine, 300);
        MapLinePool.Set_Recycle(delegate (GameObject obj)
        {
            obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        });
        RoomButtonPool.Set_Recycle((GameObject obj) =>
        {
            obj.GetComponent<RoomData>().ResetRoom();
        }); ;
        CreateMap();
    }

    /// <summary>
    /// 添加一个房间id
    /// </summary>
    public bool AddRoomId(string id, string coord)
    {
        JObject data;
        for (int i = 0; i < RoomIds.Count; i++)
        {
            data = (JObject)RoomIds[i];
            if (data["id"].ToString().Equals(id) && !data["coord"].ToString().Equals(coord))
                return false;
            if (data["id"].ToString().Equals(id) && data["coord"].ToString().Equals(coord))
                return true;
        }
        data = new JObject();
        data["id"] = id;
        data["coord"] = coord;
        RoomIds.Add(data);
        return true;
    }

    /// <summary>
    /// 删除一个房间id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void RemoveRoomId(string id)
    {
        JObject data;
        for (int i = 0; i < RoomIds.Count; i++)
        {
            data = (JObject)RoomIds[i];
            if (data["id"].ToString().Equals(id))
            {
                RoomIds.RemoveAt(i);
                break;
            }
        }

    }

    /// <summary>
    /// 创建地图
    /// </summary>
    private void CreateMap()
    {
        RoomButtonPool.Put_All(mContent);
        mButton.GetComponent<RectTransform>().sizeDelta = ButSize;
        int map_width = (int)(XCount * (ButSize.x + Space) - Space);
        int map_height = (int)(YCount * (ButSize.y + Space) - Space);

        //设置地图容器的大小
        mContent.GetComponent<RectTransform>().sizeDelta = new Vector2(map_width, map_height);
        for (int x = 0; x < XCount; x++)
        {
            for (int y = 0; y < YCount; y++)
            {
                GameObject obj = RoomButtonPool.Get();
                obj.GetComponent<RoomData>().SetCoord(x, y);
                obj.GetComponent<ItemDrag>().SetTargetParent(mContent.gameObject);
                obj.GetComponent<ItemDrag>().SetDragExec(MoveRoom);
                obj.GetComponent<ItemDrag>().SetBeginDragExec(StartDrag);
                obj.GetComponent<ItemDrag>().SetEndDragExec(EndDrag);
                obj.transform.SetParent(mContent, false);
                obj.GetComponent<RectTransform>().localPosition = new Vector2(x * (ButSize.x + Space), -(y * (ButSize.y + Space)));
                obj.GetComponent<Button>().onClick.RemoveAllListeners();
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    gameObject.GetComponent<RoomEdit>().SetRoom(obj);
                });
            }
        }
        StartCoordinates("15,15");
    }

    /// <summary>
    /// 启动拖拽
    /// </summary>
    /// <param name="obj"></param>
    private void StartDrag(GameObject obj)
    {
        if (!obj)
            return;
        RoomData data = obj.GetComponent<RoomData>();
        if (data.RoomId.Length < 1)
            return;
        SelectButton = obj;
        obj.GetComponent<Image>().sprite = But2Sprite;
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="obj"></param>
    private void EndDrag(GameObject obj)
    {
        if (MoveLine)
        {
            MapLinePool.Put(MoveLine);
            MoveLine = null;
        }

        if (obj && SelectButton)
        {
            RoomData a = SelectButton.GetComponent<RoomData>();
            RoomData b = obj.GetComponent<RoomData>();

            if (IsLink(a.GetCoord(), b.GetCoord()))
            {
                RemoveMapLine(a.GetCoord(), b.GetCoord());
            }
            else if (a.RoomId.Length > 0 && b.RoomId.Length > 0)
            {
                AddMapLine(SelectButton, obj);
            }
        }

        if (SelectButton)
            SelectButton.GetComponent<Image>().sprite = But1Sprite;
        if (MoveBut)
            MoveBut.GetComponent<Image>().sprite = But1Sprite;
        SelectButton = null;
        MoveBut = null;
    }

    /// <summary>
    /// 鼠标移动到当前按钮上
    /// </summary>
    /// <param name="obj"></param>
    private void MoveRoom(GameObject obj)
    {
        if (!obj || !SelectButton || MoveBut == obj)
            return;
        if (MoveLine)
        {
            MapLinePool.Put(MoveLine);
            MoveLine = null;
        }
        if (SelectButton == obj)
            return;

        if (MoveBut)
            MoveBut.GetComponent<Image>().sprite = But1Sprite;

        RoomData a = SelectButton.GetComponent<RoomData>();
        RoomData b = obj.GetComponent<RoomData>();

        if (b.RoomId.Length < 1)
            return;


        string position = IsAdjoin(a, b);
        if (position == string.Empty)
        {
            MoveBut = null;
            return;
        }


        MoveBut = obj;

        if (IsLink(a.GetCoord(), b.GetCoord()))
        {
            MoveLine = GenerateLine(SelectButton, obj, UtilColor.Red);
            MoveBut.GetComponent<Image>().sprite = But4Sprite;
        }
        else
        {
            MoveLine = GenerateLine(SelectButton, obj, UtilColor.Green);
            MoveBut.GetComponent<Image>().sprite = But3Sprite;
        }
    }

    /// <summary>
    /// 判断两个坐标点是否已经连接
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool IsLink(string a, string b)
    {
        string line1 = string.Format("{0}|{1}", a, b);
        string line2 = string.Format("{0}|{1}", b, a);
        for (int i = 0; i < MapLine.Count; i++)
        {
            if (line1.Equals(MapLine[i].ToString()) || line2.Equals(MapLine[i].ToString()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断是否是相邻坐标
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private string IsAdjoin(RoomData a, RoomData b)
    {
        string result = string.Empty;
        if (Math.Abs(a.X - b.X) > 1 || Math.Abs(a.Y - b.Y) > 1)
            return result;
        if (a.X == b.X && a.Y != b.Y)
        {
            if (a.Y < b.Y)
                result = "南";
            else result = "北";
        }
        else if (a.Y == b.Y && a.X != b.X)
        {
            if (a.X < b.X)
                result = "东";
            else result = "西";
        }
        else if (a.Y != b.Y && a.X != b.X || a.X > b.X && a.Y > b.Y)
        {
            if (a.X < b.X && a.Y < b.Y)
                result = "东南";
            else if (a.X > b.X && a.Y > b.Y)
                result = "东北";
            if (a.X < b.X && a.Y > b.Y)
                result = "西南";
            else if (a.X > b.X && a.Y < b.Y)
                result = "西北";
        }
        return result;
    }

    private string IsAdjoin(string a, string b)
    {
        string result = string.Empty;

        int x, x1, y, y1;
        string[] k;
        k = a.Split(',');
        if (k.Length != 2)
            return string.Empty;
        x = int.Parse(k[0]);
        y = int.Parse(k[1]);
        k = b.Split(',');
        if (k.Length != 2)
            return string.Empty;
        x1 = int.Parse(k[0]);
        y1 = int.Parse(k[1]);


        if (Math.Abs(x - x1) > 1 || Math.Abs(y - y1) > 1)
            return result;
        if (x == x1 && y != y1)
        {
            if (y < y1)
                result = "南";
            else result = "北";
        }
        else if (y == y1 && x != x1)
        {
            if (x < x1)
                result = "东";
            else result = "西";
        }
        else if (y != y1 && x != x1 || x > x1 && y > y1)
        {
            if (x < x1 && y < y1)
                result = "东南";
            else if (x > x1 && y > y1)
                result = "东北";
            if (x < x1 && y > y1)
                result = "西南";
            else if (x > x1 && y < y1)
                result = "西北";
        }
        return result;
    }

    /// <summary>
    /// 生成地图连接线
    /// </summary>
    /// <param name="data"></param>
    private GameObject GenerateLine(GameObject aBut, GameObject bBut, Color color)
    {
        RoomData a = aBut.GetComponent<RoomData>();
        RoomData b = bBut.GetComponent<RoomData>();
        string position = IsAdjoin(a, b);
        GameObject obj = MapLinePool.Get();
        obj.name = string.Format("{0}|{1}", a.GetCoord(), b.GetCoord());
        obj.transform.SetParent(mContent, false);
        obj.GetComponent<Image>().color = color;
        if (color == UtilColor.Blue)
            obj.name = string.Format("{0}|{1}", aBut.name, bBut.name);

        float width, height;
        width = aBut.GetComponent<RectTransform>().rect.width;
        height = aBut.GetComponent<RectTransform>().rect.height;

        Vector2 e1 = Coordxy(mContent.GetComponent<RectTransform>(), aBut);
        Vector2 e2 = Coordxy(mContent.GetComponent<RectTransform>(), bBut);

        int offset = LineWigth / 2;
        switch (position)
        {
            case "南":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e1.x) + (width / 2) - (LineWigth - offset), Convert.ToInt32(e2.y) + Space, 0);
                break;
            case "北":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e1.x) + (width / 2) - (LineWigth - offset), Convert.ToInt32(e2.y) - height, 0);
                break;
            case "东":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e2.x) - Space, Convert.ToInt32(e1.y) - (height / 2) + LineWigth - offset, 0);
                break;
            case "西":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e1.x) - Space, Convert.ToInt32(e1.y) - (height / 2) + LineWigth - offset, 0);
                break;
            case "东南":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e2.x) - Space, Convert.ToInt32(e1.y) - height, 0);
                break;
            case "东北":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e1.x) - Space, Convert.ToInt32(e2.y) - height, 0);
                break;
            case "西南":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e2.x), Convert.ToInt32(e1.y) + Space, 0);
                break;
            case "西北":
                obj.transform.localPosition = new Vector3(Convert.ToInt32(e1.x), Convert.ToInt32(e2.y) + Space, 0);
                break;
        }

        if (position.Equals("南") || position.Equals("北"))
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(LineWigth, Space);
        else if (position.Equals("东") || position.Equals("西"))
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(Space, LineWigth);
        else if (position.Equals("东南") || position.Equals("东北"))
        {
            double d = (Space * Space) + (Space * Space);
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2((float)Math.Sqrt(d), LineWigth);
            obj.GetComponent<RectTransform>().Rotate(0, 0, -45);
        }
        else if (position.Equals("西南") || position.Equals("西北"))
        {
            double d = (Space * Space) + (Space * Space);
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2((float)Math.Sqrt(d), LineWigth);
            obj.GetComponent<RectTransform>().Rotate(0, 0, -135);
        }
        return obj;
    }

    /// <summary>
    /// 添加一条地图连接线
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void AddMapLine(GameObject a, GameObject b)
    {
        if (IsLink(a.name, b.name))
            return;
        GenerateLine(a, b, UtilColor.Blue);
        MapLine.Add(string.Format("{0}|{1}", a.name, b.name));
    }

    /// <summary>
    /// 删除一条地图连接线
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void RemoveMapLine(string a, string b)
    {
        string line1 = string.Format("{0}|{1}", a, b);
        string line2 = string.Format("{0}|{1}", b, a);
        for (int i = 0; i < MapLine.Count; i++)
        {
            if (line1.Equals(MapLine[i].ToString()) || line2.Equals(MapLine[i].ToString()))
            {
                MapLine.RemoveAt(i);
                break;
            }
        }
        GameObject obj = UtilObject.Find_Obj(mContent, line1);
        if (!obj)
            obj = UtilObject.Find_Obj(mContent, line2);
        if (!obj)
            return;
        MapLinePool.Put(obj);
    }

    /// <summary>
    /// 删除指定坐标的相关连接线
    /// </summary>
    /// <param name="coord"></param>
    public void RemoveRoomLine(string coord)
    {
        string[] k;
        for (int i = MapLine.Count - 1; i >= 0; i--)
        {
            k = MapLine[i].ToString().Split('|');
            if (k.Length == 2)
            {
                if (k[0].Equals(coord) || k[1].Equals(coord))
                    MapLine.RemoveAt(i);
            }
        }

        for (int i = mContent.childCount - 1; i >= 0; i--)
        {
            GameObject obj = mContent.GetChild(i).gameObject;
            k = obj.name.Split('|');
            if (k.Length == 2)
            {
                if (k[0].Equals(coord) || k[1].Equals(coord))
                    Destroy(obj);
            }
        }
    }

    /// <summary>
    /// 获取子Ui在父布局里的坐标
    /// </summary>
    /// <param name="rootRect"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector2 Coordxy(RectTransform rootRect, GameObject obj)
    {
        Vector2 screenVec2 = RectTransformUtility.WorldToScreenPoint(Camera.main, obj.transform.position);
        Vector2 inRootVec2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenVec2, Camera.main, out inRootVec2);
        return inRootVec2;
    }

    /// <summary>
    /// 保存地图数据
    /// </summary>
    public void SaveMapData()
    {
        if (MapId.Length < 1)
        {
            Message.Error("请先配置地图id");
            return;
        }
        else if (MapName.Length < 1)
        {
            Message.Error("请先配置地图名称");
            return;
        }
        else if (MapDir.Length < 1)
        {
            Message.Error("请先配置地图保存文件夹名称");
            return;
        }
        else if (GlobalData.SavePath.Length < 1)
        {
            Message.Error("请先再设置处配置保存路径");
            return;
        }

        JObject SaveMapData = new JObject();
        JArray MapData = new JArray();
        for (int i = 0; i < mContent.childCount; i++)
        {
            try
            {
                RoomData roomData = mContent.GetChild(i).GetComponent<RoomData>();
                if (roomData == null)
                    continue;
                JObject data = roomData.GetJsonData();
                if (data != null)
                    MapData.Add(data);
            }
            catch
            {
                continue;
            }
        }
        SaveMapData.Add("Id", MapId);
        SaveMapData.Add("Dir", MapDir);
        SaveMapData.Add("Name", MapName);
        SaveMapData.Add("Maps", MapData);
        SaveMapData.Add("Lines", MapLine);
        UtilFile.ResetFilePath(GlobalData.SavePath + "/" + MapDir, string.Format("{0}.json", MapId), SaveMapData.ToString());
        {
            Message.Success("保存成功!");
            return;
        }
    }

    /// <summary>
    /// 把界面滚到到人物所在的地点
    /// </summary>
    public void StartCoordinates(string coord)
    {
        float x, y;
        GameObject obj;
        obj = UtilObject.Find_Obj(mContent, coord);
        if (!obj)
            return;
        Vector2 e = Coordxy(mContent.GetComponent<RectTransform>(), obj);

        x = (gameObject.GetComponent<RectTransform>().rect.width - obj.GetComponent<RectTransform>().rect.width) / 2;
        y = (gameObject.GetComponent<RectTransform>().rect.height - obj.GetComponent<RectTransform>().rect.height) / 2;
        x = Convert.ToInt32(e.x) - x;
        int e_y = Convert.ToInt32(e.y);
        y = (e_y > 0 ? e_y : -e_y) - y;

        mContent.transform.DOLocalMove(new Vector3((float)-x, (float)y, 0), 1.5f, true);
    }

    public void OpenWindowsDialog()
    {
        string str = FolderBrowserHelper.SelectFile();
        str = UtilFile.ReadFilePath(str);
        if (str.Length < 1)
            return;
        try
        {
            JObject data = JObject.Parse(str);

            LoadMap(data);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Message.Error("读取地图数据文件失败!");
        }
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    /// <param name="data"></param>
    public void LoadMap(JObject data)
    {
        ResetMap();
        if (data == null)
        {
            StartCoordinates("15,15");
            return;
        }

        MapId = data["Id"].ToString();
        MapName = data["Name"].ToString();
        MapDir = data["Dir"].ToString();
        JArray MapData = (JArray)data["Maps"];
        MapLine = (JArray)data["Lines"];
        if (MapLine == null)
            MapLine = new JArray();
        JObject room_data;
        GameObject obj;
        string first_coord = "15,15";
        for (int i = 0; i < MapData.Count; i++)
        {
            room_data = (JObject)MapData[i];
            string coord = room_data["coord"].ToString();
            if (i == 0)
                first_coord = coord;
            obj = UtilObject.Find_Obj(mContent, coord);
            if (!obj)
                continue;
            RoomData roomData = obj.GetComponent<RoomData>();
            roomData.SetDesc(room_data["desc"].ToString());
            roomData.SetId(room_data["id"].ToString());
            roomData.SetName(room_data["name"].ToString());
            roomData.SetNpc((JObject)room_data["npc"]);
        }
        GameObject a, b;
        string[] k;
        for (int i = 0; i < MapLine.Count; i++)
        {
            k = MapLine[i].ToString().Split('|');
            if (k.Length != 2)
                continue;
            a = UtilObject.Find_Obj(mContent, k[0]);
            b = UtilObject.Find_Obj(mContent, k[1]);
            if (!a || !b)
                continue;
            GenerateLine(a, b, UtilColor.Blue);
        }
        StartCoordinates(first_coord);
    }

    /// <summary>
    /// 导出地图文件
    /// </summary>
    public void ExportFile()
    {
        if (MapId.Length < 1)
        {
            Message.Error("请输入地图id");
            return;
        }
        string path = FolderBrowserHelper.GetPathFromWindowsExplorer();
        if (!Directory.Exists(path))
        {
            Message.Error("该路径不存在!");
            return;
        }
        for (int i = 0; i < mContent.childCount; i++)
        {
            try
            {
                RoomData roomData = mContent.GetChild(i).GetComponent<RoomData>();
                if (roomData == null)
                    continue;
                JObject data = roomData.GetJsonData();
                if (data == null)
                    continue;
                string str = GetTemplate(data);
                UtilFile.ResetFilePath(path, string.Format("{0}.c", data["id"]), str);
            }
            catch
            {
                continue;
            }
        }
        Message.Success("地图导出完成!");
    }

    /// <summary>
    /// 文件内容模板
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string GetTemplate(JObject data)
    {
        StringBuilder str = new StringBuilder();
        str.Append("inherit ROOM;\n\n");
        str.Append("void create()\n");
        str.Append("{\n");
        str.Append(string.Format("    set(\"short\",\"{0}\");\n", data["name"]));
        str.Append(string.Format("    set(\"long\",\"{0}\");\n", data["desc"]));
        str.Append(string.Format("    set(\"city\",\"{0}\");\n", MapId));
        str.Append(string.Format("    set(\"coord\",\"{0}\");\n", data["coord"]));
        str.Append("    set(\"exits\", ([\n");
        JObject exits = GetExits(data["coord"].ToString());
        IEnumerable<JProperty> JPr_s = exits.Properties();
        foreach (JProperty i in JPr_s)
        {
            str.Append(string.Format("	   \"{0}\":__DIR__\"{1}\",\n", i.Name, i.Value));
        }
        str.Append("    ]));\n\n");

        str.Append("    set(\"objects\", ([\n");
        JObject npc = (JObject)data["npc"];
        if (npc != null)
        {
            JPr_s = npc.Properties();
            foreach (JProperty i in JPr_s)
            {
                str.Append(string.Format("	   __DIR__\"npc/{0}\" : 1,\n", i.Name));
            }
        }
        str.Append("    ]));\n\n");
        str.Append("    setup();\n");
        str.Append("}");
        return str.ToString();
    }

    private JObject GetExits(string coord)
    {
        JObject exits = new JObject();
        string[] k;
        string exit;
        RoomData roomData;
        for (int i = 0; i < MapLine.Count; i++)
        {
            k = MapLine[i].ToString().Split('|');
            if (k.Length != 2)
                continue;
            if (k[0].Equals(coord))
            {
                exit = IsAdjoin(k[0], k[1]);
                exit = UtilChinese.ConveExit(exit);
                roomData = mContent.Find(k[1]).GetComponent<RoomData>();
                exits.Add(exit, roomData.RoomId);
            }
            else if (k[1].Equals(coord))
            {
                exit = IsAdjoin(k[1], k[0]);
                exit = UtilChinese.ConveExit(exit);
                roomData = mContent.Find(k[0]).GetComponent<RoomData>();
                exits.Add(exit, roomData.RoomId);
            }
        }
        return exits;
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    public void ResetMap()
    {
        MapLine = new JArray();
        MapDir = string.Empty;
        MapName = string.Empty;
        MapId = string.Empty;
        RoomIds = new JArray();
        TheType theType;
        GameObject obj;
        for (int i = mContent.childCount - 1; i >= 0; i--)
        {
            obj = mContent.GetChild(i).gameObject;
            theType = obj.GetComponent<TheType>();
            if (theType.type.Equals("line"))
                MapLinePool.Put(obj);
            else
                obj.GetComponent<RoomData>().ResetRoom();
        }
        StartCoordinates("15,15");
    }
}
