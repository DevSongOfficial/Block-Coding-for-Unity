using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LoopBlock : ParentBlock
{
    private TMP_InputField inputField;

    private int loopTimes;
    private float loopDelay;
    public static readonly float Default_Loop_Delay = 0.1f;

    public IEnumerator LoopCoroutine;

    protected override void Awake()
    {
        base.Awake();

        loopDelay = Default_Loop_Delay;

        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(ChangeLoopTimes);
        
        LoopCoroutine = LoopFunctionsRoutine();

        blockType = BlockType.LoopBlock;
    }

    public override void InitializeBlock()
    {
        base.InitializeBlock();

        topConnectedBlockArea.anchoredPosition = new Vector2(0, 54f);
        bottomConnectedBlockArea.anchoredPosition = new Vector2(0, -59f);
    }

    public void ChangeLoopTimes(string times)
    {
        int.TryParse(times, out loopTimes);
    }

    public IEnumerator StartLoopFunctions()
    {
        LoopCoroutine = LoopFunctionsRoutine();
        yield return StartCoroutine(LoopCoroutine);
    }

    public void StopLoopFunctions()
    {
        if (LoopCoroutine != null) 
        {
            StopCoroutine(LoopCoroutine);
        }
    }
    
    public IEnumerator LoopFunctionsRoutine()
    {
        for(int i = 0; i < loopTimes; i++)
        {
            if (IsNeglected())
            {
                SetNeglect(false, applyToChildBlocks: true);
                yield break;
            }

            for(int j = 0; j < childBlocks.Count; j++)
            {
                var childBlock = childBlocks[j];

                yield return StartCoroutine(MoveOnToNextBlock(childBlock));

                yield return childBlock.IsNeglected() ? null : new WaitForSecondsRealtime(EXCUTION_DELAY);
            }

            yield return IsNeglected() ? null : new WaitForSecondsRealtime(loopDelay);
        }
    }

    public override LoopBlock AsLoopBlock()
    {
        return this;
    }
}