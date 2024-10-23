using UnityEngine;

namespace LineOfSight
{
    public class VisionMesh : MonoBehaviour
    {
        public Agent _agent;
        private RayData[] _rayDatas;
        private float _radius;

        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;

        private Mesh _mesh;
        private MeshFilter _meshFilter;


        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
        }

        private void LateUpdate()
        {
            _rayDatas = _agent._rayDatas;
            _radius = _agent._radius;
            GenerateMesh();
        }

        private void GenerateMesh()
        {
            int meshCount = _rayDatas.Length - 1;
            int vertexCount = meshCount * 2 + 1;
            int triangleCount = meshCount * 3;

            _vertices = new Vector3[vertexCount];
            _vertices[0] = Vector3.zero;
            for (int i = 1, mesh = 0; i < _vertices.Length; i += 2, mesh++)
            {
                _vertices[i] = transform.InverseTransformPoint(_rayDatas[mesh].m_end);
                _vertices[i + 1] = transform.InverseTransformPoint(_rayDatas[mesh + 1].m_end);
            }

            _triangles = new int[triangleCount];
            for (int i = 0; i < meshCount; i++)
            {
                _triangles[i * 3] = 0;
                _triangles[i * 3 + 1] = i * 2 + 1;
                _triangles[i * 3 + 2] = i * 2 + 2;
            }

            _uvs = new Vector2[vertexCount];
            _uvs[0] = new Vector2(0.5f, 0.5f);
            float lerp = 0;
            Vector3 direction = Vector3.zero;
            for (int i = 1, mesh = 0; i < _uvs.Length; i += 2, mesh++)
            {
                lerp = _vertices[i].magnitude / _radius;
                direction = _rayDatas[mesh].m_direction * _rayDatas[mesh].m_distance * 0.6f / _radius;
                _uvs[i] = new Vector2(direction.x, direction.z) * lerp + _uvs[0];

                lerp = _vertices[i + 1].magnitude / _radius;
                direction = _rayDatas[mesh + 1].m_direction * _rayDatas[mesh].m_distance * 0.6f / _radius;
                _uvs[i + 1] = new Vector2(direction.x, direction.z) * lerp + _uvs[0];
            }

            _mesh.Clear();
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.uv = _uvs;
            _mesh.RecalculateNormals();

            _meshFilter.mesh = _mesh;
        }
    }
}