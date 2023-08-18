using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;



namespace Slice
{

    public class Plane
    {
        #region attributes
        public float a, b, c, d;
        public Vector3 reference;
        public Vector3 normal
        {
            get
            {

                return new Vector3(a, b, c).normalized;
            }
        }

        #endregion

        #region operator
        #endregion

        #region functions

        public Plane() { }

        /// <summary>
        /// 根据参照点和法线确定平面
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="normal"></param>
        public Plane(Vector3 reference, Vector3 normal)
        {
            this.reference = reference;
            a = normal.x;
            b = normal.y;
            c = normal.z;
            d = -(reference.x * a + reference.y * b + reference.z * c);
        }



        /// <summary>
        /// 判断点是否在平面范围内, eps??
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool Contain(Vector3 p)
        {
            return Mathf.Abs(a * p.x + b * p.y + c * p.z + d) <= 1e-7f;
        }

        public virtual bool Contain(Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            float t = (-d - a * from.x - b * from.y - c * from.z) / (a * dir.x + b * dir.y + c * dir.z);
            Vector3 p = from + t * dir;
            return Contain(p);
        }

        /// <summary>
        /// 判断点在平面的上方还是下方，上方返回1，在平面上返回0
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetDirection(Vector3 p)
        {
            float res = a * p.x + b * p.y + c * p.z + d;
            if (res > 0) return 1;
            else if (res < 0) return -1;
            return 0;
        }

        /// <summary>
        /// 是否有交点，不包含端点
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool HavIntersection(Line line)
        {
            if (GetDirection(line.from.position) * GetDirection(line.to.position) >= 0) return false;
            return true;
        }

        /// <summary>
        /// 是否有交点，不包含端点
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool HavIntersection(Point from, Point to)
        {
            if (GetDirection(from.position) * GetDirection(to.position) >= 0) return false;
            return true;
        }

        ///<summary>
        /// 求平面与直线的交点
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Point GetIntersection(Line line)
        {
            Vector3 dir = line.to.position - line.from.position;
            float t = (-d - a * line.from.position.x - b * line.from.position.y - c * line.from.position.z) / (a * dir.x + b * dir.y + c * dir.z);
            Point intersection = Point.Interpolate(line.from, 1.0f - t, line.to, t);
            return intersection;
        }
        #endregion
    }

    /// <summary>
    /// 扇形面
    /// </summary>
    class SectorPlane : Plane
    {
        public float radio;
        public Vector3 center;
        public Vector3 fromDirection;
        public Vector3 toDirection;

        /// <summary>
        /// 在扇形面内
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Contain(Vector3 p)
        {
            Vector3 direction = p - center;
            if (direction.magnitude > radio) return false;
            if (Vector3.Dot(Vector3.Cross(direction, fromDirection), Vector3.Cross(direction, toDirection)) > 0) return false;
            return true;
        }

        public override bool Contain(Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            float t = (-d - a * from.x - b * from.y - c * from.z) / (a * dir.x + b * dir.y + c * dir.z);
            Vector3 p = from + t * dir;
            return Contain(p);
        }
    }
}
