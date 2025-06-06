using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Facility
{
    public struct RoomData
    {
        public RoomDef def;
        public int rot;
    }
    
    public class FacilityGenerator : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private Vector2Int _size;
        [SerializeField] private float _roomSize = 0.25f;
        

        [Header("Rooms")] 
        [SerializeField] private RoomDef[] _defs;

        private Dictionary<Vector2Int, RoomDefinition> _placedRooms = new Dictionary<Vector2Int, RoomDefinition>();

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            _placedRooms.Clear();
            Vector2Int start = Vector2Int.zero;
            
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);

            RoomDef startRoom = _defs.First(r =>
                r.AvailableDirections.ToHashSet().SetEquals(new [] {Direction.Up}) || r.AvailableDirections.ToHashSet().SetEquals(new [] {Direction.Right}));

            Instantiate(startRoom, new Vector3(start.x * _roomSize, 0, start.y * _roomSize), Quaternion.identity,
                transform);
            _placedRooms.Add(start, new RoomDefinition(start, startRoom.AvailableDirections));

            foreach (Direction dir in startRoom.AvailableDirections)
            {
                Vector2Int next = start + dir.ToIntVec2();
                if (!_placedRooms.ContainsKey(next) && InBounds(next)) queue.Enqueue(next);
            }

            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();
                if (_placedRooms.ContainsKey(pos)) continue;
                
                List<Direction> neededConnections = new();

                foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) {
                    Vector2Int neighborPos = pos + dir.ToIntVec2();
                    if (_placedRooms.TryGetValue(neighborPos, out RoomDefinition neighborRoom)) {
                        if (System.Array.Exists(neighborRoom.Directions, d => d == dir.Opposite())) {
                            neededConnections.Add(dir);
                        }
                    }
                }

                RoomDef room = FindMatch(neededConnections);
                if (!room) continue;
                
                RoomDef spawned = Instantiate(room, new Vector3(pos.x * _roomSize, 0, pos.y * _roomSize), Quaternion.identity, transform);
                _placedRooms.Add(pos, new RoomDefinition(pos, room.AvailableDirections));
                foreach (Direction dir in room.AvailableDirections)
                {
                    Vector2Int next = pos + dir.ToIntVec2();
                    if (!_placedRooms.ContainsKey(next) && InBounds(next)) queue.Enqueue(next);
                }
            }
        }

        private RoomDef FindMatch(List<Direction> required) {
            foreach (RoomDef prefab in _defs) {
                if (new HashSet<Direction>(prefab.AvailableDirections).IsSupersetOf(required)) {
                    return prefab;
                }
            }
            return null;
        }
        
        private bool InBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _size.x && pos.y >= 0 && pos.y < _size.y;
        }
        
        private void OnDrawGizmos()
        {
        }
    }
}