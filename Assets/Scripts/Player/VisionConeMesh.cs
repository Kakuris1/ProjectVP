using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    public Transform eye;                 // 시야 원점(머리/가슴 높이)
    public float eyeHeight = 1.3f;
    public float fovAngle = 90f;
    public float radius = 12f;
    public int segments = 64;
    public LayerMask obstacleMask;        // 레이캐스트 충돌 레이어
    public float groundOffset = 0.02f;    // 지면 Z-fighting 방지

    Mesh _mesh; MeshFilter _mf;

    void Awake() { _mf = GetComponent<MeshFilter>(); _mesh = new Mesh(); _mf.mesh = _mesh; }

    void LateUpdate()
    {
        if (!eye) return;

        var origin = eye.position + Vector3.up * eyeHeight;
        var forward = Vector3.ProjectOnPlane(eye.forward, Vector3.up).normalized;

        var verts = new List<Vector3>();
        var tris = new List<int>();

        // 0번 정점 = 원점(메쉬 로컬로 변환)
        verts.Add(transform.InverseTransformPoint(origin + Vector3.up * (-eyeHeight) + Vector3.up * groundOffset));

        float start = -fovAngle * 0.5f;
        for (int i = 0; i <= segments; ++i)
        {
            float a = start + (fovAngle * i / segments);
            Vector3 dir = Quaternion.Euler(0, a, 0) * forward;

            Vector3 p;
            if (Physics.Raycast(origin, dir, out var hit, radius, obstacleMask, QueryTriggerInteraction.Ignore))
                p = hit.point;
            else
                p = origin + dir * radius;

            verts.Add(transform.InverseTransformPoint(p - Vector3.up * eyeHeight + Vector3.up * groundOffset));
        }

        // 삼각형(팬)
        for (int i = 1; i < verts.Count - 1; ++i)
        {
            tris.Add(0); tris.Add(i); tris.Add(i + 1);
        }

        _mesh.Clear();
        _mesh.SetVertices(verts);
        _mesh.SetTriangles(tris, 0);
        _mesh.RecalculateBounds();
    }
}


// 지형이 울퉁불퉁한 경우에 대한 추가 코드 필요함
