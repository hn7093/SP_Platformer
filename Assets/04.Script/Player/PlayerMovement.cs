using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public LayerMask climbingLayer;
    public GameObject SpeedEffect;

    [Header("Look")]
    public Transform cameraContainer;
    public float minLook = -85;
    public float maxLook = 85;
    private float CamCurXRot = 0f;
    public float lookSensitivity = 0.1f;
    private Vector2 mouseDelta;
    public bool canLook = true;

    // menber
    public Action indentory;
    private bool tryDash;
    private bool isDash;
    private float speedBuffScale = 1.3f;
    private bool isSpeedBuff;

    private bool canJump = true;
    private int jumpCount = 0;
    private int maxJumpCount = 0; // 공중 점프 가능 횟수
    private bool canClimbing = false;
    private float originMass;

    // components
    private Rigidbody _rigidbody;
    private PlayerStat _playerStat;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStat = GetComponent<PlayerStat>();
        originMass = _rigidbody.mass;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 포인터 숨김
        SpeedEffect?.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuffJump(10f);
        }
    }
    void FixedUpdate()
    {
        CheckGrounded();
        CheckClimbing();
        if (canLook)
        {
            Rotate();
        }
        Move();
        if (transform.position.y < -12)
        {
            ResetPosition();
        }
    }
    void Move()
    {
        Vector3 forward = new Vector3(cameraContainer.forward.x, 0f, cameraContainer.forward.z).normalized;
        Vector3 right = new Vector3(cameraContainer.right.x, 0f, cameraContainer.right.z).normalized;
        if (canClimbing)
        {
            // 벽 오르기
            _rigidbody.useGravity = curMovementInput.y < 0;
            Vector3 vel = _rigidbody.velocity;
            vel.y = 0;
            _rigidbody.velocity = vel;
            Vector3 moveDir = new Vector3(0, curMovementInput.y, curMovementInput.x);
            transform.position += moveDir * (isDash ? sprintSpeed : moveSpeed) * Time.deltaTime;
        }
        else
        {
            _rigidbody.useGravity = true;
            // 입력에 방향에 따라 이동, 속도는 걷기 or 뛰기  * 스피드 버프
            Vector3 moveDir = forward * curMovementInput.y + right * curMovementInput.x;
            transform.position += moveDir
                                * (isDash ? sprintSpeed : moveSpeed)
                                * (isSpeedBuff ? speedBuffScale : 1)
                                * Time.deltaTime;
        }
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
        if (context.phase == InputActionPhase.Started)
        {
            if (canJump && _playerStat.UseStamina(jumpEnergy))
            {
                // 일반 점프
                _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
            else if ((jumpCount < maxJumpCount) && _playerStat.UseStamina(jumpEnergy))
            {
                // 공중 점프
                jumpCount++;
                _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
            else if (canClimbing && _playerStat.UseStamina(jumpEnergy))
            {
                // 벽 점프
                _rigidbody.AddForce((Vector3.up + Vector3.back) * jumpPower, ForceMode.Impulse);
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
    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    bool CheckGrounded()
    {
        // 아래로 레이캐스트
        Ray footRay = new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down);

        // 땅과 가까운지 검사
        if (Physics.Raycast(footRay, 0.05f, groundLayer))
        {
            canJump = true;
            if (_rigidbody.velocity.y == 0.0f)
                jumpCount = 0;
            return true;
        }

        canJump = false;
        return false;
    }
    void ToggleCursor()
    {
        // 마우스 커서 활성/비활성
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void BuffSpeed(float amount)
    {
        speedBuffScale = 1.0f + amount / 100.0f;
        StopCoroutine(BuffMoveSpeed());
        StartCoroutine(BuffMoveSpeed());
    }
    IEnumerator BuffMoveSpeed()
    {
        isSpeedBuff = true;
        SpeedEffect?.SetActive(true);
        yield return new WaitForSeconds(10f);
        SpeedEffect?.SetActive(false);
        isSpeedBuff = false;
    }
    public void BuffJump(float duration)
    {
        StopCoroutine(BuffJumpCount(duration));
        StartCoroutine(BuffJumpCount(duration));
    }
    IEnumerator BuffJumpCount(float duration)
    {
        maxJumpCount = 1;
        yield return new WaitForSeconds(duration);
        maxJumpCount = 0;
    }
    public void BuffJumpForce(float amount)
    {
        jumpPower += amount;
    }

    void CheckClimbing()
    {
        // 공중에 있을 때
        if (!canJump)
        {
            Ray ray = new Ray(transform.position + transform.forward * 0.3f, transform.forward);
            canClimbing = Physics.Raycast(ray, 0.2f, climbingLayer);
        }
        else
        {
            canClimbing = false;
        }
    }
    public void ResetPosition()
    {
        transform.position = new Vector3(3, 0, -3);
    }
}
