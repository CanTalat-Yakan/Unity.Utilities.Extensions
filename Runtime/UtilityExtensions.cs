using UnityEngine;

namespace UnityEssentials
{
    public static class UtilityExtensions
    {
        public static void DestroyAllChildren(this Component script) =>
            DestroyAllChildren(script.transform);

        public static void DestroyAllChildren(this Transform transform)
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    Object.DestroyImmediate(transform.GetChild(0).gameObject);
                else Object.Destroy(transform.GetChild(0).gameObject);
        }

        public static T GetOrAddComponent<T>(this Component script) where T : Component =>
            GetOrAddComponent<T>(script.gameObject);

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            return gameObject.AddComponent<T>();
        }

        public static Vector3 ToVector3(this (double x, double y, double z) vector) =>
            new Vector3((float)vector.x, (float)vector.y, (float)vector.z);

        public static float Remap(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax) =>
            (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;

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

        public static float Lerp(this Vector2Int vector, float time) =>
            vector.x + (vector.y - vector.x) * time;

        public static float Lerp(this Vector2 vector, float time) =>
            vector.x + (vector.y - vector.x) * time;

        public static float Slerp(this Vector2 vector, float time, float power = 2f)
        {
            float easedTime = Mathf.Pow(time, power);
            return vector.x + (vector.y - vector.x) * easedTime;
        }
    }
}
