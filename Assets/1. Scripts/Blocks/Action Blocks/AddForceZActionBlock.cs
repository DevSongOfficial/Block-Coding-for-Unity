using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddForceZActionBlock : ActionBlock
{
    private TMP_InputField inputField;
    private float forceAmount;

    protected override void Awake()
    {
        base.Awake();

        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener((string value) => float.TryParse(value, out forceAmount));
    }

    public override IEnumerator DoFunction()
    {
        Target.rigidBody.AddForce(Vector3.forward * forceAmount, ForceMode.Impulse);

        yield return null;
    }
}