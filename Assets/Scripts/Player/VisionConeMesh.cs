using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    public Transform playerRoot;          // �÷��̾�
    public float eyeHeight = 1.3f;
    public float fovAngle = 90f;
    public float radius = 12f;
    public int segments = 64;
    public LayerMask obstacleMask;        // �þ߸� ������ ��ü ���̾�
    public LayerMask groundMask;          // ���� ���̾�
    public float groundRayHeight = 10f;         // ������ �������� �� ���� ���� (���� ���� ���ƾ���)
    public float groundOffset = 0.5f;    // ���� Z-fighting ����

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

        // 0�� ���� = ����(�޽� ���÷� ��ȯ)
        verts.Add(transform.InverseTransformPoint(playerRoot.position + Vector3.up * groundOffset));

        float start = -fovAngle * 0.5f;
        for (int i = 0; i <= segments; ++i)
        {
            // �þ� �׷����� ���� x,z ��ǥ ����ĳ��Ʈ
            float angle = start + (fovAngle * i / segments);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Vector3 p;
            if (Physics.Raycast(origin, dir, out var flatHit, radius, obstacleMask, QueryTriggerInteraction.Ignore))
                p = flatHit.point;
            else
                p = origin + dir * radius;

            // Y �� ����
            p.y -= eyeHeight;
            // Terrain ����
            if (Terrain.activeTerrain)
            {
                var t = Terrain.activeTerrain;
                p.y =  t.SampleHeight(new Vector3(p.x, 0f, p.z)) + t.transform.position.y;
            }
            // �޽� ���� ����, ������ �Ʒ��� ����ĳ��Ʈ
            Vector3 from = new Vector3(p.x, p.y + groundRayHeight, p.z);
            if (Physics.Raycast(from, Vector3.down, out var groundHit, groundRayHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
                p.y = groundHit.point.y;

            verts.Add(transform.InverseTransformPoint(p + Vector3.up * groundOffset));
        }
    }
    
    void DrawVerts()
    {
        tris.Clear();
        // �ﰢ��(��)
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