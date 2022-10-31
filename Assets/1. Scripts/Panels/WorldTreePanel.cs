using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTreePanel : MonoBehaviour
{
    private static WorldTreePanel instance;
    public static WorldTreePanel Instance { get { return instance; } }

    public List<TapButton> tapButtonsInWorldTree = new List<TapButton>();

    // Scroll View 관련
    private Transform ScrollView;
    private Transform Content;

    private TapButton tapButtonPrefab; // World View 내에 정렬된 탭 프리팹

    private void Awake()
    {
        instance = this;

        tapButtonPrefab = Resources.Load<TapButton>("Prefabs/Tap Button");

        ScrollView = transform.Find("Scroll View");
        Content = ScrollView.GetChild(0).GetChild(0);

        GameManager.Instance.OnNewTargetObjectCreated += CreateNewTapButton;
    }

    private void CreateNewTapButton(object sender, Target target)
    {
        TapButton newTapButton = Instantiate(tapButtonPrefab, Content);
        newTapButton.SetTargetAndInitialize(target);
        tapButtonsInWorldTree.Add(newTapButton);
    }
}