using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetEdit : MonoBehaviour
{
    public TMP_Text mPath;
    public Toggle mAutoSetId;

    void Start()
    {
        mPath.GetComponent<Button>().onClick.AddListener(() =>
        {
            OpenWindowsDialog();
        });
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Close();
        });
        mAutoSetId.isOn = GameDataManager.GetBool("IsAutoSetId");
        mAutoSetId.onValueChanged.AddListener((bool isOn) =>
        {
            GlobalData.IsAutoSetId = isOn;
            GameDataManager.SetBool("IsAutoSetId", isOn);
        });
    }

    public void OpenWindowsDialog()
    {
        string path = FolderBrowserHelper.GetPathFromWindowsExplorer();
        if (!Directory.Exists(path))
        {
            return;
        }
        GlobalData.SavePath = path;
        mPath.text = path;
        GameDataManager.SetString("SavePath", path);
    }

    public void Open()
    {
        if (GlobalData.SavePath.Length < 1)
            mPath.text = "ÇëÑ¡Ôñ±£´æÂ·¾¶";
        else
            mPath.text = GlobalData.SavePath;
        gameObject?.SetActive(true);
    }

    public void Close()
    {
        gameObject?.SetActive(false);
    }
}
