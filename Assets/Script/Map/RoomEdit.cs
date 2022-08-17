using HotFix_Project;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEdit : MonoBehaviour
{
    public TMP_Text mCoord;
    public TMP_InputField mName;
    public TMP_InputField mId;
    public TMP_InputField mDesc;
    public Button mSave;
    public Button mRemove;

    public GameObject mNpcItem;
    public Button mNpcAdd;
    public Transform mNpcList;

    public Message Message;
    public Map Map;
    public NpcEdit NpcEdit;

    public GameObject SelectRoom { get; private set; }

    private ObjectPool NpcItemPool;

    public void Start()
    {
        mRemove.gameObject.SetActive(false);
        mSave.onClick.AddListener(() =>
        {
            Save();
        });
        mRemove.onClick.AddListener(() =>
        {
            Remove();
        });
        mNpcAdd.onClick.AddListener(() =>
        {
            if (!SelectRoom)
                return;
            RoomData roomData = SelectRoom.transform.GetComponent<RoomData>();
            if (roomData.RoomId.Length < 1)
            {
                Message.Error("房间尚未创建!");
                return;
            }
            NpcEdit.Open(roomData);
        });
        NpcItemPool = new ObjectPool("NpcItemPool", mNpcItem, 10);
        mName.onEndEdit.AddListener((string str) =>
        {
            if (!GlobalData.IsAutoSetId)
                return;
            if (!SelectRoom)
                return;
            RoomData roomData = SelectRoom.GetComponent<RoomData>();
            if (roomData.RoomId.Length > 1)
                Map.RemoveRoomId(roomData.RoomId);
            if (str.Length >= 4)
                str = str.Substring(str.Length - 2);
            string id = UtilChinese.GetPinYins(str);
            int count = 0;
            string newId;
            while (true)
            {
                newId = id;
                if (count != 0)
                    newId += count;
                if (!Map.AddRoomId(newId, roomData.GetCoord()))
                {
                    count++;
                    continue;
                }
                break;
            }
            mId.text = newId;
        });
    }

    private void Save()
    {
        if (!SelectRoom)
            return;
        RoomData roomData = SelectRoom.transform.GetComponent<RoomData>();
        if (mId.text.Length < 1)
            Message.Error("保存失败，请输入房间id");
        else if (!roomData.SetId(mId.text))
            Message.Error("保存失败，已有重复id!");
        else
        {
            roomData.SetDesc(mDesc.text);
            roomData.SetName(mName.text);
            mRemove.gameObject.SetActive(true);
        }
    }

    private void Remove()
    {
        if (!SelectRoom)
            return;
        RoomData roomData = SelectRoom.transform.GetComponent<RoomData>();
        roomData.ResetRoom();
        mName.text = string.Empty;
        mId.text = string.Empty;
        mDesc.text = string.Empty;
        mRemove.gameObject.SetActive(false);
        Map.RemoveRoomLine(SelectRoom.name);
    }

    /// <summary>
    /// 设置选择房间
    /// </summary>
    /// <param name="obj"></param>
    public void SetRoom(GameObject obj)
    {
        if (!obj)
            return;
        SelectRoom = obj;
        RoomData roomData = obj.GetComponent<RoomData>();
        mCoord.text = roomData.GetCoord();
        mName.text = roomData.RoomName;
        mId.text = roomData.RoomId;
        mDesc.text = roomData.RoomDesc;
        SetNpc(roomData.RoomNpc);
        if (mId.text.Length > 0)
            mRemove.gameObject.SetActive(true);
        else
            mRemove.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置npc展示
    /// </summary>
    public void SetNpc(JObject list)
    {
        NpcItemPool.Put_All(mNpcList);
        if (list == null)
            return;
        GameObject obj;
        IEnumerable<JProperty> JPr_s = list.Properties();
        foreach (JProperty i in JPr_s)
        {
            obj = NpcItemPool.Get();
            obj.name = i.Name;
            string id = i.Name;
            string name = i.Value.ToString();
            obj.transform.Find("name").GetComponent<TMP_Text>().text = i.Value.ToString();
            obj.transform.Find("edit").GetComponent<Button>().onClick.RemoveAllListeners();
            obj.transform.Find("edit").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!SelectRoom)
                    return;
                RoomData roomData = SelectRoom.transform.GetComponent<RoomData>();
                NpcEdit.Open(roomData, id, name);
            });
            obj.transform.Find("remove").GetComponent<Button>().onClick.RemoveAllListeners();
            obj.transform.Find("remove").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!SelectRoom)
                    return;
                RoomData roomData = SelectRoom.transform.GetComponent<RoomData>();
                roomData.RemoveNpc(id);
            });
            obj.transform.SetParent(mNpcList, false);
        }
    }
}
