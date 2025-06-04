using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Facility
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
            }
            return direction;
        }
    }
    
    public class RoomDefinition
    {
        public Vector2Int Position { get; private set; }
        public HashSet<Direction> Connections { get; private set; }
        public bool IsStart { get; set; }
        public float Cost { get; set; }

        public RoomDefinition(Vector2Int pos)
        {
            Position = pos;
            Connections = new HashSet<Direction>();
            IsStart = false;
            Cost = 0f;
        }

        public void AddConnection(Direction dir)
        {
            Connections.Add(dir);
        }

        public bool HasConnection(Direction dir)
        {
            return Connections.Contains(dir);
        }
    }
}