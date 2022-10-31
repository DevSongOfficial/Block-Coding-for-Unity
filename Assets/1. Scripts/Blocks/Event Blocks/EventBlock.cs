using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class EventBlock : ParentBlock
{
    public IEnumerator EventCoroutine;

    public static List<EventBlock> eventBlocks = new List<EventBlock>();

    protected override void Awake()
    {
        base.Awake();

        blockType = BlockType.EventBlock;
    }

    public virtual void OnCreated()
    {
        // Do nothing in this class
    }

    public virtual void OnDestroyed()
    {
        // Do nothing in this class
    }

    public void StartEvent()
    {
        EventCoroutine = EventRoutine();
        StartCoroutine(EventCoroutine);
    }

    public override void BottomConnect(Block block)
    {
        base.BottomConnect(block);

        AddChildBlock(block, 0);
    }

    public IEnumerator EventRoutine()
    {
        for(int i = 0; i < childBlocks.Count; i++)
        {
            var childBlock = childBlocks[i];

            yield return StartCoroutine(MoveOnToNextBlock(childBlock));

            yield return new WaitForSecondsRealtime(EXCUTION_DELAY);
        }
    }

    public override EventBlock AsEventBlock()
    {
        return this;
    }

    public virtual DefaultEventBlock AsDefaultEventBlock()
    {
        return null;
    }

    public override void IncreaseHeight(float increasedAmount, bool isRecursive = true)
    {
        // Do nothing in this class
    }

    public override void DecreaseHeight(float decreasedAmount, bool isRecursive = true)
    {
        // Do nothing in this class
    }
}