using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Target
{

    public class SpawnSingleton : MonoBehaviour
    {

        public static GameObject instance { get; protected set; }

        void Awake()
        {
            instance = this.gameObject;
        }
    }

}