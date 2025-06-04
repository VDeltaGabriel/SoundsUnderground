using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Facility
{
    public class FacilityGenerator : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        [SerializeField] private int _steps = 30;
        [SerializeField] private float _roomSpacing = 10f;

        [Header("Rooms")] 
        [SerializeField] private RoomDef _deadEnd;
        [SerializeField] private RoomDef _straight;
        [SerializeField] private RoomDef _corner;
        [SerializeField] private RoomDef _tway;
        [SerializeField] private RoomDef _fway;
        
        private Dictionary<Vector2Int, RoomDefinition> _rooms = new Dictionary<Vector2Int, RoomDefinition>();
        private Dictionary<Vector2Int, RoomDef> _roomDefs = new Dictionary<Vector2Int, RoomDef>();
        private Vector2Int _startRoom;

        private void Start()
        {
            GenerateFacility();
        }

        private void GenerateFacility()
        {
            Vector2Int start = new Vector2Int(_width / 2, _height / 2);
            Vector2Int cur = start;

            _startRoom = start;
            _rooms[cur] = new RoomDefinition(cur);
            _rooms[cur].IsStart = true;

            for (int i = 0; i < _steps; i++)
            {
                Direction dir = GetRandomDir();
                Vector2Int next = cur + DirToVec(dir);

                if (!InBounds(next)) continue;

                if (!_rooms.ContainsKey(next))
                {
                    _rooms[next] = new RoomDefinition(next);
                    _rooms[cur].AddConnection(dir);
                    _rooms[next].AddConnection(dir.Opposite());
                    cur = next;
                }
                else
                {
                    cur = GetRandomOccupiedPos();
                }
            }

            CalculateCosts();
            InstantiateRooms();
            GameManager.Instance.GetPlayer().SetPosition(_roomDefs[_startRoom].transform.position + (Vector3.up * 0.25f));
        }

        private void CalculateCosts()
        {
            Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            distances[_startRoom] = 0;
            queue.Enqueue(_startRoom);

            int maxDst = 1;

            while (queue.Count > 0)
            {
                Vector2Int cur = queue.Dequeue();
                int curDst = distances[cur];
                maxDst = Mathf.Max(maxDst, curDst);

                RoomDefinition room = _rooms[cur];
                foreach (Direction dir in room.Connections)
                {
                    Vector2Int neighbor = cur + DirToVec(dir);
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = curDst + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            foreach (KeyValuePair<Vector2Int, RoomDefinition> pair in _rooms)
            {
                Vector2Int pos = pair.Key;
                RoomDefinition room = pair.Value;
                room.Cost = (float)distances[pos] / maxDst;
            }
        }

        private void InstantiateRooms()
        {
            foreach (RoomDefinition room in _rooms.Values)
            {
                Quaternion rot = GetRotForConn(room.Connections);

                Vector3 pos = new Vector3(room.Position.x * _roomSpacing, 0, room.Position.y * _roomSpacing);
                RoomDef obj = Instantiate(room.IsStart ? _deadEnd : GetRoom(room.Connections), pos, rot, transform);
                obj.InitRoom();
                _roomDefs.Add(room.Position, obj);
            }
        }

        private Quaternion GetRotForConn(HashSet<Direction> conns)
        {
            switch (conns.Count)
            {
                case 1:
                    return RotFromDir(GetFirstDir(conns));
                case 2:
                {
                    if (IsStraight(conns))
                        return conns.Contains(Direction.Left) ? Quaternion.Euler(0,90,0) : Quaternion.identity;
                    
                    if (conns.Contains(Direction.Up) && conns.Contains(Direction.Right)) return Quaternion.identity;
                    if (conns.Contains(Direction.Right) && conns.Contains(Direction.Down)) return Quaternion.Euler(0, 90, 0);
                    if (conns.Contains(Direction.Down) && conns.Contains(Direction.Left)) return Quaternion.Euler(0, 180, 0);
                    if (conns.Contains(Direction.Left) && conns.Contains(Direction.Up)) return Quaternion.Euler(0, 270, 0);
                    return Quaternion.identity;
                }
                case 3:
                {
                    if (!conns.Contains(Direction.Up)) return Quaternion.Euler(0, 180, 0);
                    if (!conns.Contains(Direction.Right)) return Quaternion.Euler(0, 270, 0);
                    if (!conns.Contains(Direction.Down)) return Quaternion.identity;
                    if (!conns.Contains(Direction.Left)) return Quaternion.Euler(0, 90, 0);
                    return Quaternion.identity;
                }
                default:
                    return Quaternion.identity;
            }
        }
        
        private RoomDef GetRoom(HashSet<Direction> dirs)
        {
            return dirs.Count switch
            {
                1 => _deadEnd,
                2 => IsStraight(dirs) ? _straight : _corner,
                3 => _tway,
                _ => _fway
            };
        }

        private Quaternion RotFromDir(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Quaternion.identity,
                Direction.Right => Quaternion.Euler(0, 90, 0),
                Direction.Down => Quaternion.Euler(0, 180, 0),
                Direction.Left => Quaternion.Euler(0, 270, 0),
                _ => Quaternion.identity,
            };
        }

        private bool IsStraight(HashSet<Direction> dirs)
        {
            return (dirs.Contains(Direction.Up) && dirs.Contains(Direction.Down)) ||
                   (dirs.Contains(Direction.Right) && dirs.Contains(Direction.Left));
        }

        private Direction GetFirstDir(HashSet<Direction> dirs)
        {
            return dirs.First();
        }

        private Vector2Int DirToVec(Direction dir)
        {
            return dir switch
            {
                Direction.Up => new Vector2Int(0, 1),
                Direction.Down => new Vector2Int(0, -1),
                Direction.Left => new Vector2Int(-1, 0),
                Direction.Right => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 0),
            };
        }

        private Vector2Int GetRandomOccupiedPos()
        {
            List<Vector2Int> keys = new List<Vector2Int>(_rooms.Keys);
            return keys[Random.Range(0, keys.Count)];
        }

        private Direction GetRandomDir()
        {
            return (Direction)Random.Range(0, 4);
        }

        private bool InBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
        }

        private void OnDrawGizmos()
        {
            if (_rooms == null) return;

            foreach (RoomDefinition room in _rooms.Values)
            {
                Vector3 pos = new Vector3(room.Position.x * _roomSpacing, 0, room.Position.y * _roomSpacing);
                Gizmos.color = Color.Lerp(Color.green, Color.red, room.Cost);
                Gizmos.DrawWireCube(_roomDefs[room.Position].transform.position + (Vector3.up * 1.25f), _roomDefs[room.Position].ModelBounds.size);
                //Gizmos.DrawSphere(pos + Vector3.up * 0.5f, 0.3f);
            }
        }
    }
}