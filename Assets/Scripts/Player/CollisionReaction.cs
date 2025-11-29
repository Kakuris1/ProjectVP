using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CollisionReaction : MonoBehaviour
{
    [SerializeField]
    public float speed = 10f;
    public float smoothK = 12f;     // 크면 tail이 짧아짐 (지수감쇠 세기)
    public float snapAbs = 0.0025f; // 절대 허용 오차 (스케일 단위)
    public float snapRel = 0.004f;  // 상대 허용 오차 (목표값 비율)
    public const float mt = 0.15f; // MaxTransform per each crush

    public Transform XPlusBone;
    public Transform YPlusBone;
    public Transform ZPlusBone;
    public Transform XMinusBone;
    public Transform YMinusBone;
    public Transform ZMinusBone;

    private float GoalScaleXPlusBone = 1f;
    private float GoalScaleYPlusBone = 1f;
    private float GoalScaleZPlusBone = 1f;
    private float GoalScaleXMinusBone = 1f;
    private float GoalScaleYMinusBone = 1f;
    private float GoalScaleZMinusBone = 1f;

    private float LatchXP = 1f;
    private float LatchYP = 1f;
    private float LatchZP = 1f;
    private float LatchXM = 1f;
    private float LatchYM = 1f;
    private float LatchZM = 1f;

    // 충돌쌍별 기여치 저장소: key=상대 콜라이더, value=그 콜라이더의 목표에 더한 벡터
    private readonly Dictionary<Collider, Vector3> contrib = new Dictionary<Collider, Vector3>();

    void Update()
    {
        // 목표 스케일 업데이트
        UpdateLatch(ref LatchXP, GoalScaleXPlusBone, XPlusBone.localScale.y);
        UpdateLatch(ref LatchXM, GoalScaleXMinusBone, XMinusBone.localScale.y);
        UpdateLatch(ref LatchYP, GoalScaleYPlusBone, YPlusBone.localScale.y);
        UpdateLatch(ref LatchYM, GoalScaleYMinusBone, YMinusBone.localScale.y);
        UpdateLatch(ref LatchZP, GoalScaleZPlusBone, ZPlusBone.localScale.y);
        UpdateLatch(ref LatchZM, GoalScaleZMinusBone, ZMinusBone.localScale.y);

        //현재 스케일을 Goal로 천천히 보간
        LerpY(XPlusBone, LatchXP);
        LerpY(XMinusBone, LatchXM);
        LerpY(YPlusBone, LatchYP);
        LerpY(YMinusBone, LatchYM);
        LerpY(ZPlusBone, LatchZP);
        LerpY(ZMinusBone, LatchZM);
    }

    void LerpY(Transform t, float goal)
    {
        if (!t) return;

        // 1) 목표 보정(클램프)
        float g = Mathf.Clamp(goal, 0.6f, 1.5f);

        // 2) 지수 보간 한 스텝
        var ls = t.localScale;
        float a = 1f - Mathf.Exp(-smoothK * Time.deltaTime); // 프레임 독립
        float next = Mathf.Lerp(ls.y, g, a);

        // 3) 근접 스냅: 충분히 가까우면 바로 붙임
        float tol = Mathf.Max(snapAbs, snapRel * Mathf.Max(1f, Mathf.Abs(g)));
        ls.y = (Mathf.Abs(g - next) <= tol) ? g : next;

        t.localScale = ls;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 sum = Vector3.zero;
        int n = collision.contactCount;
        for (int i = 0; i < n; i++) sum += collision.GetContact(i).normal;
        Vector3 avgNWorld = Vector3.zero;
        // 충돌 법선 벡터
        if (n > 0) avgNWorld = sum / n;

        // 월드 벡터 로컬 벡터로 변환
        Vector3 avgNLocal = transform.InverseTransformDirection(avgNWorld);
        avgNLocal.Normalize();

        // 충돌 Collider, 벡터 저장
        Collider key = collision.collider;
        contrib.Add(key, avgNLocal);

        // 목표 스케일 설정
        SetGoalBoneScale(avgNLocal);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (contrib.Count==0) SetGoalBoneScale(new Vector3(1f, 1f, 1f));
        Collider key = collision.collider;
        if(contrib.TryGetValue(key, out var prev))
        {
            SetGoalBoneScale(-prev);
            contrib.Remove(key);
        }
    }

    private void SetGoalBoneScale(Vector3 avgNLocal)
    {
        GoalScaleXPlusBone += mt * avgNLocal.x;
        GoalScaleXMinusBone -= mt * avgNLocal.x;
        GoalScaleYPlusBone += mt * avgNLocal.y;
        GoalScaleYMinusBone -= mt * avgNLocal.y;
        GoalScaleZPlusBone += mt * avgNLocal.z;
        GoalScaleZMinusBone -= mt * avgNLocal.z;
    }

    private void UpdateLatch(ref float LatchValue, float RequestValue, float CurrentY)
    {
        const float eps = 1e-4f;
        if(LatchValue < RequestValue)
        {
            LatchValue = RequestValue; // 추가 충돌 반영
        }
        else if(CurrentY >= LatchValue-eps)
        {
            LatchValue = RequestValue; // 최고점 도달 시에만 작아짐
        }
    }
}
