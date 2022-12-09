using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public abstract class MultiTableBase<T> : ITableBase where T : class, ITableData
    {
        public Dictionary<object, List<T>> datas = null;
        public virtual void Init()
        {
            if (datas == null)
                datas = new Dictionary<object, List<T>>();
            else
                datas.Clear();
        }
        public abstract void SetTable(JArray jsonArray);
        public virtual void Localize() { }
        public virtual void DataClear()
        {
            if (datas == null)
                return;

            var it = datas.GetEnumerator();

            while (it.MoveNext())
            {
                if (it.Current.Value == null)
                    continue;

                var vIt = it.Current.Value.GetEnumerator();
                while(vIt.MoveNext())
                {
                    vIt.Current.Init();
                }
            }

            datas.Clear();
        }


        public virtual List<T> Get(object key)
        {
            if (datas.ContainsKey(key))
            {
                return datas[key];
            }
            return null;
        }
        public virtual List<T> Get(int key)
        {
            if (datas.ContainsKey(key))
            {
                return datas[key];
            }
            return null;
        }

        public virtual List<T> Get(string key)
        {
            if (datas.ContainsKey(key))
            {
                return datas[key];
            }
            return null;
        }

        protected virtual bool Add(T data)
        {
            if (datas.ContainsKey(data.GetKey()))
            {
                datas[data.GetKey()].Add(data);
                return true;
            }

            datas.Add(data.GetKey(), new List<T>());
            datas[data.GetKey()].Add(data);
            return true;
        }

        protected virtual bool Add(int index, T data)
        {
            if (datas.ContainsKey(index))
            {
                datas[index].Add(data);
                return true;
            }

            datas.Add(index, new List<T>());
            datas[index].Add(data);
            return true;
        }

        protected virtual bool Add(string stringIndex, T data)
        {
            if (datas.ContainsKey(stringIndex))
            {
                datas[stringIndex].Add(data);
                return true;
            }

            datas.Add(stringIndex, new List<T>());
            datas[stringIndex].Add(data);
            return true;
        }

        protected virtual bool Remove(T data)
        {
            return Remove(data.GetKey());
        }

        protected virtual bool Remove(object index, T data)
        {
            if(datas.ContainsKey(index))
            {
                return datas[index].Remove(data);
            }
            return false;
        }

        protected virtual bool Remove(object _index)
        {
            if (datas.ContainsKey(_index))
            {
                return datas.Remove(_index);
            }
            return false;
        }
    }
}
