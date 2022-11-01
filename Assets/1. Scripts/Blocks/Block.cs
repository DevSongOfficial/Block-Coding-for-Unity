using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class Block : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // 코드블록을 통해 제어할 타겟 오브젝트
    // 모든 블록은 하나의 타겟 오브젝트를 할당받고, 그 타겟 오브젝트를 기반으로 동작함
    // Every block in the scene has it's own target object, and work based on it.
    public Target Target { get; private set; }  
    public virtual void SetTarget(Target target)
    {
        Target = target;
    }

    public static Canvas canvas;
    private static RectTransform canvasRectTransform;

    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    public enum BlockType { LoopBlock, ConditionalBlock, ActionBlock, EventBlock }
    [HideInInspector] public BlockType blockType;

    public static List<Block> AllBlocksInTheScene = new List<Block>();

    [Header("[Do not assign anything in the inspector]", order = 0)]
    [Header("Block Information")]
    public ParentBlock parentBlock;
    public Block topConnectedBlock;
    public Block bottomConnectedBlock;

    // true일 경우 parent블록이 이 블록을 무시하고 다음 블록으로 넘어감
    // Break블록을 통해 Loop블록을 탈출할 때만 사용
    private bool neglected = false; 
    
    protected RectTransform topConnectedBlockArea;     // UI를 이 영역의 좌표 내에 드랍했을 때 위로 붙는다고 인식함
    protected RectTransform bottomConnectedBlockArea;  // UI를 이 영역의 좌표 내에 드랍했을 때 아래로 붙는다고 인식함
    protected RectTransform connectionRecognitionArea; // 이 영역이 위 두 영역과 겹치면 연결 가능하다고 인식함

    private Vector2 previousAnchoredPosition;

    public float HeightDefalut
    {
        get
        {
            switch (blockType)
            {
                case BlockType.ActionBlock: return ActionBlockHeight;
                case BlockType.LoopBlock: return LoopBlockHeight;
                case BlockType.ConditionalBlock: return ConditionalBlockHeight;
                case BlockType.EventBlock: return EventBlockHeight;
            }

            return 0;
        }
    }
    public static readonly float ActionBlockHeight = 42; 
    public static readonly float LoopBlockHeight = 104;
    public static readonly float ConditionalBlockHeight = 104;
    public static readonly float EventBlockHeight = 50;


    // Custom Events
    public static event EventHandler OnDragEnd;

    public static bool isDragging;
    public static Block draggedBlock;

    [HideInInspector] public Outline outline;

    protected virtual void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        AllBlocksInTheScene.Add(this);

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        outline = GetComponent<Outline>();

        topConnectedBlockArea = transform.Find("TopConnectedArea").GetComponent<RectTransform>();
        bottomConnectedBlockArea = transform.Find("BottomConnectedArea").GetComponent<RectTransform>();
        connectionRecognitionArea = transform.Find("BlockRecognitionArea").GetComponent<RectTransform>();

        OnDragEnd += CheckAndTryConnection;
    }

    // Every block must be initialized by this function.
    // Notice: This fucntion have to be called after Awake().
    public static void InitializeAllBlocksInTheScene()
    {
        for (int i = 0; i < AllBlocksInTheScene.Count; i++)
        {
            var block = AllBlocksInTheScene[i];
            
            // [1] 블록 Neglect 초기화
            block.SetNeglect(false);
        }
    }

    protected virtual void Update()
    {

    }

    // This function will be called when dragged block dropped on the top-connected-area of this block.
    public void TopConnect(Block block)
    {
        if (HasParentBlock() && block.IsEventBlock()) return;

        // 마우스 커서가 블록의 움직임보다 더 빨라졌을 때 자식 블록에 Connected처리됨을 방지
        if (block.IsParentBlock() && block.AsParentBlock().Contains(this)) return;

        if (IsTopConnected())
        {
            topConnectedBlock.SetBottomConnectedBlock(block);
        }
        
        SetTopConnectedBlock(block);

        block.SetBottomConnectedBlock(this);

        if(HasParentBlock())
        {
            parentBlock.AddChildBlock(block, GetIndexFromParent());

            float yOffSet = (GetBlockHeight() - block.GetBlockHeight()) * 0.5f;
            var newPosition = new Vector2(GetAnchoredPositionX(), GetAnchoredPositionY() + yOffSet);
            block.SetAnchoredPosition(newPosition); 
            
            PushDownBy(block);
        }
        else
        {
            float yOffSet = (GetBlockHeight() + block.GetBlockHeight()) * 0.5f;
            var newPosition = new Vector2(GetAnchoredPositionX(), GetAnchoredPositionY() + yOffSet);
            block.SetAnchoredPosition(newPosition);
        }

        block.AsParentBlock()?.MoveChildBlocks(block.DeltaPosition());

        block.AsEventBlock()?.AddChildBlock(this, 0, true);
    }


    // This function will be called when dragged block dropped on the bottom-connected-area of this block.
    public virtual void BottomConnect(Block block)
    {
        // 마우스 커서가 블록의 움직임보다 더 빨라졌을 때 자식 블록에 Connected처리됨을 방지
        if (block.IsParentBlock() && block.AsParentBlock().Contains(this))
        {
            return;
        }

        if (IsBottomConnected())
        {
            bottomConnectedBlock.SetTopConnectedBlock(block);
            block.SetBottomConnectedBlock(bottomConnectedBlock);
            
            float yOffSet = (GetBlockHeight() + block.GetBlockHeight()) * 0.5f;
            Vector2 newPosition = new Vector2(GetAnchoredPositionX(), GetAnchoredPositionY() - yOffSet);
            block.SetAnchoredPosition(newPosition);

            block.bottomConnectedBlock.PushDownBy(block);
        }
        else
        {
            float yOffSet = (GetBlockHeight() + block.GetBlockHeight()) * 0.5f;
            Vector2 newPosition = new Vector2(GetAnchoredPositionX(), GetAnchoredPositionY() - yOffSet);
            block.SetAnchoredPosition(newPosition);
        }

        SetBottomConnectedBlock(block);

        block.SetTopConnectedBlock(this);

        if(HasParentBlock())
        {
            parentBlock.AddChildBlock(block, GetIndexFromParent() + 1);
        }

        block.AsParentBlock()?.MoveChildBlocks(block.DeltaPosition());
    }

    // This function will be called when you start dragging a block except event block.
    public void Disconnect()
    {
        if(IsBottomConnected())
        {
            bottomConnectedBlock.SetTopConnectedBlock(topConnectedBlock);
            if(bottomConnectedBlock.IsTopConnected())
            {
                bottomConnectedBlock?.PushUp(GetBlockHeight());
            }
        }

        if(IsTopConnected())
        {
            topConnectedBlock.SetBottomConnectedBlock(bottomConnectedBlock);
        }
        else
        {
            if (HasParentBlock())
            {
                bottomConnectedBlock?.PushUp(GetBlockHeight());
            }
        }

        bottomConnectedBlock = null;
        topConnectedBlock = null;

        parentBlock?.RemoveChildBlock(this);
    }

    // Check if draggedBlock dropped on one of the connection areas of this block.
    public void CheckAndTryConnection(object sender, EventArgs eventArgs)
    {
        if (CanConnectToDraggedBlock() == false) return;

        if (DraggedBlockOnTheArea(topConnectedBlockArea))
        {
            TopConnect(draggedBlock);
        }
        else if (DraggedBlockOnTheArea(bottomConnectedBlockArea))
        {
            BottomConnect(draggedBlock);
        }
        else 
        {
            switch (blockType) // Check middle connection.
            {
                case BlockType.LoopBlock:
                    AsParentBlock().TryMiddleConnect(draggedBlock);
                    break;
                case BlockType.ConditionalBlock:
                    AsParentBlock().TryMiddleConnect(draggedBlock);
                    break;
            }
        }
    }

    // Check if draggedBlock is hovering over one of the connection areas of this block.
    public bool CanConnectToDraggedBlock()
    {
        if (draggedBlock == null) return false;

        if (draggedBlock == this) return false;

        if (draggedBlock.IsTopConnected() || draggedBlock.IsBottomConnected()) return false;

        // 서로 다른 패널의 블록들이 Connect됨을 방지
        if (OnTheBlockPanel()) return false;

        // draggedBlock이 자식 block과 Connect됨을 방지
        if (draggedBlock.IsParentBlock() || draggedBlock.IsEventBlock())
        {
            if (draggedBlock.AsParentBlock().Contains(this)) return false;
        }

        // 서로 다른 MainPanel의 블록끼리의 Connect됨을 방지
        if (Target.connectedMainPanel.gameObject.activeSelf == false) return false;

        if (DraggedBlockOnTheArea(topConnectedBlockArea) && IsTopConnected() == false)
        {
            if (IsEventBlock()) return false;

            return true;
        }
        else if (DraggedBlockOnTheArea(bottomConnectedBlockArea))
        {
            if (draggedBlock.IsEventBlock()) return false;

            return true;
        }
        else
        {
            if (draggedBlock.IsEventBlock()) return false;
            if (IsParentBlock() == false) return false;
            if (DraggedBlockOnTheArea(AsParentBlock().middleConnectedBlockArea) == false) return false;
            if (AsParentBlock().HasChildBlock()) return false;  // child가 하나도 없는 상태이면 MiddleConnect, 그렇지 않으면 그 child 위에 붙인 후 함께 확장
            switch (blockType) 
            {
                case BlockType.LoopBlock:
                    return true;                                                     
                case BlockType.ConditionalBlock:
                    return true;
            }
        }

        return false;
    }

    // Notice: PushDownBy 함수는 block을 먼저 배치 한 후에 실행시켜야함 
    public void PushDownBy(Block block, bool isRecursive = true) // block에 의해서 한칸씩 아래로 밀려짐
    {
        float yOffSet = (GetBlockHeight() + block.GetBlockHeight()) * 0.5f;
        Vector2 newPosition = new Vector2(block.GetAnchoredPositionX(), block.GetAnchoredPositionY() - yOffSet);
        SetAnchoredPosition(newPosition);

        AsParentBlock()?.MoveChildBlocks(DeltaPosition());

        if (isRecursive && IsBottomConnected())
        {
            bottomConnectedBlock.PushDownBy(this);
        }
    }

    // 이 블록과 그 아래의 블록들의 y좌표를 amount만큼 줄임
    public void PushDown(float amount, bool isRecursive = true) 
    {
        ChangeAnchoredPositionY(-amount);

        AsParentBlock()?.MoveChildBlocks(DeltaPosition());

        if (isRecursive && IsBottomConnected())
        {
            bottomConnectedBlock.PushDown(amount);
        }
    }

    // Notice: PushUpTo 함수는 block의 위치까지 미는 것임, 즉 Disconnect되고 움직이기 전에 바로 실행되어야 함
    public void PushUpTo(Block block, bool isRecursive = true)
    {
        if (isRecursive && IsBottomConnected())
        {
            bottomConnectedBlock.PushUpTo(block);
        }

        ChangeAnchoredPositionY(block.HeightDefalut);

        AsParentBlock()?.MoveChildBlocks(DeltaPosition());
    }

    public void PushUp(float amount, bool isRecursive = true)
    {
        ChangeAnchoredPositionY(amount);

        AsParentBlock()?.MoveChildBlocks(DeltaPosition());

        if (isRecursive && IsBottomConnected())
        {
            bottomConnectedBlock.PushUp(amount);
        }
    }

    protected IEnumerator MoveOnToNextBlock(Block block)
    {
        if (GameManager.InProgress == false) yield break;
        if (block.IsNeglected()) yield break;

        switch (block.blockType)
        {
            case BlockType.ActionBlock:
                yield return block.AsActionBlock().DoFunction();
                break;

            case BlockType.LoopBlock:
                //yield return block.AsLoopBlock().StartCoroutine(block.AsLoopBlock().LoopCoroutine); 아래와 똑같은 코드
                yield return block.AsLoopBlock().StartLoopFunctions(); // LoopBlock을 제외한 다른 블록은 반복을 제어할 필요가 없으므로 그냥 StartCoroutine으로 처리함
                break;

            case BlockType.ConditionalBlock:
                yield return StartCoroutine(block.AsConditionalBlock().ConditionalFunctionsRoutine());
                break;
        }
    }

    public void SetBottomConnectedBlock(Block block)
    {
        bottomConnectedBlock = block;
    }

    public void SetTopConnectedBlock(Block block)
    {
        topConnectedBlock = block;
    }

    public bool IsBottomConnected()
    {
        return bottomConnectedBlock != null;
    }

    public bool IsTopConnected()
    {
        return topConnectedBlock != null;
    }

    public bool HasParentBlock()
    {
        return parentBlock != null;
    }

    public int GetIndexFromParent() // 부모로부터 childBlocks내에 자신의 index를 가져옴
    {
        return parentBlock.childBlocks.IndexOf(this);
    }

    public int GetIndexFromParent(ParentBlock parentBlock)
    {
        return parentBlock.childBlocks.IndexOf(this);
    }

    public Vector2 GetAnchoredPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public float GetAnchoredPositionX()
    {
        return GetAnchoredPosition().x;
    }

    public float GetAnchoredPositionY()
    {
        return GetAnchoredPosition().y;
    }

    public void SetAnchoredPosition(Vector2 position)
    {
        previousAnchoredPosition = GetAnchoredPosition();

        rectTransform.anchoredPosition = position;
    }

    public void ChangeAnchoredPosition(Vector2 delta)
    {
        previousAnchoredPosition = GetAnchoredPosition();

        rectTransform.anchoredPosition += delta;
    }

    public void ChangeAnchoredPositionX(float deltaX)
    {
        ChangeAnchoredPosition(new Vector2(deltaX, 0));
    }

    public void ChangeAnchoredPositionY(float deltaY)
    {
        ChangeAnchoredPosition(new Vector2(0, deltaY));
    }

    public Vector2 DeltaPosition()
    {
        return GetAnchoredPosition() - previousAnchoredPosition;
    }

    public void SetRectTransformWidth(float width)
    {
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }

    public void SetRectTransformHeight(float height)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    public float GetRectTransformHeight()
    {
        return rectTransform.sizeDelta.y;
    }

    public float GetBlockHeight()
    {
        return IsParentBlock() ? AsParentBlock().Height : HeightDefalut;
    }

    public void SetNeglect(bool neglect, bool applyToChildBlocks = false)
    {
        neglected = neglect;

        if (IsParentBlock() && applyToChildBlocks)
        {
            for (int i = 0; i < AsParentBlock().childBlocks.Count; i++)
            {
                var block = AsParentBlock().childBlocks[i];
                block.SetNeglect(neglect, applyToChildBlocks);
            }
        }
    }

    public bool IsNeglected()
    {
        return neglected;
    }

    public void SortInHierarchy()
    {
        transform.SetAsFirstSibling();

        if (HasParentBlock())
        {
            parentBlock.SortInHierarchy();
        }
    }

    // TODO: Transform.position으로 수정
    public static bool MousePointOnTheArea(RectTransform area, bool externalCall = false)
    {
        Vector2 mousePosition = Input.mousePosition + (externalCall ? Vector2.zero : PanelManager.Instance.LogicPanelContentPositionDelta);
        Vector2 anchorPosition = mousePosition - new Vector2(canvasRectTransform.position.x, canvasRectTransform.position.y);
        anchorPosition = new Vector2(anchorPosition.x / canvasRectTransform.lossyScale.x, anchorPosition.y / canvasRectTransform.lossyScale.y);
        Vector2 areaAnchoredPosition = area.anchoredPosition + (externalCall ? Vector2.zero : area.parent.GetComponent<RectTransform>().anchoredPosition);

        if (anchorPosition.x >= areaAnchoredPosition.x - area.rect.width * 0.5f &&
            anchorPosition.x <= areaAnchoredPosition.x + area.rect.width * 0.5f &&
            anchorPosition.y >= areaAnchoredPosition.y - area.rect.height * 0.5f &&
            anchorPosition.y <= areaAnchoredPosition.y + area.rect.height * 0.5f)
        {
            return true;
        }

        return false;
    }
    
    public bool DraggedBlockOnTheArea(RectTransform area)
    {
        if (area == null) return false;

        var draggedPosition = draggedBlock.connectionRecognitionArea;

        if (draggedPosition.position.x >= area.position.x - area.rect.width  * 0.5f &&
            draggedPosition.position.x <= area.position.x + area.rect.width * 0.5f &&
            draggedPosition.position.y >= area.position.y - area.rect.height * 0.5f &&
            draggedPosition.position.y <= area.position.y + area.rect.height * 0.5f)
        {
            return true;
        }

        return false;
    }

    public bool IsParentBlock()
    {
        switch (blockType)
        {
            case BlockType.LoopBlock:
            case BlockType.ConditionalBlock:
                return true;
        }

        return false;
    }

    public bool OnTheBlockPanel()
    {
        return transform.parent == PanelManager.Current.BlockPanelContent;
    }

    public bool OnTheLogicPanel()
    {
        return transform.parent.parent == PanelManager.Current.LogicPanelContent;
    }

    public bool IsEventBlock()
    {
        return blockType == BlockType.EventBlock;
    }

    public bool IsLoopBlock()
    {
        return blockType == BlockType.LoopBlock;
    }

    public virtual ParentBlock AsParentBlock()
    {
        return null;
    }


    public virtual EventBlock AsEventBlock()
    {
        return null;
    }


    public virtual LoopBlock AsLoopBlock()
    {
        return null;
    }

    public virtual ConditionalBlock AsConditionalBlock()
    {
        return null;
    }

    public virtual ActionBlock AsActionBlock()
    {
        return null;
    }


    // 부모 블록 중 가장 가까운 LoopBlock을 찾아서 반환
    // param[1]: 가장 가까운 LoopBlock
    // param[2]: LoopBlock과 가장 가까운 LoopBlock의 자식 (자기 자신 혹은 자기 자신의 부모 중 하나임)
    public void FindNearestLoopBlock(out LoopBlock block, out Block childOfLoopBlock)
    {
        if(HasParentBlock() == false || parentBlock.IsEventBlock())
        {
            block = null;
            childOfLoopBlock = null;
            return;
        }

        if (parentBlock.IsLoopBlock())
        {
            block =  parentBlock.AsLoopBlock();
            childOfLoopBlock = this.AsParentBlock();
            return;
        }
        else
        {
            parentBlock.FindNearestLoopBlock(out block, out childOfLoopBlock);
        }
    }

    // param: EventBlock을 제외한 가장 높은 부모 Block
    public void FindHighestParentBlock(out ParentBlock block)
    {
        if(HasParentBlock())
        {
            parentBlock.FindHighestParentBlock(out block) ;
        }
        else
        {
            if (IsParentBlock())
            {
                block = this.AsParentBlock();
            }
            else
            {
                block = null;
            }
        }
    }

    public void DestroyBlock()
    {
        if (IsParentBlock())
        {
            for(int i = AsParentBlock().childBlocks.Count - 1; i >= 0 ; i--)
            {
                AsParentBlock().childBlocks[i].DestroyBlock();
            }
        }
        
        if (IsBottomConnected())
        {
            bottomConnectedBlock.DestroyBlock();
        }

        AsEventBlock()?.OnDestroyed();
        gameObject.SetActive(false);
    }
    
    public void HighlighteOn(Color color) 
    {
        canvasGroup.alpha = 1;
        outline.effectColor = new Color(0, 0, 0, 0.5f);
        outline.effectDistance = new Vector2(0.9f, 0.9f);
        //outline.effectColor = color;
        //outline.effectDistance = new Vector2(1.6f, 1.6f);
    }

    public void HighlightOff()
    {
        canvasGroup.alpha = 0.6f;
        outline.effectColor = new Color(0, 0, 0, 0.5f);
        outline.effectDistance = new Vector2(0.9f, 0.9f);
    }

    // LeftControl키를 누른 채로 드래그 하면 
    // 이 함수를 통해 아래 연결된 모든 블록이 함께 이동
    // 이와 관련해 Disconnect되는 것과
    // 한꺼번에 Connect하는 건 나중에 구현바람 ! 
    // 
    public void MoveAllUnderConnectedBlocks(Vector2 delta)
    {
        if(bottomConnectedBlock != null)
        {
            bottomConnectedBlock.ChangeAnchoredPosition(delta / canvas.scaleFactor);

            if (bottomConnectedBlock.IsParentBlock())
            {
                bottomConnectedBlock.AsParentBlock().MoveChildBlocks(delta / canvas.scaleFactor);
            }

            bottomConnectedBlock.MoveAllUnderConnectedBlocks(delta);
        }
    }

    // 자신을 복사하여 생성
    public void CreateNewBlockOnBlockPanel()
    {
        var newBlock = Instantiate(this, PanelManager.Current.BlockPanelContent);
        newBlock.SetTarget(Target);

        AsEventBlock()?.OnCreated();
    }

    #region Drag & Drop
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // BlockPanel의 블록을 드래그 시 복사 생성
        if (OnTheBlockPanel())
        {
            CreateNewBlockOnBlockPanel();
        }

        transform.SetParent(PanelManager.Instance.transform);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        if(IsEventBlock() == false) Disconnect();

        isDragging = true;
        draggedBlock = this;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        ChangeAnchoredPosition(eventData.delta / canvas.scaleFactor);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (IsParentBlock() || IsEventBlock()) transform.SetParent(PanelManager.Current.ParentBlocksPanel);
        else transform.SetParent(PanelManager.Current.SimpleBlocksPanel);

        HighlightOff();

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        OnDragEnd.Invoke(this, EventArgs.Empty);

        isDragging = false;
        draggedBlock = null;

    }
    #endregion
}