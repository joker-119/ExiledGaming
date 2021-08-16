using UnityEngine;

namespace ExiledGaming.Components
{
    using System;

    internal class HatItemComponent : MonoBehaviour
    {
        internal HatPlayerComponent player;
        internal Vector3 pos;
        internal Vector3 itemOffset;
        internal Quaternion rot;
        internal Pickup pickup;

        private void Start()
        {
            Plugin.Instance.Methods.Hats.Add(pickup);
        }

        private void OnDestroy()
        {
            Plugin.Instance.Methods.Hats.Remove(pickup);
        }
    }
}