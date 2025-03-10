using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 레이캐스트로 애니메이션 실행하는 함정
[RequireComponent(typeof(Animator))]
public class RaycastTrap : MonoBehaviour
{
    [SerializeField] private Transform rayPoint;
    public LayerMask targetLayer;
    public float searchRange;
    public float lastCheckTime;
    public float interval = 1f;
    private int _animAction = Animator.StringToHash("Action");
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        Ray targetRay = new Ray(rayPoint.position, rayPoint.forward);
        // 오브젝트 검사
        if (Physics.Raycast(targetRay, searchRange, targetLayer))
        {
            if (lastCheckTime + interval <= Time.time)
            {
                lastCheckTime = Time.time;
                _animator.SetTrigger(_animAction);
            }

        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(rayPoint.position, rayPoint.position + Vector3.forward * searchRange);
    }
}
