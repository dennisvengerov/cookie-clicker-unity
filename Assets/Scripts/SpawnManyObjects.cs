using UnityEngine;

// public class SpawnManyObjects : MonoBehaviour
// {
// 	public Sprite creamSprite;

//     void Start()
//     {
//         float startTime = Time.realtimeSinceStartup;
        
//         // Spawn 1000 objects with sprites
//         for (int i = 0; i < 1000; i++)
//         {
//             GameObject obj = new GameObject($"Cookie_{i}");
//             SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
//             // Assign a sprite if you have one
//             sr.sprite = creamSprite;
//             obj.transform.position = Random.insideUnitCircle * 10;

//         }
        
//         float spawnTime = Time.realtimeSinceStartup - startTime;
//         Debug.Log($"Spawning 1000 objects took: {spawnTime:F3} seconds");
//     }
// // }
// public class SpawnManyObjects : MonoBehaviour
// {
//     public Sprite creamSprite;
//     public int numberOfCookies = 50;
    
//     void Start()
//     {
//         if (creamSprite == null)
//         {
//             Debug.LogError("⚠️ No sprite assigned! Drag a sprite into the Inspector!");
//             return;
//         }
        
//         // Make sure camera is set up right
//         Camera cam = Camera.main;
//         if (cam != null)
//         {
//             cam.transform.position = new Vector3(0, 0, -10);
//             cam.orthographic = true;
//             cam.orthographicSize = 15;
//             Debug.Log("✅ Camera positioned automatically!");
//         }
        
//         float startTime = Time.realtimeSinceStartup;
        
//         // Spawn cookies in upper-middle area
//         for (int i = 0; i < numberOfCookies; i++)
//         {
//             GameObject obj = new GameObject($"Cookie_{i}");
//             SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
//             sr.sprite = creamSprite;
            
//             // Spawn in a box in the upper-middle area
//             float x = Random.Range(-8f, 8f);  // Spread horizontally
//             float y = Random.Range(2f, 12f);  // Upper portion of screen
//             obj.transform.position = new Vector3(x, y, 0f);
            
//             // Make them BIGGER and more visible
//             obj.transform.localScale = Vector3.one * 0.8f;  // Much bigger!
//             sr.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.9f, 1f); // Bright colors
//             sr.sortingOrder = 10;
//         }
        
//         float time = Time.realtimeSinceStartup - startTime;
//         Debug.Log($"🍪 Spawned {numberOfCookies} cookies in {time:F3} seconds!");
//     }
// }





// using UnityEngine;
// using UnityEngine.UI;

// public class SpawnManyObjects : MonoBehaviour
// {
//     public Sprite cookieSprite;
//     public RectTransform uiParent;

//     public int numberOfCookies = 1000;
//     public Vector2 padding = new Vector2(40f, 40f);   // keep cookies away from edges
//     public Vector2 uniformScaleRange = new Vector2(0.6f, 0.9f);

//     void Start()
//     {
//         if (cookieSprite == null || uiParent == null)
//         {
//             Debug.LogError("Assign cookieSprite and uiParent (Canvas/RectTransform) in the Inspector.");
//             return;
//         }

//         var rect = uiParent.rect;

//         for (int i = 0; i < numberOfCookies; i++)
//         {
//             // Create a UI Image (not a SpriteRenderer)
//             var go = new GameObject($"Cookie_{i}", typeof(RectTransform), typeof(Image));
//             go.transform.SetParent(uiParent, false);     // parented to Canvas
//             go.transform.SetAsLastSibling();             // draw ON TOP of everything under uiParent

//             var img = go.GetComponent<Image>();
//             img.sprite = cookieSprite;
//             img.raycastTarget = false;                   // don’t block UI clicks
//             img.SetNativeSize();

//             // Random position inside the canvas rect (with padding)
//             var rt = (RectTransform)go.transform;
//             float x = Random.Range(rect.xMin + padding.x, rect.xMax - padding.x);
//             float y = Random.Range(rect.yMin + padding.y, rect.yMax - padding.y);
//             rt.anchoredPosition = new Vector2(x, y);

//             // Random uniform scale
//             float s = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
//             rt.localScale = Vector3.one * s;
//         }

