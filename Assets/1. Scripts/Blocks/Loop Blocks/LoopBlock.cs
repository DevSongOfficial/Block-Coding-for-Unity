using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public abstract class LoopBlock : ParentBlock
{
    protected int loopTimes;

    protected override void Awake()
    {
        base.Awake();
        
        blockType = BlockType.LoopBlock;
    }

    public override void InitializeBlock()
    {
        base.InitializeBlock();

        topConnectedBlockArea.anchoredPosition = new Vector2(0, 54f);
        bottomConnectedBlockArea.anchoredPosition = new Vector2(0, -54f);
    }

    public void ChangeLoopTimes(string times)
    {
        int.TryParse(times, out loopTimes);
    }

    public abstract IEnumerator StartLoopFunctions();
    
    public IEnumerator DefaultLoopFunctionsRoutine()
    {
        for(int i = 0; i < loopTimes; i++)
        {
            if (GameManager.InProgress == false) yield break;

            if (IsNeglected())
            {
                SetNeglect(false, applyToChildBlocks: true);
                yield break;
            }

            for(int j = 0; j < childBlocks.Count; j++)
            {
                var childBlock = childBlocks[j];

                yield return StartCoroutine(MoveOnToNextBlock(childBlock));
            }
        }
    }

    public IEnumerator ForeverLoopFunctionsRoutine()
    {
        while (GameManager.InProgress)
        {
            if (IsNeglected())
            {
                SetNeglect(false, applyToChildBlocks: true);
                yield break;
            }

            for (int j = 0; j < childBlocks.Count; j++)
            {
                var childBlock = childBlocks[j];

                yield return StartCoroutine(MoveOnToNextBlock(childBlock));
            }
        }
    }

    public override LoopBlock AsLoopBlock()
    {
        return this;
    }
}