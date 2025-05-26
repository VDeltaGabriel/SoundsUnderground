using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Facility
{
    public class FacilityGenerator : MonoBehaviour
    {
        public int maxRooms = 50;
        public List<RoomDefinition> roomTypes;
        public Transform dungeonParent;

        private Dictionary<Vector3Int, RoomDefinition> spawnedRooms = new();

        private readonly Dictionary<DoorDirection, Vector3Int> directionOffsets = new()
        {
            { DoorDirection.Up, new Vector3Int(0, 0, 1) },
            { DoorDirection.Down, new Vector3Int(0, 0, -1) },
            { DoorDirection.Left, new Vector3Int(-1, 0, 0) },
            { DoorDirection.Right, new Vector3Int(1, 0, 0) }
        };

        void Start()
        {
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            Queue<(RoomDefinition room, Vector3Int position)> roomQueue = new();
            Vector3Int startPos = Vector3Int.zero;
            RoomDefinition startRoom = roomTypes[Random.Range(0, roomTypes.Count)];
            roomQueue.Enqueue((startRoom, startPos));
            spawnedRooms[startPos] = startRoom;

            int roomsPlaced = 1;

            while (roomQueue.Count > 0 && roomsPlaced < maxRooms)
            {
                var (currentRoom, currentPos) = roomQueue.Dequeue();

                var currentEntries = currentRoom.DoorsDict;

                foreach (var entry in currentEntries)
                {
                    DoorDirection dir = entry.Key;
                    Vector3Int offset = directionOffsets[dir];
                    Vector3Int nextPos = currentPos + offset;

                    if (spawnedRooms.ContainsKey(nextPos))
                        continue; // already placed

                    // Find a compatible room that has matching opposite door
                    DoorDirection oppositeDir = GetOpposite(dir);
                    RoomDefinition nextRoom = GetRoomWithEntry(oppositeDir);
                    if (nextRoom == null)
                        continue;

                    spawnedRooms[nextPos] = nextRoom;
                    roomQueue.Enqueue((nextRoom, nextPos));
                    roomsPlaced++;

                    if (roomsPlaced >= maxRooms)
                        break;
                }
            }

            InstantiateRooms();
        }

        void InstantiateRooms()
        {
            foreach (var kvp in spawnedRooms)
            {
                Vector3Int gridPos = kvp.Key;
                RoomDefinition room = kvp.Value;

                Vector3 worldPos = new Vector3(gridPos.x * 10, 0, gridPos.z * 10);
                Instantiate(room.Model, worldPos, Quaternion.identity, dungeonParent);
            }
        }

        RoomDefinition GetRoomWithEntry(DoorDirection requiredEntry)
        {
            List<RoomDefinition> candidates = new();

            foreach (var room in roomTypes)
            {
                var entries = room.DoorsDict;
                if (entries.ContainsKey(requiredEntry))
                    candidates.Add(room);
            }

            return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
        }

        DoorDirection GetOpposite(DoorDirection dir)
        {
            return dir switch
            {
                DoorDirection.Up => DoorDirection.Down,
                DoorDirection.Down => DoorDirection.Up,
                DoorDirection.Left => DoorDirection.Right,
                DoorDirection.Right => DoorDirection.Left,
                _ => dir
            };
        }
    }
}