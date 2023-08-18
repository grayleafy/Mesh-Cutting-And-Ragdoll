using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSU;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

///submesh排序还未测试

namespace Slice
{

    class Slicer
    {
        public static List<Mesh> Slice(Mesh mesh, Plane plane)
        {

            if (mesh.GetTopology(0) != MeshTopology.Triangles)
            {
                Debug.Log("mesh is not triangles");
                return null;
            }

            //一些网格数据
            int vertexCount = mesh.vertexCount;
            int subMeshCount = mesh.subMeshCount;


            //初始化并查集
            DSU<int> dsu;
            {
                int[] temp = new int[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    temp[i] = i;
                }
                dsu = new DSU<int>(temp);
            }

            //构造points         
            List<Point> points = new();
            for (int i = 0; i < vertexCount; i++)
            {
                points.Add(new Point(mesh, i));
                points[i].order = i;
            }

            //合并坐标几乎一致的点
            points.Sort(new PointComparerByPos1());
            for (int i = 0; i + 1 < points.Count; i++)
            {
                if ((points[i].position - points[i + 1].position).magnitude <= 1e-8f) dsu.Union(points[i].order, points[i + 1].order);
            }
            //避免两点在划分线附近
            points.Sort(new PointComparerByPos2());
            for (int i = 0; i + 1 < points.Count; i++)
            {
                if ((points[i].position - points[i + 1].position).magnitude <= 1e-8f) dsu.Union(points[i].order, points[i + 1].order);
            }
            //恢复原来的顺序
            points.Sort(new PointComparerByOrder());

#if DEBUG
            for (int i = 0; i < vertexCount; i++) dsu.FindParent(i);
#endif



            //求切割面的点
            Dictionary<Point, int> cutPoints = new Dictionary<Point, int>(); //索引从0开始
            Dictionary<int, int> edge = new Dictionary<int, int>(); //多边形的边,索引从0开始, 上下每个点共享一份
            List<Triangle> triangles = new List<Triangle>();
            for (int submeshIndice = 0; submeshIndice < subMeshCount; submeshIndice++)
            {
                int[] indices = mesh.GetIndices(submeshIndice);
                for (int i = 0; i < indices.Length; i += 3)
                {
                    Triangle triangle = new Triangle();
                    triangle.submeshIndice = submeshIndice;
                    triangle.indices[0] = indices[i];
                    triangle.indices[1] = indices[i + 1];
                    triangle.indices[2] = indices[i + 2];

                    triangle.Intersect(plane, points, cutPoints, triangles, dsu, edge);
                }
            }

            if (cutPoints.Count == 0)
            {
                return new List<Mesh> { mesh };
            }

            //处理cutPoints
            Point[] cutPointsList = new Point[cutPoints.Count];
            foreach (Point point in cutPoints.Keys)
            {
                cutPointsList[cutPoints[point]] = point;
            }

            //在切面上连接位置几乎一致的点,如果有多个长度几乎为0的边？   
            {
                int length = edge.Count;
                int[] used = new int[length];
                for (int i = 0; i < used.Length; i++) { used[i] = 0; }
                Dictionary<int, int> temp = new();
                for (int i = 0; i < length; i++)
                {
                    int from = edge.ElementAt(i).Value;
                    for (int j = 0; j < length; j++)
                    {
                        if (i == j) continue;
                        if (used[j] == 1) continue;
                        if ((cutPointsList[edge.ElementAt(i).Value].position - cutPointsList[edge.ElementAt(j).Key].position).magnitude <= 1e-7f)
                        {
                            temp.Add(from, edge.ElementAt(j).Key);
                            used[j] = 1;
                            break;
                        }
                    }
                }
                foreach (int key in temp.Keys)
                {
                    edge.Add(key, temp[key]);
                }
            }

            //当有多个切面，保留点数最多的, 
            List<int> polygonIndices = null;
            while (edge.Count > 0)
            {
                List<int> tempPolygon = new List<int>();
                int from = edge.ElementAt(0).Key;
                int to = edge[from];
                tempPolygon.Add(from);
                edge.Remove(from);

                while (to != tempPolygon[0])
                {
                    from = to;
                    tempPolygon.Add(from);

                    if (!edge.ContainsKey(from)) break;
                    to = edge[from];
                    edge.Remove(from); ;
                }

                if (polygonIndices == null || polygonIndices.Count < tempPolygon.Count)
                {
                    polygonIndices = tempPolygon;
                }
                else
                {
                    for (int i = 0; i < tempPolygon.Count; i++)
                    {
                        //dsu.Union(tempPolygon[i] * 2 + points.Count, tempPolygon[i] * 2 + 1 + points.Count); //只保留最大的切口
                    }
                }

                //if (polygonIndices.Count > edge.Count) break;
            }
            //HashSet<int> polygonEdge = new HashSet<int>();
            //foreach (int i in polygonIndices)
            //{
            //    polygonEdge.Add(i);
            //}



            //重新连接不是主切面的并查集,容易有问题
            //for (int i = 0; i < cutPointsList.Length; i++)
            //{
            //    if (!polygonEdge.Contains(i) && )
            //    {
            //        //dsu.Union(i * 2 + points.Count, i * 2 + 1 + points.Count);
            //    }
            //}

            //构造上下网络的顶点,先不加公共切面点
            List<Point> upPoints = new List<Point>();
            List<Point> downPoints = new List<Point>();
            Dictionary<int, int> upDir = new Dictionary<int, int>();
            Dictionary<int, int> downDir = new Dictionary<int, int>();
            for (int i = 0; i < points.Count; i++)
            {
                if (dsu.IsSame(i, points.Count))
                {
                    upPoints.Add(points[i]);
                    upDir[i] = upPoints.Count - 1;
                }
                else
                {
                    downPoints.Add(points[i]);
                    downDir[i] = downPoints.Count - 1;
                }
            }


            //区分三角形
            List<Triangle> upTriangles = new List<Triangle>();
            List<Triangle> downTriangles = new List<Triangle>();
            for (int i = 0; i < triangles.Count; i++)
            {
                int upOrDown = 0;
                if (dsu.IsSame(triangles[i].indices[0], points.Count)) upOrDown = 1;
                else upOrDown = -1;

                if (upOrDown == 1)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (triangles[i].indices[j] >= points.Count)
                        {
                            triangles[i].indices[j] = (triangles[i].indices[j] - points.Count) / 2 + upPoints.Count;
                        }
                        else
                        {
                            triangles[i].indices[j] = upDir[triangles[i].indices[j]];
                        }
                    }
                    upTriangles.Add(triangles[i]);
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (triangles[i].indices[j] >= points.Count)
                        {
                            triangles[i].indices[j] = (triangles[i].indices[j] - points.Count) / 2 + downPoints.Count;
                        }
                        else
                        {
                            triangles[i].indices[j] = downDir[triangles[i].indices[j]];
                        }
                    }
                    downTriangles.Add(triangles[i]);
                }
            }

            //加入公共切面点
            for (int i = 0; i < cutPointsList.Length; i++)
            {
                upPoints.Add(cutPointsList[i]);
                downPoints.Add(cutPointsList[i]);
            }


            //按索引排序
            {
                List<Point> temp = new();
                for (int i = 0; i < polygonIndices.Count; i++)
                {
                    temp.Add(cutPointsList[polygonIndices[i]]);
                }
                //剔除位置相近的切面点
                List<Point> temp2 = new();
                for (int i = 0; i < temp.Count; i++)
                {
                    while (i < temp.Count && (temp[i].position - temp[(i + 1) % temp.Count].position).magnitude <= 1e-7f)
                    {
                        i++;
                    }
                    if (i < temp.Count) temp2.Add(temp[i]);
                }
                cutPointsList = temp2.ToArray();
            }




            //切面三角形
            {
                //三角形按子网格排序
                upTriangles.Sort(new TriangleComparerBySubmesh());
                downTriangles.Sort(new TriangleComparerBySubmesh());
                int upSubmeshCount = mesh.subMeshCount;
                int downSubmeshCount = mesh.subMeshCount;
                int upPointCount = upPoints.Count;
                int downPointCount = downPoints.Count;
                Polygon polygon = new Polygon(cutPointsList, plane.normal);
                List<Triangle> cutTriangles = polygon.Triangulate();
                for (int i = 0; i < cutTriangles.Count; i++)
                {
                    Triangle temp1 = new Triangle();
                    temp1.indices[0] = cutTriangles[i].indices[0] + upPointCount;
                    temp1.indices[1] = cutTriangles[i].indices[1] + upPointCount;
                    temp1.indices[2] = cutTriangles[i].indices[2] + upPointCount;
                    temp1.submeshIndice = upSubmeshCount;
                    upTriangles.Add(temp1);
                    Triangle temp2 = new Triangle();
                    temp2.indices[0] = cutTriangles[i].indices[2] + downPointCount;
                    temp2.indices[1] = cutTriangles[i].indices[1] + downPointCount;
                    temp2.indices[2] = cutTriangles[i].indices[0] + downPointCount;
                    temp2.submeshIndice = downSubmeshCount;
                    downTriangles.Add(temp2);
                }
            }
            //切面点
            for (int i = 0; i < cutPointsList.Length; i++)
            {
                Point temp1 = new();
                temp1.position = cutPointsList[i].position;
                temp1.boneWeight = cutPointsList[i].boneWeight;
                temp1.normal = -plane.normal;
                upPoints.Add(temp1);
                Point temp2 = new();
                temp2.position = cutPointsList[i].position;
                temp2.boneWeight = cutPointsList[i].boneWeight;
                temp2.normal = -plane.normal;
                downPoints.Add(temp2);
            }




            //从point加载到mesh
            Mesh upMesh = new Mesh();
            Mesh downMesh = new Mesh();
            //顶点坐标
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Position))
            {
                List<Vector3> upVertices = new List<Vector3>();
                foreach (Point p in upPoints)
                {
                    upVertices.Add(p.position);
                }
                upMesh.SetVertices(upVertices);

                List<Vector3> downVertices = new List<Vector3>();
                foreach (Point p in downPoints)
                {
                    downVertices.Add(p.position);
                }
                downMesh.SetVertices(downVertices);
            }
            //法线
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Normal))
            {
                List<Vector3> upNormals = new List<Vector3>();
                foreach (Point p in upPoints)
                {
                    upNormals.Add(p.normal);
                }
                upMesh.SetNormals(upNormals);

                List<Vector3> downNormals = new List<Vector3>();
                foreach (Point p in downPoints)
                {
                    downNormals.Add(p.normal);
                }
                downMesh.SetNormals(downNormals);
            }
            //切线
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Tangent))
            {
                List<Vector4> upTangents = new List<Vector4>();
                foreach (Point p in upPoints)
                {
                    upTangents.Add(p.tangent);
                }
                upMesh.SetTangents(upTangents);

                List<Vector4> downTangents = new List<Vector4>();
                foreach (Point p in downPoints)
                {
                    upTangents.Add(p.tangent);
                }
                downMesh.SetTangents(downTangents);
            }
            //颜色
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Color))
            {
                List<Color> upColors = new List<Color>();
                foreach (Point p in upPoints)
                {
                    upColors.Add(p.color);
                }
                upMesh.SetColors(upColors);

                List<Color> downColors = new List<Color>();
                foreach (Point p in downPoints)
                {
                    upColors.Add(p.color);
                }
                downMesh.SetColors(downColors);
            }
            //纹理坐标
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord0))
            {
                List<Vector2> upUVs = new();
                foreach (Point p in upPoints)
                {
                    upUVs.Add(p.uv);
                }
                upMesh.SetUVs(0, upUVs);

                List<Vector2> downUVs = new();
                foreach (Point p in downPoints)
                {
                    upUVs.Add(p.uv);
                }
                downMesh.SetUVs(0, downUVs);
            }
            //骨骼混合
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.BlendIndices))
            {
                List<BoneWeight> upWeights = new();
                foreach (Point p in upPoints)
                {
                    upWeights.Add(p.boneWeight);
                }
                upMesh.boneWeights = upWeights.ToArray();

                List<BoneWeight> downWeights = new();
                foreach (Point p in downPoints)
                {
                    downWeights.Add(p.boneWeight);
                }
                downMesh.boneWeights = downWeights.ToArray();
            }

            //骨骼，索引类型等等
            upMesh.bindposes = mesh.bindposes;
            downMesh.bindposes = mesh.bindposes;

            //三角形按子网格排序
            upTriangles.Sort(new TriangleComparerBySubmesh());
            downTriangles.Sort(new TriangleComparerBySubmesh());

            //建立索引
            upMesh.subMeshCount = mesh.subMeshCount + 1;
            int upSubmesh = 0;
            for (int i = 0; i < upTriangles.Count; i++)
            {
                List<int> upIndices = new List<int>();
                for (; i < upTriangles.Count; i++)
                {
                    upIndices.Add(upTriangles[i].indices[0]);
                    upIndices.Add(upTriangles[i].indices[1]);
                    upIndices.Add(upTriangles[i].indices[2]);
                    upSubmesh = upTriangles[i].submeshIndice;
                    if (i + 1 >= upTriangles.Count || upTriangles[i].submeshIndice != upTriangles[i + 1].submeshIndice) break;
                }
                upMesh.SetIndices(upIndices, MeshTopology.Triangles, upSubmesh);
            }
            downMesh.subMeshCount = mesh.subMeshCount + 1;
            int downSubmesh = 0;
            for (int i = 0; i < downTriangles.Count; i++)
            {
                List<int> downIndices = new List<int>();
                for (; i < downTriangles.Count; i++)
                {
                    downIndices.Add(downTriangles[i].indices[0]);
                    downIndices.Add(downTriangles[i].indices[1]);
                    downIndices.Add(downTriangles[i].indices[2]);
                    downSubmesh = downTriangles[i].submeshIndice;
                    if (i + 1 >= downTriangles.Count || downTriangles[i].submeshIndice != downTriangles[i + 1].submeshIndice) break;
                }
                downMesh.SetIndices(downIndices, MeshTopology.Triangles, downSubmesh);
            }

            //bound
            upMesh.RecalculateBounds();
            downMesh.RecalculateBounds();

            //保存
            upMesh.name = "upMesh";
            AssetDatabase.CreateAsset(upMesh, "Assets/" + upMesh.name + ".asset");
            AssetDatabase.SaveAssets();


            List<Mesh> res = new List<Mesh>
            {
                upMesh, downMesh
            };


            return res;
        }
    }
}
