using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GamePanel : MonoBehaviour,IDragHandler, IPointerDownHandler
{
    private static GamePanel instance;
    public static GamePanel Instance { get { return instance; } }

    private RectTransform rectTransform;

    private CanvasGroup canvasGroup;
    [SerializeField] private Outline topPanelOutline;
    [SerializeField] private Button sizeChangeButton;

    private void Awake()
    {
        instance = this;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        sizeChangeButton.onClick.AddListener(OnChangeSize);

        GameManager.Instance.OnGameStart += (object sender, EventArgs e) => transform.SetAsLastSibling();
    }

    public void ScaleGamePanel(float amount)
    {
        rectTransform.localScale = Vector3.one * amount;
    }

    private void OnChangeSize()
    {
        bool isMaxSize = canvasGroup.blocksRaycasts;

        canvasGroup.blocksRaycasts = !isMaxSize;
        canvasGroup.alpha = !isMaxSize ? 1 : 0;
        topPanelOutline.enabled = isMaxSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / Block.canvas.scaleFactor;
    }
}