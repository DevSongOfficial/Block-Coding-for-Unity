using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveButton : Button
{
    protected override void Awake()
    {
        onClick.AddListener(RemoveTargetObject);
    }

    private void RemoveTargetObject()
    {
        if(GameManager.CurrentTarget != null)
        {
            GameManager.Instance.RemoveTargetObject(GameManager.CurrentTarget);
        }
    }
}
