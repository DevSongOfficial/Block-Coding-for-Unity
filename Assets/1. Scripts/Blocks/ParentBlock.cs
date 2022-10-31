using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class ParentBlock : Block
{
    [HideInInspector] public RectTransform middleConnectedBlockArea; // UI를 이 영역에 드랍했을 때 중앙에 붙는다고 인식한다.

    [HideInInspector] public List<Block> childBlocks;

    public float Height { get; protected set; } // ParentBlock은 Height가 유동적임
    public float Width { get; private set; } // ParentBlock은 Width가 유동적임
    public static readonly float WidthDefault = 178;
    public static readonly int WIDTH_INCREASED_AMOUNT = 16; // ParentBlock 안에 ParentBlock이 추가될 때 늘릴 Width Amount

    private float childBlockAreaHeight;

    
    protected override void Awake()
    {
        base.Awake();

        InitializeBlock();

        childBlocks = new List<Block>();

        if(IsEventBlock())
        {
            middleConnectedBlockArea = null;
        }
        else
        {
            middleConnectedBlockArea = transform.Find("MiddleConnectedArea").GetComponent<RectTransform>();
        }
    }
    

    public virtual void InitializeBlock()
    {
        Height = HeightDefalut;
        Width = WidthDefault;
        childBlockAreaHeight = ActionBlockHeight;
        SetRectTransformHeight(Height);
    }

    
    public virtual void IncreaseHeight(float increasedAmount, bool isRecursive = true)
    {
        Height += increasedAmount;
        childBlockAreaHeight += increasedAmount;
        SetRectTransformHeight(Height);

        float yOffSet = increasedAmount * 0.5f;
        ChangeAnchoredPositionY(-yOffSet);

        topConnectedBlockArea.anchoredPosition += new Vector2(0, yOffSet);
        bottomConnectedBlockArea.anchoredPosition -= new Vector2(0, yOffSet);

        // 블록 아래에 붙어있는 블록들을 확장된 만큼 내림
        bottomConnectedBlock?.PushDown(increasedAmount);

        if (HasParentBlock() && isRecursive)
        {
            parentBlock.IncreaseHeight(increasedAmount);
        }
    }

    public virtual void DecreaseHeight(float decreasedAmount, bool isRecursive = true)
    {
        Height -= decreasedAmount;
        childBlockAreaHeight -= decreasedAmount;
        SetRectTransformHeight(Height);

        float yOffSet = decreasedAmount * 0.5f;
        ChangeAnchoredPositionY(yOffSet);

        topConnectedBlockArea.anchoredPosition -= new Vector2(0, yOffSet);
        bottomConnectedBlockArea.anchoredPosition += new Vector2(0, yOffSet);

        // 블록 아래에 붙어있는 블록들을 축소된 만큼 올림
        bottomConnectedBlock?.PushUp(decreasedAmount);

        if (HasParentBlock() && isRecursive)
        {
            parentBlock.DecreaseHeight(decreasedAmount);
        }
    }

    public void OnChildBlockAdded(Block block) 
    {
        if(childBlockAreaHeight < GetChildBlocksHeight()) // 크기가 증가해야하는 경우
        {
            float increasedHeight = block.GetBlockHeight();
            if (block.IsParentBlock() && childBlocks.Count == 1)
            {
                increasedHeight -= childBlockAreaHeight;
            }
            
            // 블록 높이를 확장시킴, 부모블록들까지 같이 확장
            IncreaseHeight(increasedHeight);
        }
    }

    public void OnChildBlockRemoved(Block block)
    {
        float decreasedHeight = block.GetBlockHeight();

        if(childBlocks.Count >= 1)
        {
            // 블록 높이를 축소시킴
            DecreaseHeight(decreasedHeight);
        }
        else // 블록을 제거한 후 남은 child블록이 하나도 없는 경우 default값으로 초기화
        {
            InitializeBlock();

            if (block.IsParentBlock())
            {
                if (IsEventBlock()) return;

                float yOffSet = (block.AsParentBlock().Height - childBlockAreaHeight) * 0.5f;
                ChangeAnchoredPositionY(yOffSet);

                bottomConnectedBlock?.PushUp(yOffSet * 2, true);
                parentBlock?.DecreaseHeight(decreasedHeight - childBlockAreaHeight);
            }
        }
    }

    public bool TryMiddleConnect(Block block)
    {
        if(HasChildBlock() == false)
        {
            AddChildBlock(block);
            
            block.SetAnchoredPosition(GetAnchoredPosition());
            block.AsParentBlock()?.MoveChildBlocks(block.DeltaPosition());

            return true;
        }

        return false;
    }

    public void AddChildBlock(Block block)
    {
        block.parentBlock?.RemoveChildBlock(block);

        childBlocks.Add(block);

        block.parentBlock = this;

        OnChildBlockAdded(block);
    }

    public void AddChildBlock(Block block, int childIndex, bool isRecursive = false)
    {
        block.parentBlock?.RemoveChildBlock(block);

        childBlocks.Insert(childIndex, block);

        block.parentBlock = this;

        OnChildBlockAdded(block);

        if(isRecursive && block.IsBottomConnected())
        {
            AddChildBlock(block.bottomConnectedBlock, childIndex + 1, true);
        }
    }

    public void RemoveChildBlock(Block block)
    {
        childBlocks.Remove(block);

        block.parentBlock = null;

        OnChildBlockRemoved(block);
    }

    public float GetChildBlocksHeight()
    {
        float height = 0;
        for (int i = 0; i < childBlocks.Count; i++)
        {
            var childBlock = childBlocks[i];
            if (childBlock.IsParentBlock())
            {
                height += childBlock.AsParentBlock().Height;
            }
            else
            {
                height += childBlock.HeightDefalut;
            }
        }

        return height;
    }

    public void MoveChildBlocks(Vector2 delta)
    {
        for(int i = 0; i < childBlocks.Count; i++)
        {
            var childBlock = childBlocks[i];
            childBlock.ChangeAnchoredPosition(delta);

            if (childBlock.IsParentBlock())
            {
                childBlock.AsParentBlock().MoveChildBlocks(delta);
            }
        }
    }

    public bool Contains(Block block)
    {
        for(int i = 0; i < childBlocks.Count; i++)
        {
            var childBlock = childBlocks[i];
            if(childBlock == block)
            {
                return true;
            }

            if (childBlock.IsParentBlock())
            {
                childBlock.AsParentBlock().Contains(block);
            }
        }

        return false;
    }

    public bool HasChildBlock()
    {
        return childBlocks.Count >= 1;
    }

    public override ParentBlock AsParentBlock()
    {
        return this;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        transform.SetAsLastSibling();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        MoveChildBlocks(eventData.delta / canvas.scaleFactor);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        SortInHierarchy();
    }
}