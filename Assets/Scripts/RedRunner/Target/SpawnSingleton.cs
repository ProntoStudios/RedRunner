using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Target
{

    public class SpawnSingleton : MonoBehaviour
    {
        public static GameObject instance { get; protected set; }

        public delegate void SpawnCreatedHandler();

        public static event SpawnCreatedHandler OnSpawnCreated;

        void Awake()
        {
            instance = this.gameObject;
            if (OnSpawnCreated != null)
            {
                OnSpawnCreated();
            }
        }
    }

}