//         Debug.Log($"🍪 Spawned {numberOfCookies} UI cookies on top.");
//     }
// }
// done w ts

// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class SpawnManyObjects : MonoBehaviour
// {
//     [Header("Input")]
//     public Sprite cookieSprite;
//     public RectTransform uiParent;          // assign TopFX (or Canvas)

//     [Header("Spawn Settings")]
//     public int numberOfCookies = 200;
//     public Vector2 padding = new Vector2(40f, 40f);

//     // WAY smaller scale (tweak as you like)
//     public Vector2 uniformScaleRange = new Vector2(0.05f, 0.12f);

//     // delay between spawns (seconds). Use Realtime to ignore Time.timeScale.
//     public float spawnInterval = 0.02f;     // 20 ms between each cookie

//     private Rect parentRect;

//     void Start()
//     {
//         if (cookieSprite == null || uiParent == null)
//         {
//             Debug.LogError("Assign cookieSprite and uiParent in the Inspector.");
//             return;
//         }

//         parentRect = uiParent.rect;
//         StartCoroutine(SpawnRoutine());
//     }

//     private IEnumerator SpawnRoutine()
//     {
//         for (int i = 0; i < numberOfCookies; i++)
//         {
//             // Create a UI Image
//             var go = new GameObject($"Cookie_{i}", typeof(RectTransform), typeof(Image));
//             var rt = (RectTransform)go.transform;
//             rt.SetParent(uiParent, false);
//             rt.SetAsLastSibling();                   // later spawns on top

//             var img = go.GetComponent<Image>();
//             img.sprite = cookieSprite;
//             img.raycastTarget = false;
//             img.SetNativeSize();

//             // make it white (works if sprite is grayscale)
//             // if your art is colored, swap to a white sprite or use a force-white material
//             img.color = Color.white; 

//             // Position within UI parent (with padding)
//             float x = Random.Range(parentRect.xMin + padding.x, parentRect.xMax - padding.x);
//             float y = Random.Range(parentRect.yMin + padding.y, parentRect.yMax - padding.y);
//             rt.anchoredPosition = new Vector2(x, y);

//             // Tiny uniform scale + a little random rotation for variety
//             float s = Random.Range(uniformScaleRange.x, uniformScaleRange.y) * 0.01f;
//             rt.localScale = new Vector3(s, s, 1f);
//             rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-12f, 12f));

//             // wait before spawning the next one (layered over time)
//             yield return new WaitForSecondsRealtime(spawnInterval);
//         }

//         Debug.Log($"🍪 Spawned {numberOfCookies} tiny UI cookies with {spawnInterval}s delay.");
//     }
// }



using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;   // Stopwatch
using Unity.Profiling;      // Optional: shows a marker in the Unity Profiler

public class SpawnManyObjects : MonoBehaviour
{
    [Header("Input")]
    public Sprite cookieSprite;
    public RectTransform uiParent;            // assign TopFX (or a child Canvas)

    [Header("Spawn Settings")]
    public int numberOfCookies = 200;
    public Vector2 padding = new Vector2(40f, 40f);
    public Vector2 uniformScaleRange = new Vector2(0.05f, 0.12f);
    public float spawnInterval = 0.02f;       // seconds, realtime

    [Header("Metrics")]
    public int logEveryN = 50;                // print stats every N spawns
    public bool showOverlay = true;           // toggle HUD in Game view

    private Rect parentRect;

    // -------- timing stats (spawn cost only) --------
    private Stopwatch spawnSW = new Stopwatch();
    private double sumSpawnMs = 0.0;
    private double minSpawnMs = double.MaxValue;
    private double maxSpawnMs = 0.0;

    // -------- cadence stats (what the user feels) --------
    private double sumCadenceMs = 0.0;
    private double minCadenceMs = double.MaxValue;
    private double maxCadenceMs = 0.0;

    private int spawned = 0;
    private float wallStart;
    private float lastSpawnWall;

    // Profiler marker (shows “SpawnOneUI” samples in Profiler Timeline)
    private static readonly ProfilerMarker MarkerSpawnOne = new ProfilerMarker("SpawnOneUI");

