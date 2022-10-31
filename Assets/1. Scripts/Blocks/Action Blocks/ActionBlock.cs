using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ActionBlock : Block
{
    protected override void Awake()
    {
        base.Awake();

        blockType = BlockType.ActionBlock;
    }

    public abstract IEnumerator DoFunction();

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        transform.SetAsLastSibling();
    }

    public override ActionBlock AsActionBlock()
    {
        return this;
    }
}