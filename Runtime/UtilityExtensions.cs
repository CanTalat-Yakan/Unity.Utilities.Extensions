using UnityEngine;

namespace UnityEssentials
{
    public static class UtilityExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            return gameObject.AddComponent<T>();
        }

        public static Vector3 ToVector3(this (double x, double y, double z)v) =>
            new Vector3((float)v.x, (float)v.y, (float)v.z);

        public static float Remap(this float value, float from1, float to1, float from2, float to2) =>
            (value - from1) / (to1 - from1) * (to2 - from2) + from2;

        public static Vector3 Round(this Vector3 vector, int decimalPlaces = 2)
        {
            float multiplier = Mathf.Pow(10f, decimalPlaces);

            return new Vector3(
                Mathf.Round(vector.x * multiplier) / multiplier,
                Mathf.Round(vector.y * multiplier) / multiplier,
                Mathf.Round(vector.z * multiplier) / multiplier);
        }

        public static Vector3 Divide(this Vector3 numerator, Vector3 denominator) =>
            new Vector3(
                numerator.x / denominator.x,
                numerator.y / denominator.y,
                numerator.z / denominator.z);


        public static float Lerp(this Vector2Int v, float t) =>
            v.x + (v.y - v.x) * t;

        public static float Lerp(this Vector2 v, float t) =>
            v.x + (v.y - v.x) * t;

        public static float Slerp(this Vector2 v, float t, float power = 2f)
        {
            // Apply exponential easing (smooth curve)
            float easedT = Mathf.Pow(t, power);

            return v.x + (v.y - v.x) * easedT;
        }
    }
}
