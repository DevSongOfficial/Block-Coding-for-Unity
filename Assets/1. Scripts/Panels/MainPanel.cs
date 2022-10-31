using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block Panel + Logic Panel + DestroyBlocks를 가지고 있는 MainPanel 오브젝트의 스크립트
public class MainPanel : MonoBehaviour 
{
    public Target Target { get; private set; } // 연결된 TargetObject
    public void SetTarget(Target target)
    {
        Target = target;

        target.SetConnectedMainPanel(this);

        for (int i = 0; i < BlockPanelContent.childCount; i++)
        {
            var block = BlockPanelContent.GetChild(i).GetComponent<Block>();
            block.SetTarget(target);
        }
    }

    // Block Panel: 드래그해서 가져올 블록들을 모아놓은 패널
    public Transform BlockPanel { get; private set; }
    public Transform BlockPanelScrollView { get; private set; }
    public Transform BlockPanelContent { get; private set; }

    // Logic Panel: Block Panel에서 드래그해서 가져온 블록들을 쌓아서 logic을 만드는 곳
    public Transform LogicPanel { get; private set; }
    public Transform LogicPanelScrollView { get; private set; }
    public Transform LogicPanelContent { get; private set; }
    public Transform ParentBlocksPanel { get; private set; }
    public Transform SimpleBlocksPanel { get; private set; }

    // DestryedBlocks: DestroyBlock() 호출 시 아래 패널로 이동시킨 뒤 비활성화
    public Transform DestroyedBlocks { get; private set; } 

    private void Awake()
    {
        BlockPanel = transform.Find("Block Panel");
        BlockPanelScrollView = BlockPanel.Find("Scroll View");
        BlockPanelContent = BlockPanelScrollView.GetChild(0).GetChild(0);

        LogicPanel = transform.Find("Logic Panel");
        LogicPanelScrollView = LogicPanel.Find("Scroll View");
        LogicPanelContent = LogicPanelScrollView.GetChild(0).GetChild(0);
        ParentBlocksPanel = LogicPanelContent.Find("ParentBlocks");
        SimpleBlocksPanel = LogicPanelContent.Find("SimpleBlocks");

        DestroyedBlocks = transform.Find("DestroyedBlocks");

        GameManager.Instance.OnTargetObjectRemoved += RemoveThePanel;
    }

    public void RemoveThePanel(object sender, Target removedTarget)
    {
        if(Target == removedTarget)
        {
            PanelManager.Instance.mainPanelList.Remove(this);
            gameObject.SetActive(false);
        }
    }
}
