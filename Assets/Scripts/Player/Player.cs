using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb; //get component

    [Header("基本数据")]
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    //detect Player's input then combine it as a Vector
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //Limit side direction speed
        if(inputX != 0 && inputY!=0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }
        movementInput = new Vector2(inputX, inputY);
    }
    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        PlayerInput();
    }
    private void FixedUpdate() {
        Movement();
    }
}
