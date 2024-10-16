using UnityEngine;
using System;
using System.Collections;

namespace Defense
{
    public abstract class DefenseSkillBase : MonoBehaviour
    {
        public GameObject player;

        public abstract void Init(GameObject player);
        public abstract void ConductDefenseAction();
    }
}
