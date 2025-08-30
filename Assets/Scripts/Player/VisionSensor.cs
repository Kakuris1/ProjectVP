using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public Transform eye;
    public float eyeHeight = 1.3f;
    public float fovAngle = 90f, radius = 4f, rearRadius = 2.0f;
    public LayerMask obstacleMask;
    public LayerMask enemyMask;       // ��
    public LayerMask allyMask;        // �Ʊ�(�׻� ���̰� ó��)

    public float updateHz = 10f;      // �ʴ� ���� Ƚ��
    float _timer;

    readonly HashSet<VisibilityFader> _visible = new();

    Collider[] _buf = new Collider[128];

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f / updateHz) return;
        _timer = 0f;

        var now = new HashSet<VisibilityFader>();

        var origin = eye.position + Vector3.up * eyeHeight;
        var forward = Vector3.ProjectOnPlane(eye.forward, Vector3.up).normalized;
        float maxR = Mathf.Max(radius, rearRadius);

        int n = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, enemyMask, QueryTriggerInteraction.Ignore);
        Debug.Log(n + "�� ����");
        for (int i = 0; i < n; ++i)
        {
            var t = _buf[i].transform;
            if (!t) continue;

            var fader = t.GetComponentInParent<VisibilityFader>();
            if (!fader) continue;

            if (IsVisible(origin, forward, t))
                now.Add(fader);
            Debug.Log(i+1 + "�� ");
        }

        // �Ʊ��� �׻� ����
        int m = Physics.OverlapSphereNonAlloc(origin, maxR, _buf, allyMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < m; ++i)
        {
            var fader = _buf[i].GetComponentInParent<VisibilityFader>();
            if (fader) { now.Add(fader); fader.SetVisible(true); }
        }

        // Enter/Exit ó��
        foreach (var f in now) if (_visible.Add(f)) f.SetVisible(true);
        var toHide = new List<VisibilityFader>();
        foreach (var f in _visible) if (!now.Contains(f)) toHide.Add(f);
        foreach (var f in toHide) { f.SetVisible(false); _visible.Remove(f); }
    }

    bool IsVisible(Vector3 origin, Vector3 forward, Transform target)
    {
        var col = target.GetComponentInParent<Collider>();
        if (col == null) return false;
        Vector3 closestColliderPoint = col.ClosestPoint(origin); // Ÿ�� Collider���� �ִ� ����
        Vector3 dir = closestColliderPoint - origin;    // �ش� �������� ���� ����
        float dist = dir.magnitude;
        Vector3 toFlat = dir; toFlat.y = 0f; // ���� üũ ���� ���� ����

        bool angleOK = (dist <= rearRadius) || Vector3.Angle(forward, toFlat) <= fovAngle * 0.5f;
        if (!angleOK) return false; // �������� �ʰ�,, �þ� �� ���� ���̸� �Ⱥ���

        // Ÿ�� Collider �ִ� ������ �������� ������ �Ⱥ���
        if (!col.Raycast(new Ray(origin, dir.normalized), out var hitToTarget, dir.magnitude + 0.05f))
            return false;

        // ��ֹ��� ������ �� ����
        if (Physics.Raycast(origin, dir.normalized, dist + 0.05f, obstacleMask, QueryTriggerInteraction.Ignore))
            return false;

        // �� ���ǵ� ������ ����
        return true;
    }
}
