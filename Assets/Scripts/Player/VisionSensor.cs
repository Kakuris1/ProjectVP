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

    Collider[] _buf = new Collider[128];
    List<Transform> TargetColliders = new List<Transform>();
    Transform NearestTarget;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f / updateHz) return;
        _timer = 0f;

        //스캔 시작 시, 리스트를 초기화(Clear)합니다.
        TargetColliders.Clear();
        NearestTarget = null; // 가장 가까운 타겟도 초기화

        var now = new HashSet<VisibilityFader>();

        var origin = eye.position + Vector3.up * eyeHeight;
        var forward = Vector3.ProjectOnPlane(eye.forward, Vector3.up).normalized;
        float maxR = Mathf.Max(radius, rearRadius);

        int n = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, enemyMask, QueryTriggerInteraction.Ignore);
        float closestDistanceSqr = float.MaxValue;
        for (int i = 0; i < n; ++i)
        {
            var t = _buf[i].transform;
            if (!t) continue;

            var fader = t.GetComponentInParent<VisibilityFader>();
            if (!fader) continue;

            if (IsVisible(origin, forward, t))
            {
                now.Add(fader);

                // SkillController 에 넘길 타겟 정보
                TargetColliders.Add(t);
                // 가장 가까운 대상
                float distanceSqr = (t.position - transform.position).magnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    NearestTarget = t;
                }
            }
        }

        // 아군은 항상 보임
        int m = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, allyMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < m; ++i)
        {
            var fader = _buf[i].GetComponentInParent<VisibilityFader>();
            if (fader) { now.Add(fader); fader.SetVisible(true); }
        }

        // Enter/Exit 처리
        foreach (var f in now) if (_visible.Add(f)) f.SetVisible(true);
        var toHide = new List<VisibilityFader>();
        foreach (var f in _visible) if (!now.Contains(f)) toHide.Add(f);
        foreach (var f in toHide)
        {
            if (f != null) // f가 파괴되지 않고 살아있을 때만 함수를 호출
            {
                f.SetVisible(false);
            }
            _visible.Remove(f); // 목록에서는 파괴되었든 아니든 제거
        }
    }

    bool IsVisible(Vector3 origin, Vector3 forward, Transform target)
    {
        var col = target.GetComponentInParent<Collider>();
        if (col == null) return false;
        Vector3 closestColliderPoint = col.ClosestPoint(origin); // 타깃 Collider와의 최단 지점
        Vector3 dir = closestColliderPoint - origin;    // 해당 지점으로 방향 벡터
        float dist = dir.magnitude;
        Vector3 toFlat = dir; toFlat.y = 0f; // 각도 체크 위한 수평 벡터

        bool angleOK = (dist <= rearRadius) || Vector3.Angle(forward, toFlat) <= fovAngle * 0.5f;
        if (!angleOK) return false; // 근접하지 않고,, 시야 각 범위 밖이면 안보임

        // 타겟 Collider 최단 지점에 도달하지 않으면 안보임
        if (!col.Raycast(new Ray(origin, dir.normalized), out var hitToTarget, dir.magnitude + 0.05f))
            return false;

        // 장애물에 가리면 안 보임
        if (Physics.Raycast(origin, dir.normalized, dist + 0.05f, obstacleMask, QueryTriggerInteraction.Ignore))
            return false;

        // 위 조건들 충족시 보임
        return true;
    }

    public List<Transform> GetCurrentTargetList()
    {
        return TargetColliders;
    }

    public Transform GetNearestTarget()
    {
        return NearestTarget;
    }
}
