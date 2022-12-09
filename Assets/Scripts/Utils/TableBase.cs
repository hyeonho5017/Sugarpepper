using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public abstract class TableBase<T> : ITableBase where T : class, ITableData
    {
        public Dictionary<object, T> datas = null;
        public virtual void Init()
        {
            if (datas == null)
                datas = new Dictionary<object, T>();
            else
                datas.Clear();
        }
        public abstract void SetTable(JArray jsonArray);
        public virtual void Localize() {}
        public virtual void DataClear()
        {
            if (datas == null)
                return;

            var it = datas.GetEnumerator();

            while (it.MoveNext())
            {
                it.Current.Value.Init();
            }

            datas.Clear();
        }

        public virtual T Get(object key)
        {
            if (datas.ContainsKey(key))
            {
                return datas[key];
            }
            return null;
        }
        public virtual T Get(int key)
        {
            if(datas.ContainsKey(key))
            {
                return datas[key];
            }
            return null;
        }
        public virtual T Get(string key)
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
                UnityEngine.Debug.LogError("SBTableBase Error : 중복 키 => " + data.GetKey());
                return false;
            }

            datas.Add(data.GetKey(), data);
            return true;
        }

        protected virtual bool Add(int index, T data)
        {
            if (datas.ContainsKey(index))
            {
                UnityEngine.Debug.LogError("SBTableBase Error : 중복 키 => " + data.GetKey());
                return false;
            }

            datas.Add(index, data);
            return true;
        }

        protected virtual bool Remove(T data)
        {
            return Remove(data.GetKey());
        }

        protected virtual bool Remove(object _index)
        {
            if(datas.ContainsKey(_index))
            {
                return datas.Remove(_index);
            }
            return false;
        }
    }
}
