using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TapButton : Button
{
    public Target Target { get; private set; } // ¿¬°áµÈ TargetObject
    public void SetTargetAndInitialize(Target target)
    {
        Target = target;

        Initialization();
    }

    private TextMeshProUGUI nameText;
    private Image iconImage;
    private Button eyeButton;
    private GameObject barImage;

    private float clickTime;

    private void Initialization()
    {
        GameManager.Instance.OnTargetObjectSelected += ChangeNormalColor;
        GameManager.Instance.OnTargetObjectRemoved += RemoveConnectedTapButton;

        nameText = transform.Find("Name Text").GetComponent<TextMeshProUGUI>();
        iconImage = transform.Find("Icon Image").GetComponent<Image>();
        eyeButton = transform.Find("Eye Button").GetComponent<Button>();
        barImage = eyeButton.transform.GetChild(0).gameObject;
        eyeButton.onClick.AddListener(Target.ChangeActivation);
        eyeButton.onClick.AddListener(() => barImage.SetActive(!barImage.activeSelf));

        UpdateNameText(this, EventArgs.Empty);
        SetIconSprite(Target.iconSprite);

        Target.OnTargetNameChanged += UpdateNameText;
    }

    private Color Color_SkyBlue = new Color(0, 0.6f, 1);
    public void ChangeNormalColor(object sender, Target target)
    {
        ColorBlock colorBlock = colors;

        colorBlock.normalColor = target == Target ? Color_SkyBlue : Color.white;

        colors = colorBlock;
    }

    private void SetNameText(string name)
    {
        nameText.text = name;
    }

    private void UpdateNameText(object sender, EventArgs e)
    {
        SetNameText(Target.Name);
    }

    private void RemoveConnectedTapButton(object sender, Target removedTarget)
    {
        if(removedTarget == Target)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetIconSprite(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    private void OnDoubleClicked()
    {
        ScenePanel.FieldCamera.transform.LookAt(Target.transform);
        float distance = Vector3.Distance(ScenePanel.CameraMover.position, Target.transform.position);
        float distanceAfterMoving = 7.5f;

        ScenePanel.Instance.CameraMoveForward(distance - distanceAfterMoving);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        GameManager.Instance.SetCurrentTarget(Target);

        if (Time.time - clickTime < 0.25f)
        {
            OnDoubleClicked();
        }
        else
        {
            clickTime = Time.time;
        }
    }
}
