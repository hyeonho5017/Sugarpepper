using UnityEngine;

namespace Sugarpepper
{
    public class TableViewGrid : TableView
    {
        [Space(10f)]
        [Header("Grid")]
        [Tooltip("1 보다는 커야합니다.")]
        [SerializeField]
        protected int grid = 1;

        public override void OnStart()
        {
            base.OnStart();

            if (this.grid < 1)
                this.grid = 1;

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
                    spawnCount = (Mathf.RoundToInt(visibleHeight / itemHeight / contentScaleY) + 1) * grid;
                }
                break;
                case eTableViewType.Horizental:
                {
                    scrollView.vertical = false;
                    scrollView.horizontal = true;
                    contentTransform.anchorMin = new Vector2(0f, 0.5f);
                    contentTransform.anchorMax = new Vector2(0f, 0.5f);
                    contentTransform.pivot = new Vector2(0f, 0.5f);
                    spawnCount = (Mathf.RoundToInt(visibleWidth / itemWidth / contentScaleX) + 1) * grid;
                }
                break;
            }

            PoolSpawn(spawnCount);

            //사용법
            //tableView.OnStart();

            //var items = new List<object>();
            //for (var i = 1; i <= 150; i++)
            //{
            //    items.Add(new Class());
            //}
            //tableView.SetDelegate(new TableViewDelegate(items, (GameObject node, object data) =>
            //{
            //      var item = (Class)data; // 사용
            //}));

            //tableView.ReLoad();
        }

        protected override void IndexChange(int idx)
        {
            if (!visibleNodes.ContainsKey(idx))
            {
                var gridRow = Mathf.FloorToInt(idx / grid);
                var gridCol = idx % grid;
                switch (this.tableType)
                {
                    case eTableViewType.Vertical:
                    {
                        PoolSpawn(1);

                        var node = itemPool.Get();
                        vec3 = node.transform.localPosition;
                        vec3.x = -contentTransform.sizeDelta.x * 0.5f + paddingX + (gridCol + 0.5f) * itemWidth + spaceingX * gridCol;
                        vec3.y = -(paddingY + (gridRow + 0.5f) * itemHeight + spaceingY * gridRow);
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
                        vec3.x = paddingX + (gridRow + 0.5f) * itemWidth + spaceingX * gridRow;
                        vec3.y = contentTransform.sizeDelta.y * 0.5f - (paddingY + (gridCol + 0.5f) * itemHeight + spaceingY * gridCol);
                        node.transform.localPosition = vec3;

                        viewDelegate.Reuse?.Invoke(node, dataSource[idx]);
                        visibleNodes.Add(idx, node);
                    }
                    break;
                }
            }
        }
        protected override void GetVisibleItemIndexVertical(float y)
        {
            var itemSpancing = itemHeight + spaceingY;
            var minY = Mathf.Max(0, Mathf.FloorToInt(y / itemSpancing) * grid);
            var maxY = minY == 0 ? spawnCount : Mathf.CeilToInt((y + visibleHeight + itemSpancing * 0.5f) / itemSpancing) * grid;

            var totalCount = dataSource.Count;
            if ((minY + spawnCount) > totalCount)
            {
                minY = Mathf.Max(0, totalCount - spawnCount);
            }
            maxY = Mathf.Min(maxY, totalCount);

            itemIndex.Clear();
            for (var i = minY; i < maxY; i++)
            {
                itemIndex.Add(i);
            }
        }

        protected override void GetVisibleItemIndexHorizental(float x)
        {
            var itemSpancing = itemWidth + spaceingX;
            var minX = Mathf.Max(0, Mathf.FloorToInt(-x / itemSpancing) * grid);
            var maxX = minX == 0 ? spawnCount : Mathf.CeilToInt((visibleWidth - x + itemSpancing * 0.5f) / itemSpancing) * grid;

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

        protected override float GetTotalWidth()
        {
            if (tableType != eTableViewType.Horizental)
                return paddingX * 2 + itemWidth * grid + (spaceingX * Mathf.Max(0, grid - 1));

            var gridLength = dataSource.Count / grid + ((dataSource.Count % grid) != 0 ? 1 : 0);
            return paddingX * 2 + itemWidth * gridLength + (spaceingX * Mathf.Max(0, gridLength - 1));
        }

        protected override float GetTotalHeight()
        {
            if (tableType != eTableViewType.Vertical)
            {
                return paddingY * 2 + itemHeight * grid + (spaceingY * Mathf.Max(0, grid - 1));
            }
            var gridLength = dataSource.Count / grid + ((dataSource.Count % grid) != 0 ? 1 : 0);
            return paddingY * 2 + itemHeight * gridLength + (spaceingY * Mathf.Max(0, gridLength - 1));
        }
    }
}