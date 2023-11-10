using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class CollisionStayConditionBlock : ConditionBlock
{
    private TMP_Dropdown dropdown;
    private static List<TMP_Dropdown.OptionData> dropdownOptionDatas = new List<TMP_Dropdown.OptionData>();

    private List<Target> collidersOfTarget = new List<Target>();
    private Target targetForCollision;


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
        Target.OnCollisionExitBetween += OnCollisionExitBetween;
    }

    private void UpdateDropdownText(object sender, Target target)
    {
        UpdateDropdownText(sender, EventArgs.Empty);
    }

    private void UpdateDropdownText(object sender, EventArgs e)
    {
        dropdown.options.Clear();

        TMP_Dropdown.OptionData dropdownOptionData = new TMP_Dropdown.OptionData();
        dropdownOptionData.text = "Target";
        dropdown.options.Add(dropdownOptionData);

        // TODO: Insert로 수정바람
        foreach (Target targetInTheScene in GameManager.Instance.EveryTargetInTheScene)
        {
            dropdownOptionData = new TMP_Dropdown.OptionData();
            dropdownOptionData.text = targetInTheScene.Name;
            dropdown.options.Add(dropdownOptionData);
        }
    }

    private void OnCollisionEnterBetween(object myTarget, Target collider)
    {
        collidersOfTarget.Add(collider);
    }

    private void OnCollisionExitBetween(object myTarget, Target collider)
    {
        collidersOfTarget.Remove(collider);
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

    public override bool IsConditionTrue()
    {
        return collidersOfTarget.Contains(targetForCollision);
    }
}
