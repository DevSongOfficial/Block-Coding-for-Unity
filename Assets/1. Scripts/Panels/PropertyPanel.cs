using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PropertyPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private static PropertyPanel instance;
    public static PropertyPanel Instance { get { return instance; } }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [Header("Top Panel")]
    [SerializeField] private Button sizeChangeButton;
    [SerializeField] private Outline topPanelOutline;

    [SerializeField] private TMP_InputField nameInputField;

    [Header("Transform Panel")]
    [SerializeField] private TMP_InputField locationXInputField;
    [SerializeField] private TMP_InputField locationYInputField;
    [SerializeField] private TMP_InputField locationZInputField;
    [SerializeField] private TMP_InputField rotationXInputField;
    [SerializeField] private TMP_InputField rotationYInputField;
    [SerializeField] private TMP_InputField rotationZInputField;
    [SerializeField] private TMP_InputField scaleXInputField;
    [SerializeField] private TMP_InputField scaleYInputField;
    [SerializeField] private TMP_InputField scaleZInputField;


    [Header("Other Settings Panel")]
    [SerializeField] private CanvasGroup otherSettingsCanvasGroup;
    [SerializeField] private Slider gameViewScaleSlider;

    [Header("Physics Panel")]
    [SerializeField] private Toggle ignoreCollisionToggle;
    [SerializeField] private Toggle useGravityToggle;
    [SerializeField] private TMP_InputField massInputField;
    [SerializeField] private TMP_InputField dragInputField;

    private void Awake()
    {
        instance = this;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        GameManager.Instance.OnTargetObjectRemoved += InitializeEveryText;
        GameManager.Instance.OnTargetObjectRemoved += DeactivatePropertyPanel;

        GameManager.Instance.OnTargetObjectSelected += ActivateAndInitializePropertyPanel;

        GameManager.Instance.OnGameStop += UpdateTransformPanel;

        sizeChangeButton.onClick.AddListener(OnChangeSize);

        nameInputField.onValueChanged.AddListener((string name) => { if (name != string.Empty) GameManager.CurrentTarget.SetName(name); });

        locationXInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetPositionX(newValue);
        });
        locationYInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetPositionY(newValue);
        });
        locationZInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetPositionZ(newValue);
        });
        rotationXInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetRotationX(newValue, externalCall: true);
        });
        rotationYInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetRotationY(newValue, externalCall: true);
        });
        rotationZInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetRotationZ(newValue, externalCall: true);
        });
        scaleXInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetScaleX(newValue);
        });
        scaleYInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetScaleY(newValue);
        });
        scaleZInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.SetScaleZ(newValue);
        });

        gameViewScaleSlider.onValueChanged.AddListener((float value) => GamePanel.Instance.ScaleGamePanel(value + 0.75f));

        ignoreCollisionToggle.onValueChanged.AddListener((bool value) => GameManager.CurrentTarget.Collider.enabled = !value);
        useGravityToggle.onValueChanged.AddListener((bool value) => GameManager.CurrentTarget.RigidBody.useGravity = value);
        massInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.RigidBody.mass = newValue;
        });
        dragInputField.onEndEdit.AddListener((string value) =>
        {
            float.TryParse(value, out float newValue);
            GameManager.CurrentTarget.RigidBody.drag = newValue;
        });
    }

    private void Update()
    {
        if (GameManager.InProgress && GameManager.CurrentTarget != null)
        {
            UpdateTransformPanel(GameManager.CurrentTarget.transform);
        }
    }

    private void UpdateTransformPanel(object o, EventArgs e)
    {
        if(GameManager.CurrentTarget != null)
        {
            UpdateTransformPanel(GameManager.CurrentTarget.transform);
        }
    }

    public void UpdateTransformPanel(Transform newTransform)
    {
        locationXInputField.text = newTransform.position.x.ToString();
        locationYInputField.text = newTransform.position.y.ToString();
        locationZInputField.text = newTransform.position.z.ToString();
        rotationXInputField.text = newTransform.eulerAngles.x.ToString();
        rotationYInputField.text = newTransform.eulerAngles.y.ToString();
        rotationZInputField.text = newTransform.eulerAngles.z.ToString();
        scaleXInputField.text = newTransform.localScale.x.ToString();
        scaleYInputField.text = newTransform.localScale.y.ToString();
        scaleZInputField.text = newTransform.localScale.z.ToString();
    }

    public void InitializeEveryText(object sender, Target target)
    {
        nameInputField.text = string.Empty;

        string stringIntializedValue = "0";
        locationXInputField.text = stringIntializedValue;
        locationYInputField.text = stringIntializedValue;
        locationZInputField.text = stringIntializedValue;
        rotationXInputField.text = stringIntializedValue;
        rotationYInputField.text = stringIntializedValue;
        rotationZInputField.text = stringIntializedValue;
        scaleXInputField.text = stringIntializedValue;
        scaleYInputField.text = stringIntializedValue;
        scaleZInputField.text = stringIntializedValue;
    }

    public void DeactivatePropertyPanel(object sender, Target target)
    {
        canvasGroup.interactable = false;
    }

    public void ActivateAndInitializePropertyPanel(object sender, Target target)
    {
        canvasGroup.interactable = true;

        nameInputField.text = target.Name;
        nameInputField.enabled = target == GameManager.Instance.gameCamera ? false : true;

        ignoreCollisionToggle.isOn = !target.Collider.enabled;
        massInputField.text = target.RigidBody.mass.ToString();
        dragInputField.text = target.RigidBody.drag.ToString();
    }

    public void OnChangeSize()
    {
        bool isMaxSize = canvasGroup.blocksRaycasts;

        canvasGroup.blocksRaycasts = !isMaxSize;
        canvasGroup.alpha = !isMaxSize ? 1 : 0;
        topPanelOutline.enabled = isMaxSize;

        otherSettingsCanvasGroup.blocksRaycasts = canvasGroup.blocksRaycasts;
        otherSettingsCanvasGroup.alpha = canvasGroup.alpha;
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