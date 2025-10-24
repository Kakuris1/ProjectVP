using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CollisionReaction : MonoBehaviour
{
    [SerializeField]
    public float speed = 10f;
    public float smoothK = 12f;     // ũ�� tail�� ª���� (�������� ����)
    public float snapAbs = 0.0025f; // ���� ��� ���� (������ ����)
    public float snapRel = 0.004f;  // ��� ��� ���� (��ǥ�� ����)
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

    // �浹�ֺ� �⿩ġ �����: key=��� �ݶ��̴�, value=�� �ݶ��̴��� ��ǥ�� ���� ����
    private readonly Dictionary<Collider, Vector3> contrib = new Dictionary<Collider, Vector3>();

    void Update()
    {
        // ��ǥ ������ ������Ʈ
        UpdateLatch(ref LatchXP, GoalScaleXPlusBone, XPlusBone.localScale.y);
        UpdateLatch(ref LatchXM, GoalScaleXMinusBone, XMinusBone.localScale.y);
        UpdateLatch(ref LatchYP, GoalScaleYPlusBone, YPlusBone.localScale.y);
        UpdateLatch(ref LatchYM, GoalScaleYMinusBone, YMinusBone.localScale.y);
        UpdateLatch(ref LatchZP, GoalScaleZPlusBone, ZPlusBone.localScale.y);
        UpdateLatch(ref LatchZM, GoalScaleZMinusBone, ZMinusBone.localScale.y);

        //���� �������� Goal�� õõ�� ����
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

        // 1) ��ǥ ����(Ŭ����)
        float g = Mathf.Clamp(goal, 0.6f, 1.5f);

        // 2) ���� ���� �� ����
        var ls = t.localScale;
        float a = 1f - Mathf.Exp(-smoothK * Time.deltaTime); // ������ ����
        float next = Mathf.Lerp(ls.y, g, a);

        // 3) ���� ����: ����� ������ �ٷ� ����
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
        // �浹 ���� ����
        if (n > 0) avgNWorld = sum / n;

        // ���� ���� ���� ���ͷ� ��ȯ
        Vector3 avgNLocal = transform.InverseTransformDirection(avgNWorld);
        avgNLocal.Normalize();

        // �浹 Collider, ���� ����
        Collider key = collision.collider;
        contrib.Add(key, avgNLocal);

        // ��ǥ ������ ����
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
            LatchValue = RequestValue; // �߰� �浹 �ݿ�
        }
        else if(CurrentY >= LatchValue-eps)
        {
            LatchValue = RequestValue; // �ְ��� ���� �ÿ��� �۾���
        }
    }
}
