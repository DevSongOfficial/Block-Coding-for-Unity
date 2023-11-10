using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class IfConditionalBlock : ConditionalBlock
{
    public ConditionBlock connectedConditionBlock;
    [HideInInspector] public RectTransform conditionBlockArea;

    private bool fromBlockPanel;

    public bool LastCondition { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        conditionBlockArea = transform.Find("ConditionConnectedArea").GetComponent<RectTransform>();
    }

    public void TryConditionConnect(ConditionBlock conditionBlock)
    {
        if (HasConditionBlock()) return;
        if (conditionBlock.IsConnected()) return;

        connectedConditionBlock = conditionBlock;
        conditionBlock.connectedConditionalBlock = this;

        float xOffset = (conditionBlock.GetRectTransformWidth() - ConditionBlock.WidthDefault) * 0.5f;
        float yOffset = (HeightDefalut - Height) * -0.5f;
        var newPosition = conditionBlockArea.anchoredPosition + GetAnchoredPosition() + new Vector2(xOffset, yOffset);
        conditionBlock.SetAnchoredPosition(newPosition);
    }

    public void OnDisconnectedFromConditionBlock()
    {
        connectedConditionBlock.connectedConditionalBlock = null;

        float difference = connectedConditionBlock.GetRectTransformWidth() - ConditionBlock.WidthDefault;

        connectedConditionBlock = null;
    }

    public override bool CheckCondition()
    {
        LastCondition = HasConditionBlock() ? connectedConditionBlock.IsConditionTrue() : false;

        return LastCondition;
    }

    public override void MoveChildBlocks(Vector2 delta)
    {
        if (HasConditionBlock())
        {
            AsIfConditionalBlock().connectedConditionBlock.ChangeAnchoredPosition(delta);
        }

        base.MoveChildBlocks(delta);
    }

    public bool HasConditionBlock()
    {
        return connectedConditionBlock != null;
    }

    public override void CreateNewBlockOnBlockPanel()
    {
        base.CreateNewBlockOnBlockPanel();

        fromBlockPanel = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (fromBlockPanel)
        {
            var elseBlock = Instantiate(ElseConditionalBlock.Prefab, PanelManager.Current.ParentBlocksPanel);
            elseBlock.SetTarget(Target);

            Block.BottomConnect(elseBlock, this);
        }

        fromBlockPanel = false;

    }

    public override IfConditionalBlock AsIfConditionalBlock()
    {
        return this;
    }
}
