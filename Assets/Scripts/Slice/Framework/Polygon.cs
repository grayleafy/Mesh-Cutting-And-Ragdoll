using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Slice
{
    public class Polygon
    {
        public Vector2[] vertices;
        public int[] from, to;
        public int[] indices;

        public Polygon(Point[] points, Vector3 normal)
        {
            vertices = new Vector2[points.Length];
            from = new int[points.Length];
            to = new int[points.Length];
            indices = new int[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
                {
                    vertices[i] = new Vector2(points[i].position.y, points[i].position.z);
                }
                else if (Mathf.Abs(normal.y) > Mathf.Abs(normal.z))
                {
                    vertices[i] = new Vector2(points[i].position.x, points[i].position.z);
                }
                else
                {
                    vertices[i] = new Vector2(points[i].position.x, points[i].position.y);
                }

                from[i] = (i - 1 + points.Length) % points.Length;
                to[i] = (i + 1) % points.Length;
                indices[i] = i;
            }


        }

        public List<Triangle> Triangulate()
        {
            HashSet<int> left = new();
            for (int i = 0; i < vertices.Length; i++) left.Add(i);
            List<Triangle> res = new();
            HashSet<int>[] isCovered = new HashSet<int>[vertices.Length];
            for (int i = 0; i < isCovered.Length; i++)
            {
                isCovered[i] = new HashSet<int>();
            }

            HashSet<int> que = new();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!Cover(i, left, isCovered))
                {
                    que.Add(i);
                }
            }

            while (que.Count >= 3)
            {
                int vi = que.ElementAt(0);
                que.Remove(vi);
                Triangle triangle = new Triangle();
                triangle.indices[0] = indices[from[vi]];
                triangle.indices[1] = indices[vi];
                triangle.indices[2] = indices[to[vi]];
                res.Add(triangle);


                to[from[vi]] = to[vi];
                from[to[vi]] = from[vi];

                left.Remove(vi);

                if (que.Count < 3) break;

                //重新计算耳朵
                foreach (int t in isCovered[vi])
                {
                    if (left.Contains(t) && !Cover(t, left, isCovered))
                    {
                        que.Add(t);
                    }
                }
                if (!Cover(from[vi], left, isCovered))
                {
                    que.Add(from[vi]);
                }
                else
                {
                    que.Remove(from[vi]);
                }
                if (!Cover(to[vi], left, isCovered))
                {
                    que.Add(to[vi]);
                }
                else
                {
                    que.Remove(to[vi]);
                }
            }

            //List<int> resIndices = new();
            //foreach (Triangle t in res)
            //{
            //    resIndices.Add(t.indices[0]);
            //    resIndices.Add(t.indices[1]);
            //    resIndices.Add(t.indices[2]);
            //}
            return res;
        }

        public bool Cover(int p, HashSet<int> left, HashSet<int>[] isCovered)
        {
            Vector2[] tri = new Vector2[3];
            tri[0] = vertices[from[p]];
            tri[1] = vertices[p];
            tri[2] = vertices[to[p]];
            bool flag = false;
            foreach (int vi in left)
            {
                if (vi == from[p] || vi == to[p] || vi == p) continue;
                Vector2 v = vertices[vi];
                float[] side = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    Vector2 a = tri[(i + 1) % 3] - tri[i];
                    Vector2 b = v - tri[i];
                    side[i] = a.x * b.y - a.y * b.x;
                }
                if (side[0] > 0 && side[1] > 0 && side[2] > 0 || side[0] < 0 && side[1] < 0 && side[2] < 0)
                {
                    flag = true;
                    if (!isCovered[vi].Contains(p)) isCovered[vi].Add(p);
                }
            }

            return flag;
        }
    }
}
