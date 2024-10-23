using UnityEngine;

namespace LineOfSight
{
    public class Agent : MonoBehaviour
    {
        public int _divide = 1, _angle = 60;
        public float _radius = 5;

        public RayData[] _rayDatas;

        public float _approximationPrecision = 0.5f;
        public float _bisectionCount = 5;

        private void Update()
        {
            if (_divide <= 0 || _divide > _angle)
                _rayDatas = null;
            else
                _rayDatas = GetNormalDatas();

            if (_rayDatas.Length > 1)
            {
                for (int i = 0; i < _rayDatas.Length - 1; i++)
                {
                    EdgeData edge = GetBisectionEdge(_rayDatas[i], _rayDatas[i + 1]);
                    if (edge != null)
                    {
                        _rayDatas[i] = edge.m_firstRay;
                        _rayDatas[i + 1] = edge.m_secondRay;
                    }
                }
            }
        }

        private RayData[] GetOriginalDatas()
        {
            RayData[] rayDatas = new RayData[_divide + 1];

            Vector3 center = transform.position;
            float startAngle = transform.eulerAngles.y - _angle / 2;
            float angle = _angle / _divide;
            RayData rayDataCache = null;

            for (int i = 0; i <= _divide; i++)
            {
                rayDataCache = new RayData(center, startAngle + angle * i, _radius);

                rayDatas[i] = rayDataCache;
            }

            return rayDatas;
        }

        private RayData[] GetNormalDatas()
        {
            RayData[] rayDatas = GetOriginalDatas();

            for (int i = 0; i < rayDatas.Length; i++)
            {
                UpdateRaycast(rayDatas[i]);
            }

            return rayDatas;
        }

        private void UpdateRaycast(RayData rayData)
        {
            rayData.m_hit = Physics.Raycast(transform.position, rayData.m_direction, out var _hit, _radius);

            if (rayData.m_hit)
            {
                rayData.m_hitCollider = _hit.collider;
                rayData.m_end = _hit.point;
            }
            else
            {
                rayData.m_hitCollider = null;
                rayData.m_end = rayData.m_start + rayData.m_direction * _radius;
            }
        }

        private EdgeData GetApproximationEdge(RayData startEdgeRayData, RayData endEdgeRayData)
        {
            if (_approximationPrecision <= 0) { return null; }

            Vector3 center = transform.position;
            float maxAngle = Vector3.Angle(startEdgeRayData.m_direction, endEdgeRayData.m_direction);
            float curAngle = _approximationPrecision;

            RayData edgeRayData = new RayData(center, startEdgeRayData.m_angle + _approximationPrecision, _radius);
            UpdateRaycast(edgeRayData);

            while (RayData.IsHittingSameObject(startEdgeRayData, edgeRayData))
            {
                curAngle += _approximationPrecision;
                if (curAngle > maxAngle)
                {
                    edgeRayData = null;
                    break;
                }

                edgeRayData.RotateAngle(_approximationPrecision);
                UpdateRaycast(edgeRayData);
            }

            if (edgeRayData == null)
            {
                return null;
            }

            EdgeData edgeData = new EdgeData();
            edgeData.m_secondRay = edgeRayData;
            edgeData.m_firstRay = new RayData(center, edgeRayData.m_angle - _approximationPrecision, _radius);
            UpdateRaycast(edgeData.m_firstRay);

            return edgeData;
        }

        private EdgeData GetBisectionEdge(RayData startEdgeRayData, RayData endEdgeRayData)
        {
            if (!startEdgeRayData.m_hit && !endEdgeRayData.m_hit)
            {
                return GetApproximationEdge(startEdgeRayData, endEdgeRayData);
            }

            if (RayData.IsHittingSameObject(startEdgeRayData, endEdgeRayData))
            {
                return null;
            }

            Vector3 center = transform.position;
            EdgeData edgeData = new EdgeData();
            float angle = 0;
            RayData edgeRayData = null;

            for (int i = 0; i < _bisectionCount; i++)
            {
                angle = (startEdgeRayData.m_angle + endEdgeRayData.m_angle) / 2;
                edgeRayData = new RayData(center, angle, _radius);
                UpdateRaycast(edgeRayData);

                if (RayData.IsHittingSameObject(startEdgeRayData, edgeRayData))
                {
                    startEdgeRayData = edgeRayData;
                }
                else
                {
                    endEdgeRayData = edgeRayData;
                }
            }

            edgeData.m_firstRay = startEdgeRayData;
            edgeData.m_secondRay = endEdgeRayData;

            return edgeData;
        }

        private void OnDrawGizmos()
        {
            if (_rayDatas != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _rayDatas.Length; i++)
                {
                    RayData rayData = _rayDatas[i];
                    Gizmos.DrawLine(rayData.m_start, rayData.m_end);
                }
            }
        }

        private class EdgeData
        {
            public RayData m_firstRay;
            public RayData m_secondRay;
        }
    }
}