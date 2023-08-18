using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Slice
{
    public class Point
    {
        #region attributes
        public Vector3 position;
        public Vector3 normal;
        /// <summary>切线的插值，w只为1或-1，与图形接口平台有关</summary>
        public Vector4 tangent;
        public Color color;
        ///<summary>默认只处理第一个纹理坐标</summary>
        public Vector2 uv;
        public BoneWeight boneWeight;

        public int order;

        #endregion

        #region operator
        public static Point operator *(float a, Point p)
        {
            Point res = new Point();
            res.position = a * p.position;
            res.normal = a * p.normal;
            res.tangent = a * p.tangent;
            res.color = a * p.color;
            res.uv = a * p.uv;
            //res.boneWeight.weight0 = a * p.boneWeight.weight0;
            //res.boneWeight.weight1 = a * p.boneWeight.weight1;
            //res.boneWeight.weight2 = a * p.boneWeight.weight2;
            //res.boneWeight.weight3 = a * p.boneWeight.weight3;
            //res.boneWeight.boneIndex0 = p.boneWeight.boneIndex0;
            //res.boneWeight.boneIndex1 = p.boneWeight.boneIndex1;
            //res.boneWeight.boneIndex2 = p.boneWeight.boneIndex2;
            //res.boneWeight.boneIndex3 = p.boneWeight.boneIndex3;
            return res;
        }

        public static Point operator +(Point p1, Point p2)
        {
            Point res = new Point();
            res.position = p1.position + p2.position;
            res.normal = p1.normal + p2.normal;
            res.tangent = p1.tangent + p2.tangent;
            res.color = p1.color + p2.color;
            res.uv = p1.uv + p2.uv;
            return res;
        }
        #endregion

        #region functions

        public Point() { }

        /// <summary>
        /// 根据网格和索引初始化一个point，每次访问都会创建新数组？
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="indice"></param>
        public Point(Mesh mesh, int indice)
        {
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Position)) position = mesh.vertices[indice];
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Normal)) normal = mesh.normals[indice];
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Tangent)) tangent = mesh.tangents[indice];
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Color)) color = mesh.colors[indice];
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord0)) uv = mesh.uv[indice];
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.BlendIndices)) boneWeight = mesh.boneWeights[indice];    //只有索引的顶点默认权重为1
        }

        //public static Point[] initPoints(Mesh mesh)
        //{
        //    int n = mesh.vertices.Length;
        //    Point[] points = new Point[n];


        //}


        /// <summary>
        /// 插值求交点,骨骼只保留四个权重最高的
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcRate"></param>
        /// <param name="dest"></param>
        /// <param name="destRate"></param>
        /// <returns></returns>
        static public Point Interpolate(Point src, float srcRate, Point dest, float destRate)
        {
            Point res = srcRate * src + destRate * dest;
            if (src.boneWeight.Equals(default) || dest.boneWeight.Equals(default)) return res;
            //骨骼部分单独处理,权重相加后取最高的四个，然后归一化
            Dictionary<int, float> boneIndiceWeights = new Dictionary<int, float>();
            //src
            if (boneIndiceWeights.ContainsKey(src.boneWeight.boneIndex0))
            {
                boneIndiceWeights[src.boneWeight.boneIndex0] += src.boneWeight.weight0 * srcRate;
            }
            else
            {
                boneIndiceWeights[src.boneWeight.boneIndex0] = src.boneWeight.weight0 * srcRate;
            }

            if (boneIndiceWeights.ContainsKey(src.boneWeight.boneIndex1))
            {
                boneIndiceWeights[src.boneWeight.boneIndex1] += src.boneWeight.weight1 * srcRate;
            }
            else
            {
                boneIndiceWeights[src.boneWeight.boneIndex1] = src.boneWeight.weight1 * srcRate;
            }

            if (boneIndiceWeights.ContainsKey(src.boneWeight.boneIndex2))
            {
                boneIndiceWeights[src.boneWeight.boneIndex2] += src.boneWeight.weight2 * srcRate;
            }
            else
            {
                boneIndiceWeights[src.boneWeight.boneIndex2] = src.boneWeight.weight2 * srcRate;
            }

            if (boneIndiceWeights.ContainsKey(src.boneWeight.boneIndex3))
            {
                boneIndiceWeights[src.boneWeight.boneIndex3] += src.boneWeight.weight3 * srcRate;
            }
            else
            {
                boneIndiceWeights[src.boneWeight.boneIndex3] = src.boneWeight.weight3 * srcRate;
            }
            //dest
            if (boneIndiceWeights.ContainsKey(dest.boneWeight.boneIndex0))
            {
                boneIndiceWeights[dest.boneWeight.boneIndex0] += dest.boneWeight.weight0 * destRate;
            }
            else
            {
                boneIndiceWeights[dest.boneWeight.boneIndex0] = dest.boneWeight.weight0 * destRate;
            }

            if (boneIndiceWeights.ContainsKey(dest.boneWeight.boneIndex1))
            {
                boneIndiceWeights[dest.boneWeight.boneIndex1] += dest.boneWeight.weight1 * destRate;
            }
            else
            {
                boneIndiceWeights[dest.boneWeight.boneIndex1] = dest.boneWeight.weight1 * destRate;
            }

            if (boneIndiceWeights.ContainsKey(dest.boneWeight.boneIndex2))
            {
                boneIndiceWeights[dest.boneWeight.boneIndex2] += dest.boneWeight.weight2 * destRate;
            }
            else
            {
                boneIndiceWeights[dest.boneWeight.boneIndex2] = dest.boneWeight.weight2 * destRate;
            }

            if (boneIndiceWeights.ContainsKey(dest.boneWeight.boneIndex3))
            {
                boneIndiceWeights[dest.boneWeight.boneIndex3] += dest.boneWeight.weight3 * destRate;
            }
            else
            {
                boneIndiceWeights[dest.boneWeight.boneIndex3] = dest.boneWeight.weight3 * destRate;
            }

            //保留四个最大的
            List<int> maxBoneIndices = new List<int>();
            List<float> maxWeights = new List<float>();
            foreach (int k in boneIndiceWeights.Keys)
            {
                if (maxBoneIndices.Count < 4)
                {
                    maxBoneIndices.Add(k);
                    maxWeights.Add(boneIndiceWeights[k]);
                }
                else
                {
                    if (boneIndiceWeights[k] > maxWeights[3])
                    {
                        maxBoneIndices[3] = k;
                        maxWeights[3] = boneIndiceWeights[k];
                        for (int i = 3; i > 0; i--)
                        {
                            if (maxWeights[i] > maxBoneIndices[i - 1])
                            {
                                var tempWeight = maxWeights[i];
                                maxWeights[i] = maxWeights[i - 1];
                                maxWeights[i - 1] = tempWeight;
                                var tempIndice = maxBoneIndices[i];
                                maxBoneIndices[i] = maxBoneIndices[i - 1];
                                maxBoneIndices[i - 1] = tempIndice;
                            }
                        }
                    }
                }
            }

            //归一化
            float sumOfSquare = 0;
            for (int i = 0; i < maxWeights.Count; i++)
            {
                sumOfSquare += maxWeights[i];
            }
            for (int i = 0; i < maxWeights.Count; i++)
            {
                maxWeights[i] /= sumOfSquare;
            }

            if (maxBoneIndices.Count > 0)
            {
                res.boneWeight.boneIndex0 = maxBoneIndices[0];
                res.boneWeight.weight0 = maxWeights[0];
            }
            else
            {
                res.boneWeight.boneIndex0 = 0;
                res.boneWeight.weight0 = 0;
            }
            if (maxBoneIndices.Count > 1)
            {
                res.boneWeight.boneIndex1 = maxBoneIndices[1];
                res.boneWeight.weight1 = maxWeights[1];
            }
            else
            {
                res.boneWeight.boneIndex1 = 0;
                res.boneWeight.weight1 = 0;
            }
            if (maxBoneIndices.Count > 2)
            {
                res.boneWeight.boneIndex2 = maxBoneIndices[2];
                res.boneWeight.weight2 = maxWeights[2];
            }
            else
            {
                res.boneWeight.boneIndex2 = 0;
                res.boneWeight.weight2 = 0;
            }
            if (maxBoneIndices.Count > 3)
            {
                res.boneWeight.boneIndex3 = maxBoneIndices[3];
                res.boneWeight.weight3 = maxWeights[3];
            }
            else
            {
                res.boneWeight.boneIndex3 = 0;
                res.boneWeight.weight3 = 0;
            }




            return res;
        }
        #endregion
    }




    /// <summary>
    /// 比较函数,按照中心点排序，sort是升序排序，则得到顺时针结果
    /// </summary>
    public class PointComparerByCenter : IComparer<Point>
    {
        private Vector3 normal;
        private Vector3 center;
        private Vector3 from;

        /// <summary>
        /// 传入中心点，法线，逆时针起点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="from"></param>
        /// <param name="normal"></param>
        public PointComparerByCenter(Vector3 center, Vector3 normal, Vector3 from)
        {
            this.center = center;
            this.normal = normal;
            this.from = from - center;
        }

        // Call CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(Point x, Point y)
        {
            Point A = (Point)x;
            Point B = (Point)y;
            Vector3 a = A.position;
            Vector3 b = B.position;
            a = (a - center).normalized;
            b = (b - center).normalized;
            float aCrossFrom = Vector3.Dot(Vector3.Cross(from, a), normal);
            float bCrossFrom = Vector3.Dot(Vector3.Cross(from, b), normal);
            //默认不会与中心点重叠
            if (aCrossFrom > 0 && bCrossFrom > 0)
            {
                return -Vector3.Dot(a, from).CompareTo(Vector3.Dot(b, from));
            }
            else if (aCrossFrom < 0 && bCrossFrom < 0)
            {
                return Vector3.Dot(a, from).CompareTo(Vector3.Dot(b, from));
            }
            else
            {
                return -aCrossFrom.CompareTo(bCrossFrom);
            }
        }
    }

    /// <summary>
    /// 按照坐标排序，根据精度划分空间
    /// </summary>
    public class PointComparerByPos1 : IComparer<Point>
    {
        float eps = 1e-8f;
        int IComparer<Point>.Compare(Point a, Point b)
        {
            float ax = a.position.x - a.position.x % eps;
            float ay = a.position.y - a.position.y % eps;
            float az = a.position.z - a.position.z % eps;
            float bx = b.position.x - b.position.x % eps;
            float by = b.position.y - b.position.y % eps;
            float bz = b.position.z + b.position.z % eps;
            if (ax > bx) return 1;
            else if (ax < bx) return -1;

            if (ay > by) return 1;
            else if (ay < by) return -1;

            if (az > bz) return 1;
            else if (az < bz) return -1;

            return 0;
        }
    }
    /// <summary>
    /// 按照坐标排序，根据精度划分空间, 0.5补充区间
    /// </summary>
    public class PointComparerByPos2 : IComparer<Point>
    {
        float eps = 1e-8f;
        float halfEps;
        public PointComparerByPos2()
        {
            halfEps = eps / 2.0f;
        }
        int IComparer<Point>.Compare(Point a, Point b)
        {

            float ax = a.position.x + halfEps;
            float ay = a.position.y + halfEps;
            float az = a.position.z + halfEps;
            float bx = b.position.x + halfEps;
            float by = b.position.y + halfEps;
            float bz = b.position.z + halfEps;
            ax = ax - ax % eps;
            ay = ay - ay % eps;
            az = az - az % eps;
            bx = bx - bx % eps;
            by = by - by % eps;
            bz = bz - bz % eps;
            if (ax > bx) return 1;
            else if (ax < bx) return -1;

            if (ay > by) return 1;
            else if (ay < by) return -1;

            if (az > bz) return 1;
            else if (az < bz) return -1;

            return 0;
        }
    }


    public class PointComparerByOrder : IComparer<Point>
    {
        int IComparer<Point>.Compare(Point x, Point y)
        {
            return x.order - y.order;
        }
    }
}

