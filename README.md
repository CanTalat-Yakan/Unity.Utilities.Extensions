# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Utility Extensions

> Quick overview: Handy, zero‑dependency extension methods for common Unity tasks: camera dynamic resolution, safe destroy helpers, component access, string formatting, vector conversions/parsing, math/remap, interpolation, and audio dB mapping.

A grab‑bag of well‑scoped helpers you can call from anywhere. Toggle HDRP dynamic resolution on a Camera, safely destroy GameObjects and their children (Editor vs Play aware), get or add components, transform `snake_case`/`camelCase` into Title Case, convert and parse vectors, remap and round values, ease between ranges, and convert UI volume levels to decibels for mixers.

![screenshot](Documentation/Screenshot.png)

## Features
- Camera helpers
  - `camera.SetDynamicResolution(bool allow)` – toggles `Camera.allowDynamicResolution` and, when HDRP is present, `HDAdditionalCameraData.allowDynamicResolution` (cached per camera)
- Destroy helpers
  - `transform.DestroyAllChildren()` – removes all child GameObjects (uses `DestroyImmediate` in Editor, `Destroy` in Play)
  - `transform.DestroyAllChildren<T>()` / `component.DestroyAllChildren<T>()` – destroys all GameObjects that have `T` in this hierarchy
  - `gameObject.Destroy()` – destroys all children first, then the GameObject (Editor‑safe)
- Component helpers
  - `GetOrAddComponent<T>(this GameObject|Component)` – returns existing `T` or adds one
- String helpers
  - `"some_valueOrName".Format()` → `"Some Value Or Name"` (underscores to spaces, camel case split, Title Case)
- Vector conversions
  - `(double,double).ToVector2()`, `(double,double,double).ToVector3()`
  - `Vector2Int.ToVector2()` → `Vector3(x, y, 0)`; `Vector3Int.ToVector3()`, `Vector3.ToVector3Int()`, `Vector2.ToVector2Int()`
- Vector parsing
  - `"1920x1080".ExtractVector2FromString('x')` → `Vector2(1920, 1080)`
  - `"1,2,3".ExtractVector3FromString(',')` → `Vector3(1, 2, 3)`
- Math utilities
  - `value.Remap(srcMin, srcMax, dstMin, dstMax)` – linear remap (no clamp)
  - `vector.Round(decimalPlaces)` – round each component
  - `vectorA.Divide(vectorB)` – component‑wise division
- Interpolation helpers
  - `new Vector2(min, max).Lerp(t)` / `new Vector2Int(min, max).Lerp(t)` – linear interpolation within a range
  - `new Vector2(min, max).Slerp(t, power)` – eased Lerp (`t^power`), not geometric spherical lerp
- Audio utility
  - `intVolume.ToDecibelLevel()` – maps 0→‑80 dB, 100→0 dB, 200→+20 dB via a perceptual curve

## Requirements
- Unity Editor 6000.0+
- Optional: HDRP (for `SetDynamicResolution` to also affect `HDAdditionalCameraData`)
- No other module dependencies required

Tip: You can use these in both Editor and Play Mode. Destroy helpers automatically choose `DestroyImmediate` in the Editor to avoid orphaned objects.

## Usage
Camera dynamic resolution (HDRP‑aware)
```csharp
Camera.main.SetDynamicResolution(true);
```

Destroy children and objects
```csharp
// Remove all children of this transform
transform.DestroyAllChildren();

// Remove all GameObjects in hierarchy that have a Light component
transform.DestroyAllChildren<Light>();

// Safely destroy a whole object (including its children)
myObject.Destroy();
```

Get or add component
```csharp
var audio = gameObject.GetOrAddComponent<AudioSource>();
```

String formatting
```csharp
"player_healthMax".Format(); // "Player Health Max"
"MainMenuButton".Format();   // "Main Menu Button"
```

Vector conversions and parsing
```csharp
var v2 = (12.5, 3.75).ToVector2();
var v3 = (1.0, 2.0, 3.0).ToVector3();
var v3From2Int = new Vector2Int(5, 9).ToVector2(); // Vector3(5, 9, 0)

var res = "1920x1080".ExtractVector2FromString('x');
var pos = "1,2,3".ExtractVector3FromString(',');
```

Math and interpolation
```csharp
float mapped = 0.25f.Remap(0f, 1f, 10f, 20f); // 12.5
var rounded = new Vector3(1.2345f, 6.789f, 0.12f).Round(2);
var divided = new Vector3(10, 20, 30).Divide(new Vector3(2, 4, 5)); // (5, 5, 6)

float t = 0.3f;
float v = new Vector2(0, 100).Lerp(t);        // 30
float eased = new Vector2(0, 100).Slerp(t, 2); // 9 (quadratic ease‑in)
```

Audio (UI slider 0‑200 → dB)
```csharp
int uiVolume = 100; // center
float dB = uiVolume.ToDecibelLevel(); // 0 dB
```

## How It Works
- HDRP dynamic resolution
  - Caches `HDAdditionalCameraData` per camera via `ConditionalWeakTable` and sets both HDRP and `Camera.allowDynamicResolution` flags
- Editor‑aware destruction
  - Uses `Object.DestroyImmediate` in the Editor and `Object.Destroy` in Play Mode; the `Destroy()` helper removes children first to avoid leftovers
- Formatting
  - Replaces underscores, inserts spaces before capitals, and applies Title Case via the current culture
- Parsing and math
  - Vector parsing uses `float.TryParse` on current culture; invalid inputs yield `Vector2.zero`/`Vector3.zero`
  - `Remap` is linear and does not clamp; `Round` rounds each component; `Divide` is per‑component
- Interpolation
  - `Lerp` treats a `Vector2`/`Vector2Int` as `[min,max]`; `Slerp` eases by `t^power` (name refers to easing, not true spherical lerp)

## Notes and Limitations
- HDRP dependency (optional)
  - `SetDynamicResolution` benefits from HDRP’s `HDAdditionalCameraData`; without HDRP, only `Camera.allowDynamicResolution` is toggled
- DestroyAllChildren<T>
  - Destroys entire GameObjects that contain `T` somewhere below the given transform
- Culture‑sensitive parsing
  - `ExtractVector*FromString` uses `float.TryParse` with current locale; decimal separators (comma vs dot) must match
- Remap domain
  - `Remap` divides by `(sourceMax - sourceMin)`; ensure the source range is non‑zero and clamp externally if needed
- Interpolation naming
  - `Slerp` here is an eased linear interpolation, not geometric spherical interpolation

## Files in This Package
- `Runtime/UtilityExtensions.cs` – Extension methods (camera, destroy helpers, components, strings, vectors, math, interpolation, audio)
- `Runtime/UnityEssentials.UtilityExtensions.asmdef` – Runtime assembly definition
- `package.json` – Package manifest metadata

## Tags
unity, utilities, extensions, helpers, camera, dynamic‑resolution, destroy, component, string, format, vector, parse, math, remap, interpolation, audio, decibel, hdrp
