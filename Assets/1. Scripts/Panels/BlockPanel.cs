using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (Block.draggedBlock == null) return;
        Block.draggedBlock.transform.SetParent(PanelManager.Current.DestroyedBlocks);
        Block.draggedBlock.DestroyBlock();
    }
}
