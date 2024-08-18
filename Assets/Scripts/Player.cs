using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public event EventHandler OnPickedSometing;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10.0f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteracterAction += GameInput_OnInteracterAction;
        gameInput.OnInteracterAlternate += GameInput_OnInteracterAlternate;
    }

    private void GameInput_OnInteracterAlternate(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.InteracterAlternate(this);
        }
    }

    //按键触发交互
    private void GameInput_OnInteracterAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        //获取输入值
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        //检测counter
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, layerMask))
        {
            //尝试获取组件
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectCounter(baseCounter);
                }
            }
            else
            {
                SetSelectCounter(null);
            }
        }
        else
        {
            SetSelectCounter(null);
        }
    }

    private void HandleMovement()
    {
        //获取输入值
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        //角色碰撞物体后对角线移动
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        //角色朝向
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);

        //走路动画
        isWalking = moveDir != Vector3.zero;
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + transform.up * 2.0f, transform.forward * 2f);
    }

    private void SetSelectCounter(BaseCounter baseCounter)
    {
        this.selectedCounter = baseCounter;
        //事件通知订阅者
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs() { selectedCounter = selectedCounter });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSometing?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}