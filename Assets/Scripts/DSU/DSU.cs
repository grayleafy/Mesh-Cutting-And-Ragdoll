using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace DSU
{
    /// <summary>
    /// 并查集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DSU<T>
    {
        Dictionary<T, T> par = new Dictionary<T, T>();
        Dictionary<T, int> rank = new Dictionary<T, int>();

        public DSU() { }
        public DSU(T[] input)
        {
            foreach (T t in input)
            {
                par.Add(t, t);
                rank.Add(t, 1);
            }
        }
        public DSU(List<T> input)
        {
            foreach (T t in input)
            {
                par.Add(t, t);
                rank.Add(t, 1);
            }
        }



        /// <summary>
        /// 在并查集中加入元素
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            Assert.IsFalse(par.ContainsKey(element));
            par.Add(element, element);
            rank.Add(element, 1);
        }
        /// <summary>
        /// 加入元素
        /// </summary>
        /// <param name="elements"></param>
        public void Add(T[] elements)
        {
            foreach (var item in elements)
            {
                Assert.IsFalse(par.ContainsKey(item));
                par.Add(item, item);
                rank.Add(item, 1);
            }
        }

        /// <summary>
        /// 返回父亲结点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public T FindParent(T element)
        {
            if (element.Equals(par[element]))
            {
                return element;
            }
            return par[element] = FindParent(par[element]);
        }

        /// <summary>
        /// a和b是否在同一个集合内
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsSame(T a, T b)
        {
            return FindParent(a).Equals(FindParent(b));
        }

        /// <summary>
        /// 合并a和b所在的集合
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Union(T a, T b)
        {
            a = FindParent(a);
            b = FindParent(b);
            if (a.Equals(b)) return;
            if (rank[a] < rank[b])
            {
                par[a] = b;
            }
            else if (rank[a] > rank[b])
            {
                par[b] = a;
            }
            else
            {
                par[b] = a;
                rank[a]++;
            }
        }
    }
}


