using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageText : MonoBehaviour
{
    public Button mClose;
    public CanvasGroup mGroup;
    private Coroutine _Coroutine;
    private static readonly WaitForSeconds _Time = new WaitForSeconds(2f);
    void Start()
    {
        _Coroutine = StartCoroutine(DelayDestroy());
        mClose.onClick.AddListener(() =>
        {
            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            Destroy();
        });
    }

    private IEnumerator DelayDestroy()
    {
        yield return _Time;
        Destroy();
    }

    private void Destroy()
    {
        DOTween.To(() => mGroup.alpha, value => mGroup.alpha = value, 0.3f, 1f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
