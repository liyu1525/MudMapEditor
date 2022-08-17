using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    public GameObject mError;
    public GameObject mSuccess;
    public GameObject mWarning;

    public void Error(string str)
    {
        GameObject obj = Instantiate(mError);
        obj.transform.Find("text").GetComponent<TMP_Text>().text = str;
        obj.SetActive(true);
        obj.transform.SetParent(transform, false);
    }

    public void Success(string str)
    {
        GameObject obj = Instantiate(mSuccess);
        obj.transform.Find("text").GetComponent<TMP_Text>().text = str;
        obj.SetActive(true);
        obj.transform.SetParent(transform, false);
    }

    public void Warning(string str)
    {
        GameObject obj = Instantiate(mWarning);
        obj.transform.Find("text").GetComponent<TMP_Text>().text = str;
        obj.SetActive(true);
        obj.transform.SetParent(transform, false);
    }
}
