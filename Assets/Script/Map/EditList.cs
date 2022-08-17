using HotFix_Project;
using JHYJ_HotFix.Script.Util;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditList : MonoBehaviour
{
    public Transform mList;
    public GameObject mItem;
    public Button mAdd;

    private ObjectPool EditItemPool;

    private int ItemIndex = 0;

    private void Start()
    {
        EditItemPool = new ObjectPool("EditItemPool", mItem, 10);
    }

    private void AddItem(string id)
    {
        GameObject obj = UtilObject.Find_Obj(mList, id);
        if (!obj)
        {
            obj = EditItemPool.Get();
            obj.name = id;
            obj.transform.SetParent(mList, false);
        }
        obj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
    }
}
