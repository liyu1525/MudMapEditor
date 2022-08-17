using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcEdit : MonoBehaviour
{
    public TMP_InputField mName;
    public TMP_InputField mId;
    public Button mSave;
    public Button mCancel;

    public Message Message;

    private RoomData _RoomData;
    private string Old_id = string.Empty;

    void Start()
    {
        mCancel.onClick.AddListener(() =>
        {
            Close();
        });
        mSave.onClick.AddListener(() =>
        {
            if (_RoomData == null)
                return;
            if (mId.text.Length < 1)
            {
                Message.Error("ÇëÊäÈëid");
                return;
            }
            if (mName.text.Length < 1)
            {
                Message.Error("ÇëÊäÈëÃû³Æ");
                return;
            }
            _RoomData.RemoveNpc(Old_id);
            _RoomData.AddNpc(mId.text, mName.text);
            Close();
        });
    }


    public void Open(RoomData data, string id = "", string name = "")
    {
        _RoomData = data;
        if (id != null)
            Old_id = id;
        mId.text = id;
        mName.text = name;
        gameObject?.SetActive(true);
    }

    public void Close()
    {
        Old_id = string.Empty;
        gameObject?.SetActive(false);
    }
}
