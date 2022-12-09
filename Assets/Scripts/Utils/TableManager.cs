using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public class TableManager : IManagerBase
    {
        protected static TableManager instance = null;
        public static TableManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new TableManager
                    {
                        tables = new Dictionary<Type, ITableBase>()
                    };

                    #region 테이블 세팅
                    #endregion
                }
                return instance;
            }
        }
        private Dictionary<Type, ITableBase> tables = null;
        public void Init()
        {
            if(tables == null)
            {
                return;
            }

            var it = tables.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.Init();
            }
        }
        public void Clear()
        {
            if (tables == null)
            {
                return;
            }

            var it = tables.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Value.DataClear();
            }
        }

        public bool AddTable(Type type, ITableBase target)
        {
            if (tables == null)
            {
                return false;
            }

            if (tables.ContainsKey(type)) //매니저 중복
            {
                return false;
            }

            tables.Add(type, target);
            return true;
        }

        public static T GetTable<T>() where T : class, ITableBase
        {
            if(Instance.tables == null)
            {
                return null;
            }
            var type = typeof(T);
            if(Instance.tables.ContainsKey(type) && Instance.tables[type] is T)
            {
                return Instance.tables[type] as T;
            }
            return null;
        }
        public void Update(float dt) {}
    }
}
