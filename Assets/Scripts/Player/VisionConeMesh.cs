using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    public Transform playerRoot;          // 플레이어
    public float eyeHeight = 1.3f;
    public float fovAngle = 90f;
    public float radius = 12f;
    public int segments = 64;
    public LayerMask obstacleMask;        // 시야를 가리는 물체 레이어
    public LayerMask groundMask;          // 지면 레이어
    public float groundRayHeight = 10f;         // 위에서 지면으로 쏠 레이 높이 (지면 보다 높아야함)
    public float groundOffset = 0.5f;    // 지면 Z-fighting 방지

    private readonly List<Vector3> verts = new List<Vector3>();
    private readonly List<int> tris = new List<int>();

    Mesh _mesh; MeshFilter _mf;

    void Awake() 
    { 
        _mf = GetComponent<MeshFilter>(); 
        _mesh = new Mesh();
        _mf.mesh = _mesh; 
    }

    void LateUpdate()
    {
        if (!playerRoot) return;
        SetVerts();
        DrawVerts();
    }
    void SetVerts()
    {
        verts.Clear();

        var origin = playerRoot.position + Vector3.up * eyeHeight;
        var forward = Vector3.ProjectOnPlane(playerRoot.forward, Vector3.up).normalized;

        // 0번 정점 = 원점(메쉬 로컬로 변환)
        verts.Add(transform.InverseTransformPoint(playerRoot.position + Vector3.up * groundOffset));

        float start = -fovAngle * 0.5f;
        for (int i = 0; i <= segments; ++i)
        {
            // 시야 그려지는 지점 x,z 좌표 레이캐스트
            float angle = start + (fovAngle * i / segments);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Vector3 p;
            if (Physics.Raycast(origin, dir, out var flatHit, radius, obstacleMask, QueryTriggerInteraction.Ignore))
                p = flatHit.point;
            else
                p = origin + dir * radius;

            // Y 값 조정
            p.y -= eyeHeight;
            // Terrain 사용시
            if (Terrain.activeTerrain)
            {
                var t = Terrain.activeTerrain;
                p.y =  t.SampleHeight(new Vector3(p.x, 0f, p.z)) + t.transform.position.y;
            }
            // 메시 지면 사용시, 위에서 아래로 레이캐스트
            Vector3 from = new Vector3(p.x, p.y + groundRayHeight, p.z);
            if (Physics.Raycast(from, Vector3.down, out var groundHit, groundRayHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
                p.y = groundHit.point.y;

            verts.Add(transform.InverseTransformPoint(p + Vector3.up * groundOffset));
        }
    }
    
    void DrawVerts()
    {
        tris.Clear();
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