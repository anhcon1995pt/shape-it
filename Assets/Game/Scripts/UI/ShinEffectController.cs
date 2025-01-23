using AC.GameTool.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShinEffectController : MonoBehaviour
{
    [SerializeField] float shineSpeed;
    [SerializeField] bool loop;
    [ConditionField("loop", true)]
    [SerializeField] int shineRepeats;
    [SerializeField] float shineWaitTime;
    private Material spriteMaterial = null;
    private float shinePositon = 0;
    private Coroutine shineRoutine = null;
    int shineLocationParameterID;
    Image image;

    void Start()
    {
        
    }
    private void OnEnable()
    {
        shineLocationParameterID = Shader.PropertyToID("_ShineLocation");

        image = GetComponent<Image>();

        spriteMaterial = image.material;
        spriteMaterial.SetFloat(shineLocationParameterID, 0);
        StartShine(0);
    }
    private void OnDisable()
    {
        StopShine();
    }
    private void OnDestroy()
    {
        StopShine();
    }

    public void StartShine(float delay)
    {
        if (shineRoutine != null)
        {
            StopCoroutine(shineRoutine);
            shineRoutine = null;
        }          
        shineRoutine = StartCoroutine(StartShineCoroutine(delay));
    }

    public void StopShine()
    {
        if (shineRoutine != null)
            StopCoroutine(shineRoutine);
        shineRoutine = null;
    }

    private float ShineCurve(float lerpProgress)
    {
        float newValue = lerpProgress * lerpProgress * lerpProgress * (lerpProgress * (6f * lerpProgress - 15f) + 10f);
        return newValue;
    }

    private IEnumerator StartShineCoroutine(float dealay)
    {
        yield return new WaitForSeconds(dealay);

        if (shineSpeed <= 0f)
            yield break;

        int count = loop ? 1 : shineRepeats;
        while (count > 0)
        {
            yield return new WaitForSeconds(shineWaitTime);

            count = loop ? 1 : count - 1;

            float startTime = Time.time;

            while (Time.time < startTime + 1f / shineSpeed)
            {
                shinePositon = ShineCurve((Time.time - startTime) * shineSpeed);
                spriteMaterial.SetFloat(shineLocationParameterID, shinePositon);

                yield return new WaitForEndOfFrame();
            }
        }

        yield break;
    }
}

