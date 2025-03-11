using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}
public class Enemy : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health = 30;
    public float walkSpeed = 1.3f;
    public float runSpeed = 3f;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance = 20f;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance = 2f;
    public float maxWanderDistance = 5f;
    public float minWanderWaitTime = 1;
    public float maxWanderWaitTime = 5;

    [Header("Combat")]
    public int damage = 10;
    public float attackRate = 1.5f;
    private float lastAttackTime;
    public float attackDistance = 2f;

    private float playerDistance;

    public float fieldOfView = 120f;  // 시야각

    private SkinnedMeshRenderer[] meshRenderers;
    // Start is called before the first frame upd ate
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }
    public void SetState(AIState state)
    {
        aiState = state;
        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }
    }

    void PassiveUpdate()
    {
        // 목표까지 남은 거리가 0.1보다 작으면
        if (aiState == AIState.Wandering && agent.remainingDistance <= 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        // 플레이어로 타겟 변경
        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    // 랜덤 목표지점 설정
    Vector3 GetWanderLocation()
    {
        NavMeshHit hit; // 타겟의 최단 거리
        // 가상의 구를 만들어서 랜덤
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
        int i = 0;
        // 너무 가까우면 다시 찾기
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break; // 최대 시도 횟수
        }
        return hit.position;

    }
    void AttackingUpdate()
    {
        // 거리 및 시야 각 검사
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.stat.TakeDamage(damage);
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                // 경로가 존재하는지 확인
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                // 공격 중에 플레이어가 멀어진다면 임의 지점
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
        StartCoroutine(DamageFlash());
    }
    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        // 붉게
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }
        yield return new WaitForSeconds(0.3f); //지속시간
        // 원래대로
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }
    private void OnDrawGizmos()
    {
        // 원의 색상 설정
        Gizmos.color = Color.green;
        // 원 중심 (현재 오브젝트 위치)에서 반지름이 playerDistance인 원을 그림
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}
