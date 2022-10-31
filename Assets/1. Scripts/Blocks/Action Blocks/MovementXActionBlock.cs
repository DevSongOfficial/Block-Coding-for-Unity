using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class MovementXActionBlock : ActionBlock
{
    private TMP_InputField inputField;
    private float moveAmount;


    protected override void Awake()
    {
        base.Awake();

        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(delegate (string value) { float.TryParse(value, out moveAmount); });
    }

    public override IEnumerator DoFunction()
    {
        Target.ChangePositionX(moveAmount);
        
        yield return null;
    }
}
