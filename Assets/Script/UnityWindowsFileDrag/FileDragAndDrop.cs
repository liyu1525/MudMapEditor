using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using Assets.Script.Util;
using Newtonsoft.Json.Linq;

public class FileDragAndDrop : MonoBehaviour
{
    public Message Message;
    public Map Map;
    void OnEnable()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        if (aFiles.Count > 0)
        {
            string path = aFiles[0];
            try
            {
                string str = UtilFile.ReadFilePath(path);
                JObject data = JObject.Parse(str);
                Map.LoadMap(data);
            }
            catch
            { }
        }
    }

}
