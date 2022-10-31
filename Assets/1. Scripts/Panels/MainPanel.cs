using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block Panel + Logic Panel + DestroyBlocks�� ������ �ִ� MainPanel ������Ʈ�� ��ũ��Ʈ
public class MainPanel : MonoBehaviour 
{
    public Target Target { get; private set; } // ����� TargetObject
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

    // Block Panel: �巡���ؼ� ������ ��ϵ��� ��Ƴ��� �г�
    public Transform BlockPanel { get; private set; }
    public Transform BlockPanelScrollView { get; private set; }
    public Transform BlockPanelContent { get; private set; }

    // Logic Panel: Block Panel���� �巡���ؼ� ������ ��ϵ��� �׾Ƽ� logic�� ����� ��
    public Transform LogicPanel { get; private set; }
    public Transform LogicPanelScrollView { get; private set; }
    public Transform LogicPanelContent { get; private set; }
    public Transform ParentBlocksPanel { get; private set; }
    public Transform SimpleBlocksPanel { get; private set; }

    // DestryedBlocks: DestroyBlock() ȣ�� �� �Ʒ� �гη� �̵���Ų �� ��Ȱ��ȭ
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
