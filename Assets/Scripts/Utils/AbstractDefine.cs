using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public abstract class Popup<T> : MonoBehaviour, IPopup where T : class
    {
        protected int order = 0;
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        protected T data = null;
        //상속 재구현 금지
        public virtual void Init(object data)
        {
            Init(data as T);
        }
        public virtual void ForceUpdate(object data)
        {
            ForceUpdate(data as T);
        }
        //
        public virtual void Init(T data)
        {
            this.data = data;

            StartCoroutine(OpenAnimation());
        }
        public abstract void InitUI();
        public abstract void ForceUpdate(T data);
        protected virtual IEnumerator OpenAnimation()
        {
            InitUI();
            yield return null;
        }
        protected virtual IEnumerator CloseAnimation()
        {
            Destroy(gameObject);
            yield return null;
        }
        public virtual void Destroy()
        {
            StartCoroutine(CloseAnimation());
        }
        public virtual int GetOrder()
        {
            return Order;
        }
    }
}
