using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakActionBlock : ActionBlock
{
    public override IEnumerator DoFunction()
    {
        FindNearestLoopBlock(out LoopBlock loopBlock, out Block childOfLoopBlock);

        if (loopBlock != null)
        {
            loopBlock.SetNeglect(true, applyToChildBlocks: true);

            yield break;
        }
        else
        {
            // ���� ������ ���� ��� �ƹ� ���� X
            yield break;
        }
    }

}