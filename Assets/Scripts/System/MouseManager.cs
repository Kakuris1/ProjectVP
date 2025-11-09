using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면 이 오브젝트는 파괴
            Destroy(gameObject);
        }
    }

    // 인스펙터 확인용 리스트
    public List<int> MouseIDList;
    // 현재 플레이어 경계중인 쥐 리스트, 읽기 전용 프로퍼티
    private List<MouseInformation> MouseList = new List<MouseInformation>();
    public IReadOnlyList<MouseInformation> CurrentChasingMouseList => MouseList;
    public Vector3 CurrentConvengePoint { get; private set; }

    [Header("이 이상 숫자가 모이면 적대적")]
    public int hostileMouseCount = 7; // 이 숫자 이상 모이면 적대적으로 전환
    [Header("이 이하로 숫자가 줄면 도망(비적대적)")]
    public int nonHostileMouseCount = 2; // 이 숫자까지 줄어들면 도망
    [Header("생쥐 무리의 플레이어와 유지 거리")]
    public int DistanceWithPlayer = 30;
    [Header("이 수치 이하로 피가 까이면 잠시 후퇴")]
    public float runAwayHP = 60f;
    [Header("생쥐 무리의 전원 동시 공격 명령")]
    public bool overrideKitingAndAttack = false;

    // 생쥐 무리 수의 변화 알림
    public event Action MouseSwarmUpdate;
    private float scanTimer;
    private float updateHz = 1f; // 초당 1회 실행

    private void Update()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer < 1f / updateHz) return;

        // 생쥐 집합 장소 갱신
        SetMouseConvengePoint();
        // 생쥐들의 체력 상태를 체크 
        CheckWoundedStatus();
        scanTimer = 0f;
    }

    public void MeetPlayer(MouseInformation Mouse)
    {
        if(!MouseList.Contains(Mouse))
        {
            // 경계 중인 쥐 추가
            MouseList.Add(Mouse);
            MouseIDList.Add(Mouse.EnemyID);
            Debug.Log($"생쥐{Mouse.EnemyID}가 Player를 발견하였습니다. 현재 생쥐 무리 수 : {MouseList.Count}");

            // 생쥐 무리 변화 이벤트 알림
            MouseSwarmUpdate?.Invoke();

            // 생쥐 무리가 충분히 모이면 공격 (적대적으로 변환)
            int M_Count = MouseList.Count;
            if (M_Count >= hostileMouseCount)
            {
                for (int i = 0; i < M_Count; i++)
                {
                    MouseList[i].Hostile = true;
                }
            }
        }
    }

    public void RemoveMouse(MouseInformation Mouse)
    {
        if (MouseList.Contains(Mouse))
        {
            // 죽은 쥐 제거
            MouseList.Remove(Mouse);
            MouseIDList.Remove(Mouse.EnemyID);
            Debug.Log($"생쥐{Mouse.EnemyID}가 죽었습니다. 현재 생쥐 무리 수 : {MouseList.Count}");

            // 생쥐 무리 변화 이벤트 알림
            MouseSwarmUpdate?.Invoke();

            // 생쥐 무리가 줄어들면 도망 (비적대적으로 변환)int M_Count = MouseList.Count;
            int M_Count = MouseList.Count;
            if (M_Count <= nonHostileMouseCount)
            {
                for (int i = 0; i < M_Count; i++)
                {
                    MouseList[i].Hostile = false; // 비적대적으로 변환
                    MouseList[i].SetCurrentHP(100); // 체력 회복
                }

                // 임시 : 한번 도망갈 때 마다, 요구되는 무리 숫자 +1
                hostileMouseCount++;
                nonHostileMouseCount++;
            }
        }
    }

    // 이 MouseManager 오브젝트의 위치는 출구 앞 -> 생쥐들이 출구 방향쪽 기준으로 모이게 함
    private void SetMouseConvengePoint()
    {
        Vector3 playerPosition = Player.Instance.transform.position;
        Vector3 dir = (transform.position - playerPosition).normalized;
        CurrentConvengePoint = playerPosition + dir * DistanceWithPlayer;
    }

    // 전원 돌격 플래그를 갱신하는 함수
    private void CheckWoundedStatus()
    {
        // 경계 중인 쥐가 없으면 플래그를 내림
        if (MouseList.Count == 0)
        {
            overrideKitingAndAttack = false;
            return;
        }

        foreach (MouseInformation mouse in MouseList)
        {
            // 한마리라도 후퇴하지 않았다면
            if (mouse.CurrentHP >= runAwayHP)
            {
                overrideKitingAndAttack = false; // "전원 돌격" 취소
                return;
            }
        }

        // (위의 'return'이 실행되지 않고) 루프가 끝까지 돌았다면
        // -> "모든 쥐가 60% 미만"이라는 뜻
        overrideKitingAndAttack = true; // "전원 돌격" 명령!
    }
}
