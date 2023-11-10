using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Target : MonoBehaviour, ISelectable
{
    [field: SerializeField] public string Name { get; private set; }
    public void SetName(string name)
    {
        Name = name;

        OnTargetNameChanged.Invoke(this, EventArgs.Empty);
    }

    // 직접 할당
    public Sprite iconSprite;

    // 외부 접근 용
    [HideInInspector] public MainPanel ConnectedMainPanel { get; private set; }
    [HideInInspector] public Rigidbody RigidBody { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; } // trigger용 collider가 아닌 collsion용 collider를 할당

    private Outline outline;

    // Custom events
    public event EventHandler OnTargetNameChanged;
    public event EventHandler<Target> OnCollisionEnterBetween;
    public event EventHandler<Target> OnCollisionExitBetween;
    
    private Transform transformBeforEventStarted;
    public struct Transform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
    public void UpdateTransformBeforEventStarted()
    {
        transformBeforEventStarted.position = transform.position;
        transformBeforEventStarted.rotation = transform.rotation;
        transformBeforEventStarted.scale = transform.localScale;
    }
    public void RevertToTrasnformBeforeEventStarted()
    {
        transform.position = transformBeforEventStarted.position;
        transform.rotation = transformBeforEventStarted.rotation;
        transform.localScale = transformBeforEventStarted.scale;
    }

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        outline.enabled = false;
        RigidBody.isKinematic = true;
    }

    public void GetSelected()
    {
        PropertyPanel.Instance.UpdateTransformPanel(transform);

        if(!GameManager.InProgress) PanelManager.Instance.DeactivateAllBlockPanels();
        ConnectedMainPanel.gameObject.SetActive(true);
        PanelManager.Instance.SetCurrentMainPanel(ConnectedMainPanel);
    }

    public void SetConnectedMainPanel(MainPanel mainPanel)
    {
        ConnectedMainPanel = mainPanel;
    }

    public void ChangeActivation()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void AddOutline() // outline.eraseRenderer는 오브젝트가 겹쳐있을 때 오류 발생해서 enabled로 함
    {
        outline.enabled = true;
    }

    public void RemoveOutline()
    {
        outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var collisionTaret = other.gameObject.GetComponent<Target>();
        if (collisionTaret != null)
        {
            OnCollisionEnterBetween?.Invoke(this, collisionTaret);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var collisionTaret = other.gameObject.GetComponent<Target>();
        if (collisionTaret != null)
        {
            OnCollisionExitBetween?.Invoke(this, collisionTaret);
        }
    }

    #region Position
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public float GetPositionX()
    {
        return GetPosition().x;
    }

    public float GetPositionY()
    {
        return GetPosition().y;
    }

    public float GetPositionZ()
    {
        return GetPosition().z;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;

        PropertyPanel.Instance.UpdateTransformPanel(transform);
    }

    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }

    public void SetPositionX(float x)
    {
        SetPosition(x, GetPositionY(), GetPositionZ());
    }

    public void SetPositionY(float y)
    {
        SetPosition(GetPositionX(), y, GetPositionZ());
    }

    public void SetPositionZ(float z)
    {
        SetPosition(GetPositionX(), GetPositionY(), z);
    }

    public void ChangePosition(Vector3 delta)
    {
        SetPosition(GetPosition() + delta);
    }

    public void ChangePosition(float x, float y, float z)
    {
        ChangePosition(new Vector3(x, y, z));
    }

    public void ChangePositionX(float x)
    {
        SetPositionX(GetPositionX() + x);
    }

    public void ChangePositionY(float y)
    {
        SetPositionY(GetPositionY() + y);
    }

    public void ChangePositionZ(float z)
    {
        SetPositionZ(GetPositionZ() + z);
    }
    #endregion

    #region Rotation

    public float GetRotationX()
    {
        return transform.eulerAngles.x;
    }

    public float GetRotationY()
    {
        return transform.eulerAngles.y;
    }

    public float GetRotationZ()
    {
        return transform.eulerAngles.z;
    }
    public void SetRotation(Vector3 rotation, bool externalCall = false)
    {
        float x = Mathf.Round(rotation.x * 100000) * 0.00001f;
        float y = Mathf.Round(rotation.y * 100000) * 0.00001f;
        float z = Mathf.Round(rotation.z * 100000) * 0.00001f;

        transform.rotation = Quaternion.Euler(x, y, z);

        if (!externalCall)
        {
            PropertyPanel.Instance.UpdateTransformPanel(transform);
        }
    }

    public void SetRotation(float x, float y, float z, bool externalCall = false)
    {
        SetRotation(new Vector3(x, y, z), externalCall);
    }


    public void SetRotationX(float x, bool externalCall = false)
    {
        SetRotation(x, GetRotationY(), GetRotationZ(), externalCall);
    }

    public void SetRotationY(float y, bool externalCall = false)
    {
        SetRotation(GetRotationX(), y, GetRotationZ(), externalCall);
    }

    public void SetRotationZ(float z, bool externalCall = false)
    {
        SetRotation(GetRotationX(), GetRotationY(), z, externalCall);
    }
    #endregion

    #region Scale
    public Vector3 GetScale()
    {
        return transform.localScale;
    }

    public float GetScaleX()
    {
        return GetScale().x;
    }

    public float GetScaleY()
    {
        return GetScale().y;
    }

    public float GetScaleZ()
    {
        return GetScale().z;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;

        PropertyPanel.Instance.UpdateTransformPanel(transform);
    }

    public void SetScale(float x, float y, float z)
    {
        SetScale(new Vector3(x, y, z));
    }

    public void SetScaleX(float x)
    {
        SetScale(x, GetScaleY(), GetScaleZ());
    }

    public void SetScaleY(float y)
    {
        SetScale(GetScaleX(), y, GetScaleZ());
    }

    public void SetScaleZ(float z)
    {
        SetScale(GetScaleX(), GetScaleY(), z);
    }
    #endregion
}
