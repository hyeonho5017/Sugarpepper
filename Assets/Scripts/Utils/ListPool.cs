using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public class ListPool<T> where T : class
    {
        public delegate void SBListPoolReuse(T item);
        public delegate void SBListPoolUnuse(T item);

        protected SBListPoolReuse reuse = null;
        protected SBListPoolUnuse unuse = null;
        protected List<T> datas = null;

        public ListPool()
        {
            reuse = null;
            unuse = null;
        }
        public ListPool(SBListPoolReuse reuse, SBListPoolUnuse unuse)
        {
            this.reuse = reuse;
            this.unuse = unuse;
        }
        public int Count
        {
            get
            {
                if (datas == null)
                    return 0;
                return datas.Count;
            }
        }

        public void Put(T item)
        {
            if (datas == null)
                datas = new List<T>();

            if (!datas.Contains(item))
            {
                unuse?.Invoke(item);
                datas.Add(item);
            }
        }
        public void Clear()
        {
            if (datas == null)
                datas = new List<T>();
            else
                datas.Clear();
        }
        public T Get()
        {
            var last = Count - 1;
            if (last < 0)
                return null;

            // Pop the last object in pool
            var item = datas[last];
            datas.RemoveAt(last);

            reuse?.Invoke(item);

            return item;
        }
    }
}