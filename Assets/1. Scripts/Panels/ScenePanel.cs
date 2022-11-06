using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RuntimeGizmos;

public class ScenePanel : MonoBehaviour, IDropHandler
{
    private static ScenePanel instance;
    public static ScenePanel Instance { get { return instance; } }

    private RectTransform rectTransform;

    public static Camera FieldCamera { get; private set; }
    public static Transform CameraMover { get; private set; }

    private GameObject topPanel;
    private Button createButton;
    private GameObject objectListPanel;
    private Button cubeButton;
    private Button sphereButton;
    private Button cylinderButton;
    private Button moveButton;
    private Button rotateButton;
    private Button scaleButton;

    private void Awake()
    {
        instance = this;

        rectTransform = GetComponent<RectTransform>();

        FieldCamera = Camera.main;
        CameraMover = FieldCamera.transform.parent;

        topPanel = transform.GetChild(0).gameObject;
        createButton = topPanel.transform.Find("NewObject Button").GetComponent<Button>();
        objectListPanel = topPanel.transform.Find("NewObjectList Panel").gameObject;
        cubeButton = objectListPanel.transform.GetChild(0).GetComponent<Button>();
        sphereButton = objectListPanel.transform.GetChild(1).GetComponent<Button>();
        cylinderButton = objectListPanel.transform.GetChild(2).GetComponent<Button>();
        moveButton = topPanel.transform.Find("Move Button").GetComponent<Button>();
        rotateButton = topPanel.transform.Find("Rotate Button").GetComponent<Button>();
        scaleButton = topPanel.transform.Find("Scale Button").GetComponent<Button>();

        createButton.onClick.AddListener(() => objectListPanel.SetActive(!objectListPanel.activeSelf));
        cubeButton.onClick.AddListener(() => GameManager.Instance.CreateNewTargetObject(GameManager.Instance.cubePrefab));
        sphereButton.onClick.AddListener(() => GameManager.Instance.CreateNewTargetObject(GameManager.Instance.spherePrefab));
        cylinderButton.onClick.AddListener(() => GameManager.Instance.CreateNewTargetObject(GameManager.Instance.cylinderPrefab));

        moveButton.onClick.AddListener(TransformGizmo.Instance.SetTransformTypeToMove);
        rotateButton.onClick.AddListener(TransformGizmo.Instance.SetTransformTypeToRotate);
        scaleButton.onClick.AddListener(TransformGizmo.Instance.SetTransformTypeToScale);
    }

    private void Update()
    {
        HandleCameraTransform();
    }


    #region Camera Transform
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetCursorPos(out MousePosition mousePosition);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int X, int Y);

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePosition
    {
        public int x;
        public int y;
    }

    MousePosition mousePosition;
    private float rotateSpeed = 150;
    private float xRotate;
    private float yRotate;

    private void HandleCameraTransform()
    {
        if (Block.MousePointOnTheArea(rectTransform, externalCall: true) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            xRotate = FieldCamera.transform.eulerAngles.x;
            yRotate = FieldCamera.transform.eulerAngles.y;

            Cursor.visible = false;
            GetCursorPos(out mousePosition);

            StartCoroutine(CameraRotateRoutine());
        }
    }

    private IEnumerator CameraRotateRoutine()
    {
        while(Input.GetKeyUp(KeyCode.Mouse1) == false)
        {
            float xRotateMove = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed;
            float yRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;

            xRotate += xRotateMove;
            yRotate += yRotateMove;

            FieldCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0);


            // Move while roatating
            Vector3 forwardDirection = FieldCamera.transform.localRotation * Vector3.forward;
            Vector3 rightDirection = FieldCamera.transform.localRotation * Vector3.right;
            Vector3 upDirection = FieldCamera.transform.localRotation * Vector3.up;

            float moveCoefficient = Input.GetKey(KeyCode.LeftShift) ? 20f : 7.5f;
            
            if (Input.GetKey(KeyCode.W))
            {
                CameraMover.Translate(forwardDirection * moveCoefficient * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                CameraMover.Translate(forwardDirection * -moveCoefficient * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                CameraMover.Translate(rightDirection * moveCoefficient * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                CameraMover.Translate(rightDirection * -moveCoefficient * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                CameraMover.transform.Translate(upDirection * moveCoefficient * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                CameraMover.transform.Translate(upDirection * -moveCoefficient * Time.deltaTime);
            }

            SetCursorPos((int)mousePosition.x, (int)mousePosition.y);

            yield return null;
        }

        Cursor.visible = true;
        SetCursorPos((int)mousePosition.x, (int)mousePosition.y);
    }

    public void CameraMoveForward(float moveAmount)
    {
        Vector3 forwardDirection = FieldCamera.transform.localRotation * Vector3.forward;
        CameraMover.transform.Translate(forwardDirection * moveAmount);
    }
    #endregion

    public void OnDrop(PointerEventData eventData)
    {
        if (Block.draggedBlock == null) return;
        Block.draggedBlock.DestroyBlock();
    }
}