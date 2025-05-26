using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Facility
{
    public enum DoorDirection
    {
        Up,Down,Left,Right
    }

    [System.Serializable]
    public struct DoorDefinition
    {
        public DoorDirection Direction;
        public Transform Door;
    }

    [CreateAssetMenu(fileName = "New Room Def", menuName = "Facility/RoomDefinition/Create")]
    public class RoomDefinition : ScriptableObject
    {
        public List<DoorDefinition> Doors = new List<DoorDefinition>();
        public GameObject Model;

        public Dictionary<DoorDirection, Transform> DoorsDict
        {
            get
            {
                Dictionary<DoorDirection, Transform> dict = new Dictionary<DoorDirection, Transform>();
                foreach (DoorDefinition def in Doors) dict.Add(def.Direction, def.Door);
                return dict;
            }
        }
    }
}