using UnityEngine;

namespace LineOfSight
{
    public class RayData
    {
        public Vector3 m_start;
        public float m_distance;
        public float m_angle;

        public Vector3 m_direction;
        public Vector3 m_end;

        public Collider m_hitCollider;
        public bool m_hit;

        public RayData(Vector3 start, float angle, float distance)
        {
            SetData(start, angle, distance);
        }

        public void SetData(Vector3 start, float angle, float distance)
        {
            m_start = start;
            m_distance = distance;
            m_angle = angle;

            UpdateDirection();
        }

        public void RotateAngle(float angle)
        {
            m_angle += angle;
            UpdateDirection();
        }

        private void UpdateDirection()
        {
            m_direction = DirectionFromAngle(m_angle);
            m_end = m_start + m_direction * m_distance;
        }

        private Vector3 DirectionFromAngle(float angle)
        {
            float pi = Mathf.Deg2Rad;

            return new Vector3(Mathf.Sin(angle * pi), 0, Mathf.Cos(angle * pi));
        }

        public static bool IsHittingSameObject(RayData data1, RayData data2)
        {
            return data1.m_hitCollider == data2.m_hitCollider;
        }
    }
}