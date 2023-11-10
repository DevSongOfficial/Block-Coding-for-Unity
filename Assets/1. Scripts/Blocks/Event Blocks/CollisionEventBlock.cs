using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class CollisionEventBlock : EventBlock
{
    private TMP_Dropdown dropdown;

    private Target targetForCollision;

    private static List<TMP_Dropdown.OptionData> dropdownOptionDatas = new List<TMP_Dropdown.OptionData>();

    private void UpdateDropdownText(object sender, Target target)
    {
        UpdateDropdownText(sender, EventArgs.Empty);
    }

    private void UpdateDropdownText(object sender, EventArgs e)
    {
        dropdown.options.Clear();

        TMP_Dropdown.OptionData dropdownOptionData = new TMP_Dropdown.OptionData();
        dropdownOptionData.text = "Select Target";
        dropdown.options.Add(dropdownOptionData);

        // TODO: Insert로 수정바람
        foreach (Target targetInTheScene in GameManager.Instance.EveryTargetInTheScene)
        {
            dropdownOptionData = new TMP_Dropdown.OptionData();
            dropdownOptionData.text = targetInTheScene.Name;
            dropdown.options.Add(dropdownOptionData);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.options = dropdownOptionDatas;
        dropdown.onValueChanged.AddListener(SetTargetForCollision);

        GameManager.Instance.OnNewTargetObjectCreated += UpdateDropdownText;
        GameManager.Instance.OnNewTargetObjectCreated += (object sender, Target target) => target.OnTargetNameChanged += UpdateDropdownText;
        GameManager.Instance.OnTargetObjectRemoved += UpdateDropdownText;
        GameManager.Instance.OnTargetObjectRemoved += (object sender, Target target) => target.OnTargetNameChanged -= UpdateDropdownText;
    }

    public override void SetTarget(Target target)
    {
        base.SetTarget(target);

        Target.OnCollisionEnterBetween += OnCollisionEnterBetween;
    }

    private void OnCollisionEnterBetween(object myTarget, Target collider)
    {
        if (GameManager.InProgress)
        {
            if(collider == targetForCollision)
            {
                StartEvent();
            }
        }
    }

    public void SetTargetForCollision(int index)
    {
        if (index == 0) // 0번째는 설명용 
        {
            targetForCollision = null;
            return;
        }

        targetForCollision = GameManager.Instance.EveryTargetInTheScene[index - 1];
    }
}
