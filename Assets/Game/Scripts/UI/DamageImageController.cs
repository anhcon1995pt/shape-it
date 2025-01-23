using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageImageController : MonoBehaviour
{
    [SerializeField] float _timeShowDamageImage;
    [SerializeField] Image _damageImage;
    Coroutine _showDamageEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartShowDamage()
    {
        gameObject.SetActive(true);
        if(_showDamageEffect != null )
        {
            StopCoroutine(_showDamageEffect);
            _showDamageEffect = null;
        }
        _showDamageEffect = StartCoroutine(ShowDamageImage());
    }

    IEnumerator ShowDamageImage()
    {
        Color color = _damageImage.color;
        color.a = 0;
        _damageImage.color = color;
        _damageImage.DOKill();
        Tween aniFade = _damageImage.DOFade(1, _timeShowDamageImage / 2f).SetLoops(2, LoopType.Yoyo);
        yield return aniFade.WaitForCompletion();
        gameObject.SetActive(false);
    }
}
