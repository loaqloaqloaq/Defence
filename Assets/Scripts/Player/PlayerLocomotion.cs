﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    //入力
    private PlayerInput playerInput;

    //コンポネント
    private Animator animator;
    private CharacterController charController;
    private ActiveWeapon activeWeapon;
    private ReloadWeapon reloadWeapon;
    private PlayerAiming characterAiming;
    //入力ベクタ

    //移動
    private Vector3 rootMotion;
    private Vector3 velocity;
    private bool isJumping;

    [SerializeField] float groundSpeed; //地面上のスピード
    [Range(1.0f, 2.0f)][SerializeField] float speedModifier;
    [SerializeField] float jumpHeight; //3 ジャンプ力
    [SerializeField] float gravity; //15~20 重力
    [SerializeField] float stepDown; //0.3 階段 Step Amount
    [SerializeField] float airControl; //2.5 空中左右移動速度パラメータ x * aircontrol / 100 
    [SerializeField] float jumpDamp; //0.5
    [SerializeField] float pushPower; // 2

    private int isSprintingParam = Animator.StringToHash("IsSprinting");
    [SerializeField] Animator rigController; //アニメーションリギングコントローラ

    //スタミナ
    [SerializeField] private float recoveryDuration = 3.0f;
    [SerializeField] private float sprintDuration = 20.0f;

    private const float maxStamina = 100.0f;

    private float currentStamina;
    private float lastSprintTime;

    private Vector2 input;

    //コンポネント取得
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterAiming = GetComponent<PlayerAiming>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        input = playerInput.input;
        if (!playerInput || !playerInput.enabled)
        {
            SetMoveAnimation(0, 0);
            return;
        }

        if (playerInput.Jump) { Jump(); }

        if (input.sqrMagnitude > 1.0f) { input.Normalize(); }

        SetMoveAnimation(input.x, input.y);
        UpdateIsSprinting();

        UpdateUI();
    }


    private void FixedUpdate()
    {
        if (!playerInput || !playerInput.enabled) input = Vector2.zero;

        if (isJumping)
        { //  Is In Air State
            UpdateInAir();
        }
        else
        { // IsGrounded State
            UpdateOnGround();
        }
    }

    private void SetMoveAnimation(float x, float y)
    {
        animator.SetFloat("InputX", x);
        animator.SetFloat("InputY", y);
    }

    //走る状態か確認
    bool IsSprinting()
    {
        if (input.x < -0.5f || input.x > 0.5f) { return false; } 
        if (input.y < 0.1f || currentStamina <= 0) { return false; }

        bool notThrowing = rigController.GetCurrentAnimatorStateInfo(3).shortNameHash
        == Animator.StringToHash("notThrowing");
        bool isSprinting = playerInput.Sprint;
        bool isFiring = activeWeapon.IsFiring();
        bool isReloading = reloadWeapon.isReloading;
        bool isChangingWeapon = activeWeapon.isChangingWeapon;
        bool isAiming = characterAiming.isAiming;

        return isSprinting && !isFiring && !isReloading && !isChangingWeapon && !isAiming && notThrowing;
    }

    //走る処理
    private void UpdateIsSprinting()
    {
        bool isSprinting = IsSprinting();

        animator.SetBool(isSprintingParam, isSprinting);
        rigController.SetBool(isSprintingParam, isSprinting);

        if (isSprinting)
        {
            float reduceRate = maxStamina / sprintDuration;
            currentStamina -= reduceRate * Time.deltaTime;
            lastSprintTime = Time.time;
        }
        else if (currentStamina < maxStamina && Time.time > lastSprintTime + 1.5f)
        {
            float recoverRate = maxStamina / recoveryDuration;
            currentStamina += recoverRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    
    private void UpdateOnGround()
    {
        rootMotion = transform.right * input.x + transform.forward * input.y;
        var targetSpeed = groundSpeed;
        bool isSprinting = IsSprinting();
        if (isSprinting) targetSpeed *= speedModifier;

        Vector3 stepForwardAmount = rootMotion * targetSpeed * Time.fixedDeltaTime;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        charController.Move(stepDownAmount + stepForwardAmount * targetSpeed * Time.deltaTime);

        if (!charController.isGrounded)
        {
            //Debug.Log("In Air State");
            SetInAir(0);
        }
    }
    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        charController.Move(displacement);
        isJumping = !charController.isGrounded;
        rootMotion = Vector3.zero;

        animator.SetBool("IsJumping", isJumping); 
    }

    void Jump()
    {
        if (!isJumping)
        {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity);
        }
    }
    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = charController.velocity;
        velocity.y = jumpVelocity;

        animator.SetBool("IsJumping", true);
    }


    private Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    private void UpdateUI()
    {
        bool isActive = IsSprinting() || currentStamina < maxStamina;
        UIManager.Instance?.SetActiveStamina(isActive);
        UIManager.Instance?.UpdateStaminaGauge(maxStamina, currentStamina);
    }
}
