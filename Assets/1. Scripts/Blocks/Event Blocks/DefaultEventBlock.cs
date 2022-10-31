using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DefaultEventBlock : EventBlock
{
    public static List<DefaultEventBlock> defaultEventBlocks = new List<DefaultEventBlock>();

    public override DefaultEventBlock AsDefaultEventBlock()
    {
        return this;
    }

    public override void OnCreated()
    {
        base.OnCreated();

        defaultEventBlocks.Add(this);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        defaultEventBlocks.Remove(this);
    }

}