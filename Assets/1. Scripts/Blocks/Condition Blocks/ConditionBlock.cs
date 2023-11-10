using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionBlock : Block
{
    [HideInInspector] public IfConditionalBlock connectedConditionalBlock;

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

    public bool IsConnected()
    {
        return connectedConditionalBlock != null;
    }

    public abstract bool IsConditionTrue();

    public override ConditionBlock AsConditionBlock()
    {
        return this;
    }
}