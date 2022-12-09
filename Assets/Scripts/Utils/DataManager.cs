using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public class DataManager
    {
        protected static DataManager instance = null;
        public static DataManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataManager();
                    instance.datas = new Dictionary<object, object>();
                }
                return instance;
            }
        }
        protected Dictionary<object, object> datas = null;

        public static bool AddData(object key, object value, bool isRemove = false)
        {
            if (Instance.datas.ContainsKey(key))
            {
                if(isRemove)
                {
                    DelData(key);
                }
                else
                {
                    return false;
                }
            }
            Instance.datas.Add(key, value);
            return true;
        }
        public static object GetData(object key) //꺼내면서 캐스팅하여 사용.
        {
            if(Instance.datas.ContainsKey(key))
            {
                return Instance.datas[key];
            }
            return null;
        }
        public static T GetData<T>(object key) where T : class
        {
            if (Instance.datas.ContainsKey(key))
            {
                return Instance.datas[key] as T;
            }
            return null;
        }
        public static bool DelData(object key)
        {
            return Instance.datas.Remove(key);
        }
    }
}
