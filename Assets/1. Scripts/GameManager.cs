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

    public static bool InProgress { get; private set; } // Return if the game has started.

    // Custom Events
    public event EventHandler<Target> OnTargetObjectSelected;
    public event EventHandler<Target> OnNewTargetObjectCreated;
    public event EventHandler<Target> OnTargetObjectRemoved;
    public event EventHandler OnGameStart;

    public static Target CurrentTarget { get; private set; } // TargetObject currently being selected.
    public static Target PreviousTarget { get; private set; }
    public void SetCurrentTarget(Target targetObject)
    {
        PreviousTarget = CurrentTarget;

        if (PreviousTarget != null)
        {
            PreviousTarget.RemoveOutline();
        }

        CurrentTarget = targetObject;
        CurrentTarget.GetComponent<ISelectable>().GetSelected();

        targetObject.AddOutline();

        OnTargetObjectSelected?.Invoke(this, CurrentTarget);
    }

    public List<Target> EveryTargetInTheScene { get; private set; }

    // Target Object Prefabs
    [HideInInspector] public Target cubePrefab;     // [1] Cube
    [HideInInspector] public Target spherePrefab;   // [2] Sphere
    [HideInInspector] public Target cylinderPrefab; // [3] Cylinder
    
    [HideInInspector] public Target gameCamera;

    [SerializeField] private GameObject raycastBlock;

    public static Light DirectionalLight { get; private set; }

    private void Awake()
    {
        instance = this;

        EveryTargetInTheScene = new List<Target>();

        // Load Target Object Prefabs
        cubePrefab = Resources.Load<Target>("Prefabs/Target_Cube");
        spherePrefab = Resources.Load<Target>("Prefabs/Target_Sphere");
        cylinderPrefab = Resources.Load<Target>("Prefabs/Target_Cylinder");
        gameCamera = GameObject.Find("Game Camera").GetComponent<Target>();

        DirectionalLight = GameObject.Find("Directional Light").GetComponent<Light>();

        Screen.SetResolution(1280, 720, false);
    }

    private void Start()
    {
        Block.InitializeAllBlocksInTheScene();

        SetGameCamera();
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
    // By implementing every DefaultEventBlock's event
    [ContextMenu("Start The Game")]
    public void GameStart()
    {
        InProgress = true;

        OnGameStart.Invoke(this, EventArgs.Empty);

        raycastBlock.SetActive(true);

        foreach (Target target in EveryTargetInTheScene)
        {
            target.UpdateTransformBeforEventStarted();
            target.rigidBody.isKinematic = false;
        }

        PanelManager.Instance.ActivateAllBlockPanels();

        for (int i = 0; i < DefaultEventBlock.defaultEventBlocks.Count; i++)
        {
            DefaultEventBlock.defaultEventBlocks[i].StartEvent();
        }
    }

    [ContextMenu("Stop The Game")]
    // Stop the game and initialize the targets in the scene
    public void GameStop()
    {
        InProgress = false; // Loop Block and Wait Block would be stopped by the variable.

        raycastBlock.SetActive(false);

        foreach (Target target in EveryTargetInTheScene)
        {
            target.RevertToTrasnformBeforeEventStarted();
            target.rigidBody.isKinematic = true;
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

    private void SetGameCamera()
    {
        EveryTargetInTheScene.Add(gameCamera);
        OnNewTargetObjectCreated.Invoke(this, gameCamera);
    }

    public void RemoveTargetObject(Target target)
    {
        if (target.gameObject.name == "Game Camera") return;
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