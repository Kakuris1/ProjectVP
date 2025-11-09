using Combat.Skills; 
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyInformation))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyInformation enemyInfo;
    private EnemyController enemyController;
    private EnemySensorSight sensor;

    [Header("Patrol (순찰) 상태 설정")]
    [SerializeField] private float patrolWanderRadius = 5.0f; // 순찰 반경
    [SerializeField] private float patrolWanderInterval = 4.0f; // 순찰 주기 (초)
    private float patrolWanderTimer;
    private Vector3 patrolOrigin; // 순찰 기준점 (시작 위치)

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyInfo = GetComponent<EnemyInformation>();
        enemyController = GetComponent<EnemyController>();
        sensor = GetComponent<EnemySensorSight>();

        // 순찰 기준점을 이 유닛이 처음 스폰된 위치로 기억합니다.
        patrolOrigin = transform.position;
    }

    private void Update()
    {
        if (!agent.enabled)
        {
            return;
        }
        // EnemyInformation에 기록된 현재 상태를 읽어와서 그에 맞는 이동 로직을 실행
        switch (enemyInfo.CurrentState)
        {
            case EnemyUnitState.Patrol:
                HandlePatrolMovement();
                break;

            case EnemyUnitState.StopAndWatching:
                HandleStopAndWatchingMovement();
                break;

            case EnemyUnitState.MovingToCommand:
                HandleMovingToCommandMovement();
                break;

            case EnemyUnitState.Engaging:
                HandleEngagingMovement();
                break;

            case EnemyUnitState.Dead:
                HandleDead();
                break;
        }
    }

    // (순찰) 상태: patrolOrigin을 기준으로 무작위 배회 
    private void HandlePatrolMovement()
    {
        patrolWanderTimer -= Time.deltaTime;

        // 배회할 시간이 되었다면
        if (patrolWanderTimer <= 0f)
        {
            // 1. 기준점(patrolOrigin) 5m 반경 내의 무작위 지점 설정
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * patrolWanderRadius;
            Vector3 randomTargetPos = patrolOrigin + new Vector3(randomCircle.x, 0, randomCircle.y);

            // 2. 해당 지점이 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomTargetPos, out hit, patrolWanderRadius, NavMesh.AllAreas))
            {
                // 3. NavMesh 위의 유효한 지점으로 이동 명령
                agent.stoppingDistance = 0f; // 순찰 지점까지는 정확히 이동
                agent.speed = enemyInfo.patrolSpeed;
                agent.SetDestination(hit.position);
            }

            // 4. 다음 배회 시간 설정
            patrolWanderTimer = patrolWanderInterval + UnityEngine.Random.Range(-1f, 1f);
        }
    }

    // (경계) 상태: 이동을 멈추고 타겟을 쳐다봄
    private void HandleStopAndWatchingMovement()
    {
        if (enemyInfo.nonHostileBehavior != null)
        {
            enemyInfo.nonHostileBehavior.Execute(enemyInfo, enemyController, sensor);
        }
        else
        {
            // SO가 없는 일반 유닛은 그냥 타겟을 쳐다봄
            if (enemyInfo.CurrentTarget != null)
            {
                Vector3 lookDir = (enemyInfo.CurrentTarget.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }
            }
        }
    }

    // (명령 이동) 상태: Controller가 설정한 CommandTargetPosition으로 이동
    private void HandleMovingToCommandMovement()
    {
        agent.stoppingDistance = 2.0f;
        agent.speed = enemyInfo.runSpeed;
        agent.SetDestination(enemyInfo.CommandTargetPosition);

        // agent.remainingDistance는 경로 계산이 완료되어야 정확함
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 도착 완료!
            StopMovement();
            // 이동 명령 완료
            enemyInfo.hasMoveCommand = false;
        }
    }

    // (교전) 상태: 타겟을 향해 스킬 사거리까지 접근
    private void HandleEngagingMovement()
    {
        // 타겟이 없으면(죽었거나) 이동을 멈추고 Controller가 상태를 바꿔주길 기다림
        if (enemyInfo.CurrentTarget == null)
        {
            StopMovement();
            return;
        }

        // 스킬 사거리에 맞춰 정지
        agent.stoppingDistance = enemyInfo.skillRange - 0.5f; // 사거리보다 0.5m 안쪽
        agent.speed = enemyInfo.engagingSpeed;
        agent.SetDestination(enemyInfo.CurrentTarget.position);
    }

    // (죽음) 상태: 모든 컴포넌트를 비활성화
    private void HandleDead()
    {
        StopMovement();
        agent.enabled = false;

        GetComponent<EnemyController>().enabled = false;
        GetComponent<EnemySensorSight>().enabled = false;
        GetComponent<SkillController>().enabled = false;
        GetComponent<SkillPipeline>().enabled = false; //

        this.enabled = false; // 자기 자신도 끈다
    }

    private void StopMovement()
    {
        if (agent.isOnNavMesh && !agent.isStopped)
        {
            agent.ResetPath();
        }
    }

    // BT가 "공격"할 때 이 함수를 호출합니다.
    public void PerformLungeAttack(Transform target, Action onHitCallback)
    {
        // 타겟이 유효하고, 이미 다른 코루틴이 실행 중이지 않을 때만 실행
        // (agent.enabled는 코루틴이 시작할 때 false가 되므로 좋은 플래그가 됨)
        if (target != null && agent.enabled)
        {
            StartCoroutine(LungeCoroutine(target, onHitCallback));
        }
    }

    private IEnumerator LungeCoroutine(Transform target, Action onHitCallback)
    {
        // 1. NavMeshAgent를 잠시 꺼서, 내가 직접 움직일 수 있게 함
        agent.enabled = false;

        Vector3 startPos = transform.position;
        // 목표 지점 (타겟의 살짝 앞)
        Vector3 targetPos = target.position - (target.position - startPos).normalized * 0.5f;

        // 2. (선딜) 0.1초간 살짝 멈춰서 '힘을 모으는' 느낌
        yield return new WaitForSeconds(0.1f);

        // 3. (돌진) 0.08초만에 목표 지점까지 돌진 (Lerp 사용)
        float t = 0;
        while (t < 0.08f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t / 0.08f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        //    DeliveryAsset이 "이때 실행해!"라고 맡겨둔 '데미지 로직'을 실행
        onHitCallback?.Invoke();
        // (SkillController/ImpactAsset이 데미지를 적용함) 

        // 4. (복귀) 0.1초만에 원래 자리로 복귀
        t = 0;
        while (t < 0.1f)
        {
            transform.position = Vector3.Lerp(targetPos, startPos, t / 0.1f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        // 5. NavMeshAgent를 다시 켜서 BT의 제어권을 돌려줌
        agent.enabled = true;
    }
}