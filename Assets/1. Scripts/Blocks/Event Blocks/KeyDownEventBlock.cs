using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class KeyDownEventBlock : EventBlock
{
    private TMP_Dropdown dropdown;

    private KeyCode keyCode = KeyCode.None;

    protected override void Awake()
    {
        base.Awake();

        dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();

        dropdown.onValueChanged.AddListener(SetKeyCode);
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.InProgress)
        {
            if (Input.GetKeyDown(keyCode))
            {
                StartEvent();
            }
        }
    }

    public void SetKeyCode(int index)
    {
        if(index == 0) // 0번째는 설명용 키코드
        {
            keyCode = KeyCode.None;
            return;
        }

        string keyString = GetStringValue(index);
        keyCode = (KeyCode)(System.Enum.Parse(typeof(KeyCode), keyString));
    }

    public string GetStringValue(int value)
    {
        return dropdown.options[value].text;
    }
}