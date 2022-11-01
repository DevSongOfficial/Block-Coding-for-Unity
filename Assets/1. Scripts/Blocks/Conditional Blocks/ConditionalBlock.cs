using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionalBlock : ParentBlock
{
    protected override void Awake()
    {
        base.Awake();

        blockType = BlockType.ConditionalBlock;
    }

    public override void InitializeBlock()
    {
        base.InitializeBlock();

        topConnectedBlockArea.anchoredPosition = new Vector2(0, 54f);
        bottomConnectedBlockArea.anchoredPosition = new Vector2(0, -54f);
    }

    public abstract bool CheckCondition();

    public IEnumerator ConditionalFunctionsRoutine()
    {
        if (IsNeglected())
        {
            SetNeglect(false, applyToChildBlocks: true);
            yield break;
        }

        if (CheckCondition())
        {
            for (int i = 0; i < childBlocks.Count; i++)
            {
                var childBlock = childBlocks[i];

                yield return StartCoroutine(MoveOnToNextBlock(childBlock));
            }
        }
    }

    public override ConditionalBlock AsConditionalBlock()
    {
        return this;
    }
}