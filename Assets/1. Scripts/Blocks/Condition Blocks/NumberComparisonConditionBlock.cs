using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class NumberComparisonConditionBlock : ConditionBlock
{
    // ¼ø¼­ DropDownÀÌ¶û ¸ÂÃç¾ß ÇÔ
    private enum ValueType
    {
        X = 0,
        Y,
        Z,
        Num
    }
    private ValueType valueTypeLeft;
    private ValueType valueTypeRight;

    public float valueLeft;
    public float valueRight;

    private TMP_Dropdown dropDownLeft;
    private TMP_Dropdown dropDownRight;
    private TMP_InputField inputFieldLeft;
    private TMP_InputField inputFieldRight;

    protected override void Awake()
    {
        base.Awake();

        dropDownLeft = transform.Find("Dropdown_Left").GetComponent<TMP_Dropdown>();
        dropDownRight = transform.Find("Dropdown_Right").GetComponent<TMP_Dropdown>();
        inputFieldLeft = transform.Find("InputField_Left").GetComponent<TMP_InputField>();
        inputFieldRight = transform.Find("InputField_Right").GetComponent<TMP_InputField>();

        dropDownLeft.onValueChanged.AddListener(SetValueLeft);
        dropDownRight.onValueChanged.AddListener(SetValueRight);
        inputFieldLeft.onValueChanged.AddListener(ChangeLeftValue);
        inputFieldRight.onValueChanged.AddListener(ChangeRightValue);
    }

    protected override void Update()
    {
        switch (valueTypeLeft)
        {
            case ValueType.X:
                ChangeLeftValue(Target.GetPositionX());
                break;
            case ValueType.Y:
                ChangeLeftValue(Target.GetPositionY());
                break;
            case ValueType.Z:
                ChangeLeftValue(Target.GetPositionZ());
                break;
            case ValueType.Num:
                inputFieldLeft.gameObject.SetActive(true);
                break;
        }

        switch (valueTypeRight)
        {
            case ValueType.X:
                ChangeRightValue(Target.GetPositionX());
                break;
            case ValueType.Y:
                ChangeRightValue(Target.GetPositionY());
                break;
            case ValueType.Z:
                ChangeRightValue(Target.GetPositionZ());
                break;
            case ValueType.Num:
                inputFieldRight.gameObject.SetActive(true);
                break;
        }
    }

    public void SetValueLeft(int value)
    {
        inputFieldLeft.gameObject.SetActive(false);

        valueTypeLeft = (ValueType)value;
    }

    public void SetValueRight(int value)
    {
        inputFieldRight.gameObject.SetActive(false);

        valueTypeRight = (ValueType)value;
    }

    public void ChangeLeftValue(string value)
    {
        float.TryParse(value, out valueLeft);
    }

    public void ChangeLeftValue(float value)
    {
        valueLeft = value;
    }

    public void ChangeRightValue(string value)
    {
        float.TryParse(value, out valueRight);
    }

    public void ChangeRightValue(float value)
    {
        valueRight = value;
    }

    public override bool IsConditionTrue()
    {
        return valueLeft < valueRight;
    }

    public string GetStringValue(int value)
    {
        return dropDownLeft.options[value].text;
    }
}
