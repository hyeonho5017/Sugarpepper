using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Sugarpepper
{
    public enum eTableViewType
    {
        Vertical,
        Horizental
    }
    public class TableViewDelegate
    {
        public delegate void ReuseDelegate(GameObject itemNode, object item);
        public delegate void UnuseDelegate(GameObject itemNode);

        public List<object> Items = null;
        public ReuseDelegate Reuse = null;
        public UnuseDelegate Unuse = null;

        public TableViewDelegate(List<object> items, ReuseDelegate reuse = null, UnuseDelegate unuse = null)
        {
            Items = items;
            Reuse = reuse;
            Unuse = unuse;
        }

        public void SetDelegate(List<object> items, ReuseDelegate reuse = null, UnuseDelegate unuse = null)
        {
            Items = items;
            Reuse = reuse;
            Unuse = unuse;
        }
    }

    public class TableView : MonoBehaviour
    {
        [Header("Default")]
        [SerializeField]
        protected eTableViewType tableType  = eTableViewType.Vertical;
        [SerializeField]
        protected GameObject itemTemplate = null;
        public GameObject ItemTemplate { get { return itemTemplate; } }
        [Space(10f)]
        [Header("Padding")]
        [SerializeField]
        protected float paddingX = 0f;
        [SerializeField]
        protected float paddingY= 0f;
        [Space(10f)]
        [Header("Spaceing")]
        [SerializeField]
        protected float spaceingX = 0f;
        [SerializeField]
        protected float spaceingY = 0f;
        protected ScrollRect scrollView = null;
        protected float itemWidth = 0f;
        protected float itemHeight = 0f;
        protected ListPool<GameObject> itemPool = null;
        protected TableViewDelegate viewDelegate = null;
        protected List<object> dataSource = null;
        protected float visibleWidth = 0f;
        protected float visibleHeight = 0f;
        protected int spawnCount = 0;
        protected Dictionary<int, GameObject> visibleNodes = null;
        protected float lastX = float.MinValue;
        protected float lastY = float.MinValue;
        protected HashSet<int> itemIndex = null;
        protected Vector3 vec3 = Vector3.zero;
        protected RectTransform contentTransform = null;
        protected float contentScaleX = 1f;
        protected float contentScaleY = 1f;
        protected virtual void OnDestroy()
        {
            if(itemPool != null)
            {
                while(itemPool.Get() != null)
                {
                    continue;
                }
            }
        }
        public virtual void OnStart()
        {
            itemPool = new ListPool<GameObject>((GameObject item) =>
            {
                if (item == null)
                    return;
                if(item.transform.parent != contentTransform)
                {
                    item.transform.SetParent(contentTransform, false);
                    item.transform.localScale = Vector3.one;
                }
                item.SetActive(true);
            }, (GameObject item) =>
            {
                if (item == null)
                    return;

                item.SetActive(false);
                //item.transform.SetParent(null, false);
            });

            if (dataSource == null)
                dataSource = new List<object>();
            if (visibleNodes == null)
                visibleNodes = new Dictionary<int, GameObject>();
            if (itemIndex == null)
                itemIndex = new HashSet<int>();

            scrollView = GetComponent<ScrollRect>();

            if (itemTemplate != null)
            {
                var itemSize = itemTemplate.GetComponent<RectTransform>();
                if (itemSize != null)
                {
                    itemWidth = itemSize.sizeDelta.x;
                    itemHeight = itemSize.sizeDelta.y;
                }
            }
            itemPool.Put(itemTemplate);
            lastX = float.MinValue;
            lastY = float.MinValue;

            if (scrollView != null)
            {
                var scrollViewRect = scrollView.viewport.GetComponent<RectTransform>();
                if (scrollViewRect != null)
                {
                    visibleWidth = scrollViewRect.sizeDelta.x;
                    visibleHeight = scrollViewRect.sizeDelta.y;
                }

                contentScaleX = scrollView.content.localScale.x;
                contentScaleY = scrollView.content.localScale.y;

                contentTransform = scrollView.content;
            }

            switch (tableType)
            {
                case eTableViewType.Vertical:
                {
                    scrollView.vertical = true;
                    scrollView.horizontal = false;
                    contentTransform.anchorMin = new Vector2(0.5f, 1f);
                    contentTransform.anchorMax = new Vector2(0.5f, 1f);
                    contentTransform.pivot = new Vector2(0.5f, 1f);
                    spawnCount = Mathf.RoundToInt(visibleHeight / itemHeight / contentScaleY) + 1;
                }
                break;
                case eTableViewType.Horizental:
                {
                    scrollView.vertical = false;
                    scrollView.horizontal = true;
                    contentTransform.anchorMin = new Vector2(0f, 0.5f);
                    contentTransform.anchorMax = new Vector2(0f, 0.5f);
                    contentTransform.pivot = new Vector2(0f, 0.5f);
                    spawnCount = Mathf.RoundToInt(visibleWidth / itemWidth / contentScaleX) + 1;
                }
                break;
            }

            PoolSpawn(spawnCount);
        }

        protected virtual void LateUpdate()
        {
            if(contentTransform != null) { 
                switch (tableType)
                {
                    case eTableViewType.Vertical:
                    {
                        var y = contentTransform.anchoredPosition.y / contentScaleY;
                        if (lastY != y)
                        {
                            lastY = y;
                            GetVisibleItemIndexVertical(y);

                            var keys = new List<int>(visibleNodes.Keys);
                            var count = keys.Count;
                            for (var i = 0; i < count; ++i)
                            {
                                var idx = keys[i];
                                if (!itemIndex.Contains(idx))
                                {
                                    viewDelegate.Unuse?.Invoke(visibleNodes[idx]);
                                    itemPool.Put(visibleNodes[idx]);
                                    visibleNodes.Remove(idx);
                                }
                            }

                            var it = itemIndex.GetEnumerator();
                            while (it.MoveNext())
                            {
                                IndexChange(it.Current);
                            }
                        }
                    }
                    break;
                    case eTableViewType.Horizental:
                    {
                        var x = contentTransform.anchoredPosition.x / contentScaleX;
                        if (lastX != x)
                        {
                            lastX = x;
                            GetVisibleItemIndexHorizental(x);

                            var keys = new List<int>(visibleNodes.Keys);
                            var count = keys.Count;
                            for (var i = 0; i < count; ++i)
                            {
                                var idx = keys[i];
                                if (!itemIndex.Contains(idx))
                                {
                                    viewDelegate.Unuse?.Invoke(visibleNodes[idx]);
                                    itemPool.Put(visibleNodes[idx]);
                                    visibleNodes.Remove(idx);
                                }
                            }

                            var it = itemIndex.GetEnumerator();
                            while (it.MoveNext())
                            {
                                IndexChange(it.Current);
                            }
                        }
                    }
                    break;
                }
            }
        }
        protected virtual void PoolSpawn(int count)
        {
            while (itemPool.Count < count)
            {
                itemPool.Put(Instantiate(itemTemplate));
            }
        }
        public virtual void SetDelegate(TableViewDelegate viewDelegate)
        {
            this.viewDelegate = viewDelegate;
        }
        public virtual void ReLoad(bool initPos = true)
        {
            dataSource = viewDelegate.Items;
            var size = contentTransform.sizeDelta;
            size.x = GetTotalWidth();
            size.y = GetTotalHeight();
            contentTransform.sizeDelta = size;
            switch (tableType)
            {
                case eTableViewType.Vertical:
                {
                    var children = Func.GetChildren(scrollView.content.gameObject);
                    var cCount = children.Length;
                    for(var i = 0; i < cCount; ++i)
                    {
                        itemPool.Put(children[i]);
                    }

                    //scrollView.content.DetachChildren();
                    visibleNodes.Clear();

                    scrollView.StopMovement();

                    if (initPos)
                    {
                        scrollView.normalizedPosition = new Vector2(0.5f, 1f);
                    }

                    lastY = int.MinValue;
                } break;
                case eTableViewType.Horizental:
                {
                    var children = Func.GetChildren(scrollView.content.gameObject);
                    var cCount = children.Length;
                    for (var i = 0; i < cCount; ++i)
                    {
                        itemPool.Put(children[i]);
                    }

                    //scrollView.content.DetachChildren();
                    visibleNodes.Clear();

                    scrollView.StopMovement();

                    if (initPos)
                    {
                        scrollView.normalizedPosition = new Vector2(0f, 0.5f);
                    }

                    lastX = int.MinValue;
                } break;
            }
        }

        protected virtual void GetVisibleItemIndexVertical(float y)
        {
            var itemSpancing = itemHeight + spaceingY;
            var minY = Mathf.Max(0, Mathf.FloorToInt(y / itemSpancing));
            var maxY = minY == 0 ? spawnCount : Mathf.CeilToInt((y + visibleHeight) / itemSpancing);

            var totalCount = dataSource.Count;
            if ((minY + spawnCount) > totalCount)
            {
                minY = Mathf.Max(0, totalCount - spawnCount);
            }
            maxY = Mathf.Min(maxY, totalCount);

            itemIndex.Clear();
            for (var i = minY; i < maxY; ++i)
            {
                itemIndex.Add(i);
            }
        }

        protected virtual void GetVisibleItemIndexHorizental(float x)
        {
            var itemSpancing = itemWidth + spaceingX;
            var minX = Mathf.Max(0, Mathf.FloorToInt(-x / itemSpancing));
            var maxX = minX == 0 ? spawnCount : Mathf.CeilToInt((visibleWidth - x) / itemSpancing);

            var totalCount = dataSource.Count;
            if ((minX + spawnCount) > totalCount)
            {
                minX = Mathf.Max(0, totalCount - spawnCount);
            }
            maxX = Mathf.Min(maxX, totalCount);

            itemIndex.Clear();
            for (var i = minX; i < maxX; i++)
            {
                itemIndex.Add(i);
            }
        }

        protected virtual void IndexChange(int idx)
        {
            if (!visibleNodes.ContainsKey(idx))
            {
                switch (tableType)
                {
                    case eTableViewType.Vertical:
                    {
                        PoolSpawn(1);

                        var node = itemPool.Get();
                        vec3 = node.transform.localPosition;
                        vec3.x = paddingX;
                        vec3.y = -(paddingY + (idx + 0.5f) * itemHeight + spaceingY * idx);
                        node.transform.localPosition = vec3;

                        viewDelegate.Reuse?.Invoke(node, dataSource[idx]);
                        visibleNodes.Add(idx, node);
                    }
                    break;
                    case eTableViewType.Horizental:
                    {
                        PoolSpawn(1);

                        var node = itemPool.Get();
                        vec3 = node.transform.localPosition;
                        vec3.x = paddingX + (idx + 0.5f) * itemWidth + spaceingX * idx;
                        vec3.y = paddingY;
                        node.transform.localPosition = vec3;

                        viewDelegate.Reuse?.Invoke(node, dataSource[idx]);
                        visibleNodes.Add(idx, node);
                    }
                    break;
                }
            }
        }
        protected virtual float GetTotalWidth()
        {
            if (tableType != eTableViewType.Horizental)
            {
                return 0f;
            }
            return paddingX * 2 + itemWidth * dataSource.Count + (spaceingX * Mathf.Max(0, dataSource.Count - 1));
        }
        protected virtual float GetTotalHeight()
        {
            if (tableType != eTableViewType.Vertical)
            {
                return 0f;
            }
            return paddingY * 2 + itemHeight * dataSource.Count + (spaceingY * Mathf.Max(0, dataSource.Count - 1));
        }
    }
}
