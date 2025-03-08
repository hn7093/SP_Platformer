using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 7f;
    public float sprintEnergy = 10f;
    public float jumpPower = 5f;
    public float jumpEnergy = 10f;
    private Vector2 curMovementInput;
    public LayerMask groundLayer;

    [Header("Look")]
    public Transform cameraContainer;
    public float minLook = -85;
    public float maxLook = 85;
    private float CamCurXRot = 0f;
    public float lookSensitivity = 0.1f;
    private Vector2 mouseDelta;

    public bool canLook = true;
    public Action indentory;
    private bool tryDash;
    private bool isDash;
    // components
    private Rigidbody _rigidbody;
    private PlayerStat _playerStat;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStat = GetComponent<PlayerStat>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 포인터 숨김
    }
    void Update()
    {
        // 달릴 수 있다면 스테미너 소모
        if (tryDash)
        {
            // 달리기
            isDash = _playerStat.UseStamina(Time.deltaTime * sprintEnergy);
        }
        else
        {
            isDash = false;
        }
    }
    void FixedUpdate()
    {

        if (canLook)
        {
            Rotate();
        }
        Move();
    }

    void Move()
    {
        Vector3 forward = new Vector3(cameraContainer.forward.x, 0f, cameraContainer.forward.z).normalized;
        Vector3 right = new Vector3(cameraContainer.right.x, 0f, cameraContainer.right.z).normalized;
        Vector3 moveDir = forward * curMovementInput.y + right * curMovementInput.x;
        //transform.forward = moveDir;
        transform.position += moveDir * (isDash ? sprintSpeed : moveSpeed) * Time.deltaTime;
        Debug.Log(isDash);
        // 달리고 있는중이라면 자연 회복 중지
        if (isDash)
        {
            _playerStat.ActiveStemina(false);
        }
    }
    void Rotate()
    {
        // 좌우
        CamCurXRot += mouseDelta.y * lookSensitivity;
        CamCurXRot = Mathf.Clamp(CamCurXRot, minLook, maxLook);
        cameraContainer.localEulerAngles = new Vector3(-CamCurXRot, 0f, 0f);

        // 상하
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            if (_playerStat.UseStamina(jumpEnergy))
            {
                _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            indentory?.Invoke();
            ToggleCursor();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            tryDash = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            tryDash = false;
            _playerStat.ActiveStemina(true);
        }
    }
    bool IsGrounded()
    {
        // 아래로 레이캐스트
        Ray footRay = new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down);

        // 땅과 가까운지 검사
        if (Physics.Raycast(footRay, 0.1f, groundLayer))
        {
            return true;
        }

        return false;
    }
    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
