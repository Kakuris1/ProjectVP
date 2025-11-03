using System;
using System.Collections.Generic;
using UnityEngine;

// 팀 관리 스크립트
public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    // 팀 전체 전투 모드/비전투모드
    public bool IsCombatMode {  get; private set; }

    // 아군 목록 인스펙터 확인용 리스트
    public List<int> teamlist;

    // 아군 목록 리스트, 읽기전용 프로퍼티
    private List<AllyInformation> allies = new List<AllyInformation>();
    public IReadOnlyList<AllyInformation> CurrentTeam => allies;

    // 팀 구성원이 변경되었을 때 발생하는 이벤트
    public event Action OnTeamUpdated;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 지정된 유닛을 아군으로 영입하고 팀 목록에 추가
    public void RecruitAlly(AllyInformation newAlly)
    {
        // 이미 팀에 합류한 유닛인지 확인하여 중복 추가를 방지
        if (!allies.Contains(newAlly))
        {
            allies.Add(newAlly);

            Debug.Log($"{newAlly.name}을(를) 아군으로 영입했습니다! 현재 팀원 수: {allies.Count}");

            // 팀 리스트 확인용 변수
            teamlist.Add(newAlly.targetAreaNumber);

            // 팀원이 추가되었으므로 포메이션 슬롯을 갱신
            UpdateFormationSlots();

            // 팀이 업데이트되었다고 이벤트 발송 (UI 갱신 등에 사용)
            OnTeamUpdated?.Invoke();
        }
    }

    // 지정된 유닛을 팀에서 제외 (사망 등의 경우)
    public void RemoveAlly(AllyInformation allyToRemove)
    {
        if (allies.Contains(allyToRemove))
        {
            allies.Remove(allyToRemove);
            Debug.Log($"{allyToRemove.name}이(가) 팀에서 이탈했습니다. 현재 팀원 수: {allies.Count}");

            // 팀원이 제거되었으므로 포메이션 슬롯을 갱신
            UpdateFormationSlots();

            OnTeamUpdated?.Invoke();
        }
    }

    // 현재 아군 목록을 기준으로 모든 아군의 포메이션 슬롯 번호를 갱신
    private void UpdateFormationSlots()
    {
        for (int i = 0; i < allies.Count; i++)
        {
            // 리스트의 각 아군에게 현재 인덱스(순서)를 포메이션 슬롯 번호로 할당
            if (allies[i] != null)
            {
                allies[i].FormationSlot = i;
            }
        }
        Debug.Log("모든 아군의 포메이션 슬롯이 재정렬되었습니다.");
    }

    // 팀 전체 전투모드/비전투모드 토글 버튼
    public void ToggleCombatMode(bool currentState)
    {
        IsCombatMode = currentState;
        Debug.Log("현재 공격 모드 " + IsCombatMode);
    }
}

