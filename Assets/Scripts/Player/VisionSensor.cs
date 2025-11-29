using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour, ISkillTargetSensor
{
    public Transform eye;
    public float eyeHeight = 1.3f;
    public float fovAngle = 90f, radius = 4f, rearRadius = 2.0f;
    public LayerMask obstacleMask;
    public LayerMask enemyMask;       // 적
    public LayerMask allyMask;        // 아군(항상 보이게 처리)

    public float updateHz = 10f;      // 초당 판정 횟수
    float _timer;

    readonly HashSet<VisibilityFader> _visible = new();
    private readonly HashSet<VisibilityFader> _nowVisibleFaders = new HashSet<VisibilityFader>();
    private readonly List<VisibilityFader> _fadersToHide = new List<VisibilityFader>();

    Collider[] _buf = new Collider[128];
    List<Transform> TargetColliders = new List<Transform>();
    Transform NearestTarget;
    Collider NearestTargetCollider;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f / updateHz) return;
        _timer = 0f;

        //스캔 시작 시, 리스트를 초기화(Clear)합니다.
        TargetColliders.Clear();
        NearestTarget = null; // 가장 가까운 타겟도 초기화
        NearestTargetCollider = null; // 콜라이더 캐시도 초기화

        _nowVisibleFaders.Clear();

        var origin = eye.position + Vector3.up * eyeHeight;
        var forward = Vector3.ProjectOnPlane(eye.forward, Vector3.up).normalized;
        float maxR = Mathf.Max(radius, rearRadius);

        int n = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, enemyMask, QueryTriggerInteraction.Ignore);
        float closestDistanceSqr = float.MaxValue;
        for (int i = 0; i < n; ++i)
        {
            var col = _buf[i];
            if (col == null) continue;
            var t = col.transform;

            var fader = t.GetComponentInParent<VisibilityFader>();
            if (!fader) continue;

            if (IsVisible(origin, forward, t, col))
            {
                _nowVisibleFaders.Add(fader);

                // SkillController 에 넘길 타겟 정보
                TargetColliders.Add(t);

                // 가장 가까운 대상 (NearestTarget 계산은 ClosestPoint가 여전히 유효함)
                Vector3 closestPoint = col.ClosestPoint(origin);
                float distanceSqr = (closestPoint - origin).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    NearestTarget = t;
                    NearestTargetCollider = col; // 콜라이더도 함께 캐시
                }
            }
        }

        // 아군은 항상 보임
        int m = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, allyMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < m; ++i)
        {
            var fader = _buf[i].GetComponentInParent<VisibilityFader>();
            if (fader) { _nowVisibleFaders.Add(fader); fader.SetVisible(true); }
        }

        // Enter/Exit 처리
        foreach (var f in _nowVisibleFaders) if (_visible.Add(f)) f.SetVisible(true);
        _fadersToHide.Clear();
        foreach (var f in _visible) if (!_nowVisibleFaders.Contains(f)) _fadersToHide.Add(f);
        foreach (var f in _fadersToHide)
        {
            if (f != null) // f가 파괴되지 않고 살아있을 때만 함수를 호출
            {
                f.SetVisible(false);
            }
            _visible.Remove(f); // 목록에서는 파괴되었든 아니든 제거
        }
    }
    bool IsVisible(Vector3 origin, Vector3 forward, Transform target, Collider col)
    {
        // 1.  '가장 가까운 지점' 대신 '콜라이더의 중심점'을 사용
        Vector3 targetPoint = col.bounds.center;

        // 2. 중심점을 기준으로 방향, 거리, 각도 계산
        Vector3 dir = targetPoint - origin;    // 해당 지점으로 방향 벡터
        float dist = dir.magnitude;
        Vector3 toFlat = dir; toFlat.y = 0f; // 각도 체크 위한 수평 벡터

        // 3. 시야각 체크
        bool angleOK = (dist <= rearRadius) || Vector3.Angle(forward, toFlat) <= fovAngle * 0.5f;
        if (!angleOK) return false; // 근접하지 않고,, 시야 각 범위 밖이면 안보임

        // 4. [제거] 불필요하고 불안정했던 col.Raycast 체크 제거
        // if (!col.Raycast(...)) return false;

        // 5. 장애물 체크 (origin -> targetPoint)
        //    (Raycast가 타겟 자신의 콜라이더를 무시하도록 QueryTriggerInteraction.Ignore 사용)
        if (Physics.Raycast(origin, dir.normalized, dist - 0.1f, obstacleMask, QueryTriggerInteraction.Ignore))
            return false;

        // 6. 위 조건들 충족시 보임
        return true;
    }
    // ▲▲▲ [수정 완료] ▲▲▲


    public List<Transform> GetCurrentTargetList()
    {
        return TargetColliders;
    }

    public Transform GetNearestTarget()
    {
        return NearestTarget;
    }

    public bool IsNearestTargetInAttackRange(float range, Vector3 origin)
    {
        // (이 함수는 ClosestPoint를 쓰는 것이 맞으므로 수정하지 않습니다)
        if (NearestTarget == null || NearestTargetCollider == null)
        {
            return false;
        }
        Vector3 closestPoint = NearestTargetCollider.ClosestPoint(origin);
        float distance = Vector3.Distance(origin, closestPoint);
        return distance <= range;
    }
}