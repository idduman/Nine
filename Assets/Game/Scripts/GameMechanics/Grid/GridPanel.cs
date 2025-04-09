using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    public class GridPanel : MonoBehaviour
    {
        [SerializeField] private List<GridController> _gridList;
        public GridController ActiveGrid { get; private set; }
        public void Initialize()
        {
        }

        public void SetActiveGrid(int size)
        {
            GridController grid = null;
            for (int i = 0; i < _gridList.Count; i++)
            {
                if (_gridList[i].Width == size)
                {
                    ActiveGrid = _gridList[i];
                    _gridList[i].gameObject.SetActive(true);
                }
                else
                    _gridList[i].gameObject.SetActive(false);
            }
            if(!ActiveGrid)
                throw new ArgumentException("Grid difficulty is out of range");
        }
    }
}