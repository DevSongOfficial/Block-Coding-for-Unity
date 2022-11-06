using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionBlock : Block
{
    [HideInInspector] public ConditionalBlock connectedConditionalBlock;

    public static readonly float WidthDefault = 100;

    protected override void Awake()
    {
        base.Awake();

        blockType = BlockType.ConditionBlock;
    }

    public void DisconnectFromConditionalBlock()
    {
        connectedConditionalBlock?.OnDisconnectedFromConditionBlock();
    }

    public abstract bool IsConditionTrue();

    public override ConditionBlock AsConditionBlock()
    {
        return this;
    }
}