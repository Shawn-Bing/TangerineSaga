using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb; //get component

    [Header("人物移动")]
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;
    private bool canPlayerMove;//是否允许玩家移动
    [Header("人物动画")]
    private bool isMoving;

    [Header("人物工具动画")]
    private float toolMouseX;
    private float toolMouseY;
    private bool useTool;

    private Animator[] animators;//创建数组获取Player身上全部Animator

    
    /// <summary>
    /// 切换人物动画
    /// </summary>
    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("toolMouseX", toolMouseX);
            anim.SetFloat("toolMouseY", toolMouseY);

            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    /// <summary>
    /// detect Player's input then combine it as a Vector
    /// </summary>
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        // Limit side direction speed
        if(inputX != 0 && inputY!=0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        // 按下Shift切换为走路状态
        if(Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }

        movementInput = new Vector2(inputX, inputY);
        
        isMoving = movementInput != Vector2.zero;//判断玩家是否在移动
    }
    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }
    private void Update() {
        if(!canPlayerMove)
        {
            isMoving = false;
            
        }else
        {
            PlayerInput();
        }
        SwitchAnimation();
    }
    private void FixedUpdate() {
        if(canPlayerMove)
        {
            Movement();
        }   
    }

    #region 注册玩家坐标移动、鼠标点击事件

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }


    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        canPlayerMove = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        canPlayerMove = true;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    /// <summary>
    /// //点击物品时通知Player切换到对应动画
    /// </summary>
    /// <param name="pos">鼠标所指的世界坐标</param>
    /// <param name="itemDetails">选中物品详情</param>
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        if (useTool)
            return;

        //TODO:执行动画
        // 若所选物品类型为工具（非种子、商品、家具）
        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            toolMouseX = mouseWorldPos.x - transform.position.x;
            toolMouseY = mouseWorldPos.y - (transform.position.y + Settings.playerAnimaitonHeightOffset);

            // 将斜向改为左右
            if (Mathf.Abs(toolMouseX) > Mathf.Abs(toolMouseY))
                toolMouseY = 0;
            else
                toolMouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
        // 在播放动画之后再执行，否则刚播放砍树动画，树就倒下了
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        canPlayerMove = false;
        yield return null;
        // 确保执行完毕

        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            //将使用工具时人物的面朝方向链接到人物本来的运动方向
            anim.SetFloat("InputX", toolMouseX);
            anim.SetFloat("InputY", toolMouseY);
        }
        //等待时间然后执行
        //FIXME:根据动画时间设定等待时间的值
        yield return new WaitForSeconds(Settings.playerBeforeToolAnimaitonWaitTime);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(Settings.playerAfterToolAnimaitonWaitTime);
        
        // 等待动画结束
        useTool = false;
        canPlayerMove = true;
    }

    #endregion
}
