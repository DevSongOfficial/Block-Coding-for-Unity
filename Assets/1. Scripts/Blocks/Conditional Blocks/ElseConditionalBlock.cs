using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ElseConditionalBlock : ConditionalBlock
{
    private bool condition;

    public static ElseConditionalBlock Prefab { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Prefab = Resources.Load<ElseConditionalBlock>("Prefabs/Block Prefabs/Conditional_Else");
    }

    public override bool CheckCondition()
    {
        if (parentBlock != null)
        {
            int index = parentBlock.childBlocks.IndexOf(this);
            for (int i = index - 1; i >= 0; i--)
            {
                var block = parentBlock.childBlocks[i];
                if (block.IsConditionalBlock())
                {
                    if(block.AsIfConditionalBlock() != null)
                    {
                        return !block.AsIfConditionalBlock().LastCondition;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }
}