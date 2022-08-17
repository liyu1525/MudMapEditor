using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEdit : MonoBehaviour
{
    public TMP_InputField mName;
    public TMP_InputField mId;
    public TMP_InputField mPath;
    public Button mSave;
    public Button mCancel;
    public TMP_Text mMapButText;

    public Message Message;
    public Map Map;

    private string Old_id = string.Empty;

    void Start()
    {
        mCancel.onClick.AddListener(() =>
        {
            Close();
        });

        mSave.onClick.AddListener(() =>
        {
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
            Map.MapId = mId.text;
            Map.MapName = mName.text;
            Map.MapDir = mPath.text;
            mMapButText.text = string.Format("{0}(ÅäÖÃ)", mName.text);
            Close();
        });
    }


    public void Open()
    {
        mId.text = Map.MapId;
        Old_id = mId.text;
        mName.text = Map.MapName;
        mPath.text = Map.MapDir;
        gameObject?.SetActive(true);
    }

    public void Close()
    {
        Old_id = string.Empty;
        gameObject?.SetActive(false);
    }
}
