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

        public static Vector2Int ToIntVec2(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => new Vector2Int(0, 1),
                Direction.Down => new Vector2Int(0, -1),
                Direction.Left => new Vector2Int(-1, 0),
                Direction.Right => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 0),
            };
        }
    }
    
    public class RoomDefinition
    {
        public Vector2Int Position { get; private set; }
        public HashSet<Direction> Connections { get; private set; }
        public Direction[] Directions { get; private set; }
        public bool IsStart { get; set; }
        public float Cost { get; set; }

        public RoomDefinition(Vector2Int pos, Direction[] directions)
        {
            Position = pos;
            Connections = new HashSet<Direction>();
            IsStart = false;
            Cost = 0f;
            Directions = directions;
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