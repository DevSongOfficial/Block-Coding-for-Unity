using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class DefaultLoopBlock : LoopBlock
{
    private TMP_InputField inputField;

    protected override void Awake()
    {
        base.Awake();

        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(ChangeLoopTimes);
    }

    public override IEnumerator StartLoopFunctions()
    {
        yield return StartCoroutine(DefaultLoopFunctionsRoutine());
    }
}
