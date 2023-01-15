using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Path : Building
    {
        [SerializeField] private List<PathBuild> _pathBuilds;
        [SerializeField, ReadOnly] private List<Direction> _directions;
        
        public void SetTile(List<Direction> list)
        {
            print($"path set {list.Count}");
            _pathBuilds.ForEach(x => x.TileGameObject.SetActive(false));
            foreach (Direction direction in list)
            {
                _pathBuilds.Find(x => x.TileDirection == direction).TileGameObject.SetActive(true);
            }
            
            _directions.Clear();
            _directions = list;
        }
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [Serializable]
    public struct PathBuild
    {
        public Direction TileDirection;
        public GameObject TileGameObject;
    }
}