using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightMenu : MonoBehaviour, IPointerClickHandler
{
    public GameObject mRightMenu;
    public Button mCopy;
    public Button mPaste;
    public Button mSelect;
    public Button mCancelSelect;
    public Button mDelete;
    public Button mExchange;
    public Message Message;
    public Map Map;
    public Sprite But1Sprite;
    public Sprite But5Sprite;

    //目标按钮
    private GameObject Target;
    //被选择的按钮
    private GameObject SelectObj;
    private JObject CopyData = null;


    public void Copy()
    {
        if (!Target)
            return;
        CopyData = Target.GetComponent<RoomData>().GetJsonData();
        Close();
    }

    public void Parse()
    {
        if (!Target || CopyData == null)
            return;
        RoomData roomData = Target.GetComponent<RoomData>();
        roomData.SetName(CopyData["name"].ToString());
        roomData.SetDesc(CopyData["desc"].ToString());
        roomData.SetNpc((JObject)CopyData["npc"]);
        string str = roomData.RoomName;
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
        roomData.SetId(newId);
        Close();
    }

    public void Remove()
    {
        if (!Target)
            return;
        Target.GetComponent<RoomData>().ResetRoom();
        Map.RemoveRoomLine(Target.name);
        Close();
    }

    public void Select()
    {
        if (SelectObj)
            SelectObj.GetComponent<Image>().sprite = But1Sprite;
        if (!Target)
        {
            SelectObj = null;
            return;
        }
        SelectObj = Target;
        SelectObj.GetComponent<Image>().sprite = But5Sprite;
        Close();
    }

    public void CancelSelect()
    {
        if (SelectObj)
            SelectObj.GetComponent<Image>().sprite = But1Sprite;
        SelectObj = null;
        Close();
    }

    public void Exchange()
    {
        if (!Target || !SelectObj)
            return;
        RoomData a = Target.GetComponent<RoomData>();
        RoomData b = SelectObj.GetComponent<RoomData>();
        JObject data = a.GetJsonData();
        a.SetDesc(b.RoomDesc);
        a.SetId(b.RoomId);
        a.SetName(b.RoomName);
        a.SetNpc(b.RoomNpc);
        b.SetId(data["id"].ToString());
        b.SetName(data["name"].ToString());
        b.SetNpc((JObject)data["npc"]);
        b.SetDesc(data["desc"].ToString());

        SelectObj.GetComponent<Image>().sprite = But1Sprite;
        SelectObj = null;
        Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            Close();
    }

    private void Show()
    {
        RoomData roomData = Target.GetComponent<RoomData>();
        if (roomData.RoomId.Length > 1)
        {
            mSelect.gameObject.SetActive(true);
            mCopy.gameObject.SetActive(true);
        }

        else
        {
            mCopy.gameObject.SetActive(false);
            mSelect.gameObject.SetActive(false);
        }



        if (CopyData != null)
            mPaste.gameObject.SetActive(true);
        else
            mPaste.gameObject.SetActive(false);
        if (roomData.RoomId.Length < 1)
            mDelete.gameObject.SetActive(false);
        else
            mDelete.gameObject.SetActive(true);

        if (SelectObj && roomData.RoomId.Length > 1 && SelectObj != Target)
            mExchange.gameObject.SetActive(true);
        else
            mExchange.gameObject.SetActive(false);

        if (SelectObj && roomData.RoomId.Length > 1 && SelectObj == Target)
            mCancelSelect.gameObject.SetActive(true);
        else
            mCancelSelect.gameObject.SetActive(false);
    }

    public void Open(GameObject obj)
    {
        Target = obj;
        Show();
        Vector2 vector2 = Map.Coordxy(transform.GetComponent<RectTransform>(), obj);
        float width = obj.GetComponent<RectTransform>().rect.width;
        float height = obj.GetComponent<RectTransform>().rect.height;
        mRightMenu.transform.localPosition = new Vector2(vector2.x + width, vector2.y - (height / 2));
        gameObject?.SetActive(true);
    }

    public void Close()
    {
        gameObject?.SetActive(false);
    }
}
