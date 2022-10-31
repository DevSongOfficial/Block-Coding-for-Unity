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
        inputField.onValueChanged.AddListener(delegate (string value) { float.TryParse(value, out time); });
    }

    public override IEnumerator DoFunction()
    {
        yield return new WaitForSeconds(time);
    }
}