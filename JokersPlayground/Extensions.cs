using UnityEngine;

namespace JokersPlayground
{
    public static class Extensions
    {
        public static bool IsZero(this Quaternion quaternion) => quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0;
    }
}