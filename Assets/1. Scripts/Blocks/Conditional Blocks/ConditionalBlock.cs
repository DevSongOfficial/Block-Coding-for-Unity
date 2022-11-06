using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class ConditionalBlock : ParentBlock
{
    [HideInInspector] public RectTransform conditionBlockArea;

    public ConditionBlock connectedConditionBlock;

    protected override void Awake()
    {
        base.Awake();

        blockType = BlockType.ConditionalBlock;

        conditionBlockArea = transform.Find("ConditionConnectedArea").GetComponent<RectTransform>();
    }

    public override void InitializeBlock()
    {
        base.InitializeBlock();

        topConnectedBlockArea.anchoredPosition = new Vector2(0, 54f);
        bottomConnectedBlockArea.anchoredPosition = new Vector2(0, -54f);
    }

    public void TryConditionConnect(ConditionBlock conditionBlock)
    {
        if (connectedConditionBlock != null) return;
        if (conditionBlock.connectedConditionalBlock != null) return;

        connectedConditionBlock = conditionBlock;
        conditionBlock.connectedConditionalBlock = this;

        float xOffset = (conditionBlock.GetRectTransformWidth() - ConditionBlock.WidthDefault) * 0.5f;
        float yOffset = (HeightDefalut - Height) * -0.5f;
        var newPosition = AsConditionalBlock().conditionBlockArea.anchoredPosition + AsConditionalBlock().GetAnchoredPosition() + new Vector2(xOffset, yOffset);
        conditionBlock.SetAnchoredPosition(newPosition);
    }

    public void OnDisconnectedFromConditionBlock()
    {
        connectedConditionBlock.connectedConditionalBlock = null;

        float difference = connectedConditionBlock.GetRectTransformWidth() - ConditionBlock.WidthDefault;

        connectedConditionBlock = null;
    }

    public bool CheckCondition()
    {
        if(connectedConditionBlock == null)
        {
            return false;
        }

        return connectedConditionBlock.IsConditionTrue();
    }

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

    public override void MoveChildBlocks(Vector2 delta)
    {
        if (connectedConditionBlock != null)
        {
            AsConditionalBlock().connectedConditionBlock.ChangeAnchoredPosition(delta);
            
        }

        base.MoveChildBlocks(delta);
    }

    public override ConditionalBlock AsConditionalBlock()
    {
        return this;
    }
}