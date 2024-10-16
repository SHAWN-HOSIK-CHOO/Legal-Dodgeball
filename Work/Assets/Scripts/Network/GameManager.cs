using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; protected set; } = null;

        public bool IsServer = false;
        public List<GameObject> prefabLists = new List<GameObject>();
        protected Dictionary<string, GameObject> prefabDicts = new Dictionary<string, GameObject>();
    }
}
