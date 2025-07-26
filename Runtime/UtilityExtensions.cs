using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEssentials
{
    public static class UtilityExtensions
    {
        private static readonly ConditionalWeakTable<Camera, HDAdditionalCameraData> _cameraDataCache = new();
        public static void SetDynamicResolution(this Camera camera, bool allow)
        {
            if (!_cameraDataCache.TryGetValue(camera, out var cameraData))
            {
                cameraData = camera.GetComponent<HDAdditionalCameraData>();
                if (cameraData != null)
                    _cameraDataCache.Add(camera, cameraData);
            }

            if (cameraData != null)
                cameraData.allowDynamicResolution = allow;
            camera.allowDynamicResolution = allow;
        }

        public static void DestroyAllChildren(this Component script) =>
            DestroyAllChildren(script.transform);

        public static void DestroyAllChildren<T>(this Component script) where T : Component =>
            DestroyAllChildren<T>(script.transform);

        public static void DestroyAllChildren(this Transform transform)
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    Object.DestroyImmediate(transform.GetChild(0).gameObject);
                else Object.Destroy(transform.GetChild(0).gameObject);
        }

        public static void DestroyAllChildren<T>(this Transform transform) where T : Component
        {
            foreach (var child in transform.GetComponentsInChildren<T>())
                Destroy(child.gameObject);
        }

        public static void Destroy(this GameObject gameObject)
        {
            DestroyAllChildren(gameObject.transform);

            if (Application.isEditor)
                Object.DestroyImmediate(gameObject);
            else Object.Destroy(gameObject);
        }

        public static T GetOrAddComponent<T>(this Component script) where T : Component =>
            GetOrAddComponent<T>(script.gameObject);

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            return gameObject.AddComponent<T>();
        }

        public static Vector2 ToVector2(this (double x, double y) vector) =>
            new Vector2((float)vector.x, (float)vector.y);

        public static Vector3 ToVector2(this Vector2Int vector) =>
            new Vector3(vector.x, vector.y);

        public static Vector2Int ToVector2Int(this Vector2 vector) =>
            new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));

        public static Vector3 ToVector3(this (double x, double y, double z) vector) =>
            new Vector3((float)vector.x, (float)vector.y, (float)vector.z);

        public static Vector3 ToVector3(this Vector3Int vector) =>
            new Vector3(vector.x, vector.y, vector.z);

        public static Vector3Int ToVector3Int(this Vector3 vector) =>
            new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));

        public static Vector2 ExtractVector2FromString(this string input, char separator)
        {
            var parts = input.Split(separator);
            if (parts.Length != 2)
                return Vector2.zero;
            if (float.TryParse(parts[0], out var width) && float.TryParse(parts[1], out var height))
                return new Vector2(width, height);
            return Vector2.zero;
        }

        public static Vector3 ExtractVector3FromString(this string input, char separator)
        {
            var parts = input.Split(separator);
            if (parts.Length != 3)
                return Vector3.zero;
            if (float.TryParse(parts[0], out var x) && float.TryParse(parts[1], out var y) && float.TryParse(parts[2], out var z))
                return new Vector3(x, y, z);
            return Vector3.zero;
        }

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

        /// <summary>
        /// Converts an integer volume level to a decibel level.
        /// </summary>
        /// <remarks>The method applies a logarithmic scaling to convert the volume level to a decibel
        /// level, ensuring a more natural perception of volume changes.</remarks>
        /// <param name="volume">The volume level to convert, expected to be in the range [0, 200].</param>
        /// <returns>A float representing the decibel level, where 0 corresponds to -80 dB, 100 corresponds to 0 dB, and 200
        /// corresponds to +20 dB.</returns>
        public static float ToDecibelLevel(this int volume)
        {
            // Clamp the input volume to ensure it's within the range [0, 200]
            // Then normalize it to the range [0, 1] where:
            // - 0 corresponds to the minimum volume (-80 dB)
            // - 0.5 corresponds to the default volume (0 dB)
            // - 1 corresponds to the maximum volume (+20 dB)
            float normalized = Mathf.Clamp(volume, 0, 200) / 200f;

            // Step 1: Apply the logarithmic scaling
            // Use Mathf.Pow(16, 1 - normalized) to create a logarithmic curve for volume
            float logValue = Mathf.Pow(16, 1 - normalized);

            // Step 2: Scale the log value to a range that will work for interpolation
            // We divide by 16 to normalize it to a range between 0 and 1
            float logScaledVolume = logValue / 16f;

            // Step 3: Invert the scaled volume so that 0 maps to -80 dB and 1 maps to +20 dB
            float invertedLogVolume = 1 - logScaledVolume;

            // Step 4: Normalize the inverted log volume (for interpolation) so it fits in the dB range
            // The range we want is from -80 dB to +20 dB.
            float interpolationValue = invertedLogVolume / (1 - Mathf.Pow(16, -1));

            // Step 5: Use Mathf.Lerp to map the interpolation value to the desired dB range
            // -80 dB to +20 dB
            float decibel = Mathf.Lerp(-80f, 20f, interpolationValue);

            return decibel;
        }
    }
}
