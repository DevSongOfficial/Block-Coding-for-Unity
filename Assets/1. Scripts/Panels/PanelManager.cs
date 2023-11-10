using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    private static PanelManager instance;
    public static PanelManager Instance { get { return instance; } }
    public static MainPanel Current { get { return instance.currentMainPanel; } }

    // Main Panel (Block Panel + Logic Panel�� ����)
    private MainPanel mainPanelPrefab;
    [HideInInspector] public MainPanel currentMainPanel; 
    [HideInInspector] public List<MainPanel> mainPanelList = new List<MainPanel>();

    private Vector3 logicPanelContent_InitialPosition;
    [HideInInspector] public Vector3 LogicPanelContentPositionDelta 
    { get { return logicPanelContent_InitialPosition - currentMainPanel.LogicPanelContent.transform.position; } }

    private Transform mainPanels;

    private void Awake()
    {
        instance = this;

        mainPanels = transform.Find("Main Panels");
        mainPanelPrefab = Resources.Load<MainPanel>("Prefabs/Main Panel");
        
        GameManager.Instance.OnNewTargetObjectCreated += CreateNewMainPanel;
    }

    private void Start()
    {
        #region �ʱⰪ ������ ���� ��� �����ߴٰ� �ı�, (PanelManager���� GameManager�� ���� ����Ǳ⶧���� ���� �ڵ�, ���� �ٶ�)
        var targetObject = GameManager.Instance.CreateNewTargetObject(GameManager.Instance.cubePrefab);
        mainPanelList.Add(targetObject.ConnectedMainPanel);
        currentMainPanel = mainPanelList[0];
        currentMainPanel.gameObject.SetActive(true);
        GameManager.Instance.SetCurrentTarget(targetObject);

        logicPanelContent_InitialPosition = currentMainPanel.LogicPanelContent.transform.position;

        GameManager.Instance.RemoveTargetObject(targetObject);
        #endregion
    }

    public void CreateNewMainPanel(object sender, Target target)
    {
        MainPanel newMainPanel = Instantiate(mainPanelPrefab, transform.position, Quaternion.identity, mainPanels);
        mainPanelList.Add(newMainPanel);
        newMainPanel.SetTarget(target);
        newMainPanel.gameObject.SetActive(false);
        newMainPanel.transform.SetAsFirstSibling();
    }

    // �гε��� ������� ���� ���� Ȥ�� �� ������ �����ϱ� ���� ��Ȱ��ȭ ��
    // EventBlock�� ����Ǿ�� �� ���� ��� Ȱ��ȭ ��
    public void ActivateAllBlockPanels()
    {
        for (int i = 0; i < mainPanelList.Count; i++)
        {
            mainPanelList[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateAllBlockPanels()
    {
        for(int i = 0; i < mainPanelList.Count; i++)
        {
            mainPanelList[i].gameObject.SetActive(false);
        }
    }

    public void SetCurrentMainPanel(MainPanel mainPanel)
    {
        currentMainPanel = mainPanel;
        currentMainPanel.transform.SetAsLastSibling();
    }
}
