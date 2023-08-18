using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSU;

namespace Slice
{
    public class Triangle
    {
        public int[] indices = new int[3];
        public int submeshIndice;

        /// <summary>
        /// 求三角形和平面的交叉，并且将分割后的三角形加入列表，将交叉点加入列表,切面点的索引为网格顶点数加2 * i（上)或网格顶点数加2 * i（下）,当平面在三角形中，会缺失一些分割网格
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="points"></param>
        /// <param name="cutPoints"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        public bool Intersect(Plane plane, List<Point> points, Dictionary<Point, int> cutPoints, List<Triangle> triangles, DSU<int> dsu, Dictionary<int, int> edge)
        {
            //如果与无线平面交但是与范围平面不相交，则直接返回
            bool flag = false;
            for (int i = 0; i < 3; i++)
            {
                if (plane.HavIntersection(points[indices[i]], points[indices[(i + 1) % 3]]))
                {
                    if (plane.Contain(points[indices[i]].position, points[indices[(i + 1) % 3]].position))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                if (plane.GetDirection(points[indices[i]].position) == 0)
                {
                    if (plane.Contain(points[indices[i]].position))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
            {
                for (int i = 0; i < 3; i++)
                {
                    dsu.Union(indices[i], indices[(i + 1) % 3]);
                }
                triangles.Add(this);
                return false;
            }

            //分割三角形
            Stack<int> up = new Stack<int>();
            Stack<int> down = new Stack<int>();
            for (int i = 0; i < 3; i++)
            {
                //不在分界线上
                if (plane.GetDirection(points[indices[i]].position) > 0)
                {
                    up.Push(indices[i]);
                }
                else if (plane.GetDirection(points[indices[i]].position) < 0)
                {
                    down.Push(indices[i]);
                }
                //当前点在分界线上
                else if (plane.GetDirection(points[indices[i]].position) == 0)
                {

                    int id;
                    if (cutPoints.ContainsKey(points[indices[i]])) id = cutPoints[points[indices[i]]];
                    else
                    {
                        cutPoints.Add(points[indices[i]], cutPoints.Count);
                        id = cutPoints[points[indices[i]]];
                        dsu.Add(id * 2 + points.Count);
                        dsu.Add(id * 2 + 1 + points.Count);
                    }

                    up.Push(id * 2 + points.Count);
                    down.Push(id * 2 + 1 + points.Count);
                }


                if (up.Count >= 3)
                {
                    Triangle triangle = new Triangle();
                    triangle.submeshIndice = submeshIndice;
                    triangle.indices[2] = up.Pop();
                    triangle.indices[1] = up.Pop();
                    triangle.indices[0] = up.Pop();
                    up.Push(triangle.indices[0]);
                    up.Push(triangle.indices[2]);

                    //维护多边形
                    if (triangle.indices[0] >= points.Count && triangle.indices[1] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[2]].position) > 0)
                        {
                            edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                        }
                    }
                    if (triangle.indices[0] >= points.Count && triangle.indices[2] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[1]].position) > 0)
                        {
                            edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                        }
                    }
                    if (triangle.indices[1] >= points.Count && triangle.indices[2] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[0]].position) > 0)
                        {
                            edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                        }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        dsu.Union(triangle.indices[j], triangle.indices[(j + 1) % 3]);
                    }
                    triangles.Add(triangle);
                }
                if (down.Count >= 3)
                {
                    Triangle triangle = new Triangle();
                    triangle.submeshIndice = submeshIndice;
                    triangle.indices[2] = down.Pop();
                    triangle.indices[1] = down.Pop();
                    triangle.indices[0] = down.Pop();
                    down.Push(triangle.indices[0]);
                    down.Push(triangle.indices[2]);

                    //维护多边形
                    if (triangle.indices[0] >= points.Count && triangle.indices[1] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[2]].position) > 0)
                        {
                            edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                        }
                    }
                    if (triangle.indices[0] >= points.Count && triangle.indices[2] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[1]].position) > 0)
                        {
                            edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                        }
                    }
                    if (triangle.indices[1] >= points.Count && triangle.indices[2] >= points.Count)
                    {
                        if (plane.GetDirection(points[triangle.indices[0]].position) > 0)
                        {
                            edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                        }
                        else
                        {
                            edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                        }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        dsu.Union(triangle.indices[j], triangle.indices[(j + 1) % 3]);
                    }
                    triangles.Add(triangle);
                }

                //线段有交点
                if (plane.HavIntersection(points[indices[i]], points[indices[(i + 1) % 3]]))
                {
                    Line line = new Line();
                    line.from = points[indices[i]];
                    line.to = points[indices[(i + 1) % 3]];
                    Point interPoint = plane.GetIntersection(line);

                    int id;
                    if (cutPoints.ContainsKey(interPoint)) id = cutPoints[interPoint];
                    else
                    {
                        cutPoints.Add(interPoint, cutPoints.Count);
                        id = cutPoints[interPoint];
                        dsu.Add(id * 2 + points.Count);
                        dsu.Add(id * 2 + 1 + points.Count);
                    }

                    up.Push(id * 2 + points.Count);
                    down.Push(id * 2 + 1 + points.Count);


                    if (up.Count >= 3)
                    {
                        Triangle triangle = new Triangle();
                        triangle.submeshIndice = submeshIndice;
                        triangle.indices[2] = up.Pop();
                        triangle.indices[1] = up.Pop();
                        triangle.indices[0] = up.Pop();
                        up.Push(triangle.indices[0]);
                        up.Push(triangle.indices[2]);

                        //维护多边形
                        if (triangle.indices[0] >= points.Count && triangle.indices[1] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[2]].position) > 0)
                            {
                                edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                            }
                        }
                        if (triangle.indices[0] >= points.Count && triangle.indices[2] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[1]].position) > 0)
                            {
                                edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                            }
                        }
                        if (triangle.indices[1] >= points.Count && triangle.indices[2] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[0]].position) > 0)
                            {
                                edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                            }
                        }

                        for (int j = 0; j < 3; j++)
                        {
                            dsu.Union(triangle.indices[j], triangle.indices[(j + 1) % 3]);
                        }
                        triangles.Add(triangle);
                    }
                    if (down.Count >= 3)
                    {
                        Triangle triangle = new Triangle();
                        triangle.submeshIndice = submeshIndice;
                        triangle.indices[2] = down.Pop();
                        triangle.indices[1] = down.Pop();
                        triangle.indices[0] = down.Pop();
                        down.Push(triangle.indices[0]);
                        down.Push(triangle.indices[2]);

                        //维护多边形
                        if (triangle.indices[0] >= points.Count && triangle.indices[1] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[2]].position) > 0)
                            {
                                edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                            }
                        }
                        if (triangle.indices[0] >= points.Count && triangle.indices[2] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[1]].position) > 0)
                            {
                                edge[(triangle.indices[0] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[0] - points.Count) / 2;
                            }
                        }
                        if (triangle.indices[1] >= points.Count && triangle.indices[2] >= points.Count)
                        {
                            if (plane.GetDirection(points[triangle.indices[0]].position) > 0)
                            {
                                edge[(triangle.indices[2] - points.Count) / 2] = (triangle.indices[1] - points.Count) / 2;
                            }
                            else
                            {
                                edge[(triangle.indices[1] - points.Count) / 2] = (triangle.indices[2] - points.Count) / 2;
                            }
                        }

                        for (int j = 0; j < 3; j++)
                        {
                            dsu.Union(triangle.indices[j], triangle.indices[(j + 1) % 3]);
                        }
                        triangles.Add(triangle);
                    }
                }
            }

            return true;
        }
    }

    public class TriangleComparerBySubmesh : IComparer<Triangle>
    {
        int IComparer<Triangle>.Compare(Triangle x, Triangle y)
        {
            return x.submeshIndice.CompareTo(y.submeshIndice);
        }
    }

}
