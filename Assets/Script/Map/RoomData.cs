using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public TMP_Text mName;
    public TMP_Text mCoord;
    public Map Map;
    public RoomEdit RoomEdit;

    public string RoomName { get; private set; } = string.Empty;
    public string RoomId { get; private set; } = string.Empty;
    public string RoomDesc { get; private set; } = string.Empty;
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;
    public JObject RoomNpc { get; private set; } = null;

    /// <summary>
    /// 重置房间
    /// </summary>
    public void ResetRoom()
    {
        Map.RemoveRoomId(RoomId);
        RoomName = string.Empty;
        RoomId = string.Empty;
        RoomDesc = string.Empty;
        RoomNpc = null;
        mName.text = string.Empty;
        RoomEdit.SetNpc(RoomNpc);
    }

    /// <summary>
    /// 设置房间名称
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        RoomName = name;
        mName.text = name;
    }

    /// <summary>
    /// 设置房间id
    /// </summary>
    /// <param name="id"></param>
    public bool SetId(string id)
    {
        string old_id = RoomId;
        Map.RemoveRoomId(RoomId);
        if (Map.AddRoomId(id, GetCoord()))
        {
            RoomId = id;
            return true;
        }
        else
        {
            Map.AddRoomId(old_id, GetCoord());
            return false;
        }
    }

    /// <summary>
    /// 设置房间描述
    /// </summary>
    /// <param name="desc"></param>
    public void SetDesc(string desc)
    {
        RoomDesc = desc;
    }

    /// <summary>
    /// 设置房间坐标
    /// </summary>
    /// <param name="coord"></param>
    public void SetCoord(int x, int y)
    {
        this.X = x;
        this.Y = y;
        mCoord.text = string.Format("{0},{1}", x, y);
        gameObject.name = mCoord.text;
    }

    /// <summary>
    /// 获取房间坐标
    /// </summary>
    public string GetCoord()
    {
        return gameObject.name;
    }

    /// <summary>
    /// 添加一个npc
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void AddNpc(string id, string name)
    {
        if (RoomNpc == null)
            RoomNpc = new JObject();
        RoomNpc[id] = name;
        RoomEdit.SetNpc(RoomNpc);
    }

    /// <summary>
    /// 直接通过json数据设置npc
    /// </summary>
    /// <param name="data"></param>
    public void SetNpc(JObject data)
    {
        if (data == null)
            return;
        RoomNpc = data;
        RoomEdit.SetNpc(RoomNpc);
    }

    /// <summary>
    /// 删除一个npc
    /// </summary>
    /// <param name="id"></param>
    public void RemoveNpc(string id)
    {
        if (RoomNpc == null)
            return;
        try
        {
            RoomNpc.Remove(id);
        }
        catch
        { }
        RoomEdit.SetNpc(RoomNpc);
    }

    /// <summary>
    /// 获取该房间的json数据
    /// </summary>
    public JObject GetJsonData()
    {
        if (RoomId.Length < 1)
            return null;
        JObject data = new JObject();
        data["id"] = RoomId;
        data["coord"] = mCoord.text;
        data["name"] = RoomName;
        data["desc"] = RoomDesc;
        if (RoomNpc != null)
            data["npc"] = RoomNpc;
        return data;
    }
}