    void Start()
    {
        if (cookieSprite == null || uiParent == null)
        {
            UnityEngine.Debug.LogError("Assign cookieSprite and uiParent (RectTransform) in the Inspector.");
            return;
        }

        parentRect = uiParent.rect;
        wallStart = Time.realtimeSinceStartup;
        lastSpawnWall = wallStart;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < numberOfCookies; i++)
        {
            // ---- cadence timing (wall time between appearances) ----
            float now = Time.realtimeSinceStartup;
            double cadenceMs = (now - lastSpawnWall) * 1000.0;
            if (spawned > 0)  // skip cadence for the very first item
            {
                sumCadenceMs += cadenceMs;
                if (cadenceMs < minCadenceMs) minCadenceMs = cadenceMs;
                if (cadenceMs > maxCadenceMs) maxCadenceMs = cadenceMs;
            }
            lastSpawnWall = now;

            // ---- pure spawn cost timing (no waiting) ----
            spawnSW.Restart();
            using (MarkerSpawnOne.Auto()) // shows in Profiler
            {
                SpawnOne();               // create/place the UI Image
            }
            spawnSW.Stop();

            double spawnMs = spawnSW.Elapsed.TotalMilliseconds;
            sumSpawnMs += spawnMs;
            if (spawnMs < minSpawnMs) minSpawnMs = spawnMs;
            if (spawnMs > maxSpawnMs) maxSpawnMs = spawnMs;

            spawned++;

            // periodic log
            if (spawned % logEveryN == 0)
                LogPartial();

            // delay to make it feel layered over time
            yield return new WaitForSecondsRealtime(spawnInterval);
        }

        // final summary
        float totalWall = Time.realtimeSinceStartup - wallStart;
        UnityEngine.Debug.Log(
            $"[SpawnManyObjects] DONE: count={spawned} | total={totalWall*1000f:F1} ms | " +
            $"avgSpawn={AvgSpawnMs():F3} ms | avgCadence={AvgCadenceMs():F3} ms");
    }

    // ----- actual work of creating a single UI image -----
    private void SpawnOne()
    {
        var go = new GameObject($"Cookie_{spawned}", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(uiParent, false);
        rt.SetAsLastSibling(); // newest on top

        var img = go.GetComponent<Image>();
        img.sprite = cookieSprite;
        img.raycastTarget = false;
        img.SetNativeSize();
        img.color = Color.white;

        float x = Random.Range(parentRect.xMin + padding.x, parentRect.xMax - padding.x);
        float y = Random.Range(parentRect.yMin + padding.y, parentRect.yMax - padding.y);
        rt.anchoredPosition = new Vector2(x, y);

        float s = Random.Range(uniformScaleRange.x, uniformScaleRange.y) * 0.01f; // 100× smaller
        rt.localScale = new Vector3(s, s, 1f);
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-12f, 12f));
    }

    private void LogPartial()
    {
        UnityEngine.Debug.Log(
            $"[SpawnManyObjects] n={spawned} | " +
            $"spawn(ms): avg={AvgSpawnMs():F3} min={minSpawnMs:F3} max={maxSpawnMs:F3} | " +
            $"cadence(ms): avg={AvgCadenceMs():F3} min={minCadenceMs:F3} max={maxCadenceMs:F3}");
    }

    private double AvgSpawnMs()   => spawned > 0 ? (sumSpawnMs / spawned) : 0.0;
    private double AvgCadenceMs() => (spawned > 1) ? (sumCadenceMs / (spawned - 1)) : 0.0;

    // ----- tiny HUD so you can see numbers live -----
    void OnGUI()
    {
        if (!showOverlay) return;

        const int w = 340, h = 90;
        GUI.depth = 0;
        var rect = new Rect(10, 10, w, h);
        GUI.Box(rect, "Spawn Metrics");
        var line = 28f;

        GUI.Label(new Rect(18, 34, w-16, 22),
            $"n={spawned}  |  avgSpawn={AvgSpawnMs():F3}ms  (min:{minSpawnMs:F3}  max:{maxSpawnMs:F3})");
        GUI.Label(new Rect(18, 34 + line, w-16, 22),
            $"avgCadence={AvgCadenceMs():F3}ms  (min:{minCadenceMs:F3}  max:{maxCadenceMs:F3})");
    }
}


