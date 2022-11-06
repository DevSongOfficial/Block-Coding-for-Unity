using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ForeverLoopBlock : LoopBlock
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator StartLoopFunctions()
    {
        yield return StartCoroutine(ForeverLoopFunctionsRoutine());
    }
}
