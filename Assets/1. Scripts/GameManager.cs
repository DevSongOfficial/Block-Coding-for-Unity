using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface ISelectable
{
    void GetSelected();
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static bool InProgress { get; private set; } // return if the game has started

    // Custom Events
    public event EventHandler<Target> OnTargetObjectSelected;
    public event EventHandler<Target> OnNewTargetObjectCreated;
    public event EventHandler<Target> OnTargetObjectRemoved;

    public static Target CurrentTarget { get; private set; } // 현재 선택된 TargetObject
    public static Target PreviousTarget { get; private set; } 
    public void SetCurrentTarget(Target targetObject)
    {
        PreviousTarget = CurrentTarget;

        if (PreviousTarget != null)
        {
            PreviousTarget.RemoveOutline();
        }

        CurrentTarget = targetObject.GetComponent<Target>();
        CurrentTarget.GetComponent<ISelectable>().GetSelected();

        targetObject.AddOutline();

        OnTargetObjectSelected?.Invoke(this, CurrentTarget);
    }

    public List<Target> EveryTargetInTheScene { get; private set; }

    // Target Object Prefabs
    [HideInInspector] public Target cubePrefab;     // [1] Cube
    [HideInInspector] public Target spherePrefab;   // [2] Sphere
    [HideInInspector] public Target cylinderPrefab; // [3] Cylinder

    private void Awake()
    {
        instance = this;

        EveryTargetInTheScene = new List<Target>();

        // Load Target Object Prefabs
        cubePrefab = Resources.Load<Target>("Prefabs/Target_Cube");
        spherePrefab = Resources.Load<Target>("Prefabs/Target_Sphere");
        cylinderPrefab = Resources.Load<Target>("Prefabs/Target_Cylinder");

        Screen.SetResolution(1280, 720, true);
    }

    private void Start()
    {
        Block.InitializeAllBlocksInTheScene();
    }

    void Update()
    {
        HandleObjectSelection();
        HandleDraggedBlockHighlight();
    }

    private void HandleObjectSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            if (hit.collider != null && hit.collider.GetComponent<ISelectable>() != null)
            {
                SetCurrentTarget(hit.transform.GetComponent<Target>());
            }
        }
    }

    private void HandleDraggedBlockHighlight()
    {
        if (Block.draggedBlock == null) return;

        bool canConnect = false;
        for (int i = 0; i < Block.AllBlocksInTheScene.Count; i++)
        {
            var block = Block.AllBlocksInTheScene[i];
            if (block.OnTheLogicPanel())
            {
                if (block.CanConnectToDraggedBlock())
                {
                    canConnect = true;
                }
            }
        }
        
        if (canConnect)
        {
            Block.draggedBlock.HighlighteOn(new Color(1, 0.85f, 0, 1));

        }
        else Block.draggedBlock.HighlightOff();
    }

    // Start The Game
    // Default Event Block 모두 실행 => Scratch의 초록 깃발 버튼과 동일 기능
    [ContextMenu("StartGame")]
    public void GameStart()
    {
        InProgress = true;

        PanelManager.Instance.ActivateAllBlockPanels();

        for(int i = 0; i < DefaultEventBlock.defaultEventBlocks.Count; i++)
        {
            DefaultEventBlock.defaultEventBlocks[i].StartEvent();
        }
    }

    public Target CreateNewTargetObject(Target targetPrefab)
    {
        var newTargetObject = Instantiate(targetPrefab);
        EveryTargetInTheScene.Add(newTargetObject);
        RandomizePosition(newTargetObject.gameObject);

        OnNewTargetObjectCreated.Invoke(this, newTargetObject);

        return newTargetObject;
    }

    public void RemoveTargetObject(Target target)
    {
        if (target == null) return;

        EveryTargetInTheScene.Remove(target);
        target.gameObject.SetActive(false);

        CurrentTarget = null;

        OnTargetObjectRemoved.Invoke(this, target);
    }

    public void RandomizePosition(GameObject obj)
    {
        float x = UnityEngine.Random.Range(-4.5f, 3);
        float y = UnityEngine.Random.Range(0.0f, 3);
        float z = UnityEngine.Random.Range(-5f, 10);
        
        obj.transform.position = new Vector3(x, y, z);
    }
}