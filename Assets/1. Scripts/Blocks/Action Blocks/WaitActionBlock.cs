using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class WaitActionBlock : ActionBlock
{
    private TMP_InputField inputField;
    private float time;

    protected override void Awake()
    {
        base.Awake();

        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener((string value) => { float.TryParse(value, out time); });
    }

    float timer;
    public override IEnumerator DoFunction()
    {
        timer = 0;

        while (GameManager.InProgress)
        {
            if (timer >= time)
            {
                yield break;
            }
            else
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}