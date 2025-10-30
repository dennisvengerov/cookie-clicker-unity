using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

public class Runtime_Metrics_Sampler : MonoBehaviour
{
    public int warmup_seconds = 2;
    public int sample_seconds = 8;
    public int target_frame_rate = 60;

    private long start_total_memory_bytes;
    private long end_total_memory_bytes;
    private readonly List<float> frame_times_ms = new List<float>();

    private void Awake()
    {
        Application.targetFrameRate = target_frame_rate;
        QualitySettings.vSyncCount = 0;
    }

    private IEnumerator Start()
    {
        yield return null; // let scene initialize
        start_total_memory_bytes = Profiler.GetTotalAllocatedMemoryLong();

        float warmup_end_time = Time.time + warmup_seconds;
        while (Time.time < warmup_end_time) { yield return null; }

        float sample_end_time = Time.time + sample_seconds;
        while (Time.time < sample_end_time)
        {
            frame_times_ms.Add(Time.deltaTime * 1000f);
            yield return null;
        }

        end_total_memory_bytes = Profiler.GetTotalAllocatedMemoryLong();

        frame_times_ms.Sort();
        float sum_ms = 0f;
        for (int i = 0; i < frame_times_ms.Count; i++) sum_ms += frame_times_ms[i];
        float average_frame_time_ms = (frame_times_ms.Count > 0) ? (sum_ms / frame_times_ms.Count) : 0f;
        float average_fps = 1000f / Mathf.Max(0.0001f, average_frame_time_ms);
        int index_p99 = Mathf.Clamp(Mathf.FloorToInt(0.99f * (frame_times_ms.Count - 1)), 0, Mathf.Max(0, frame_times_ms.Count - 1));
        float p99_frame_time_ms = (frame_times_ms.Count > 0) ? frame_times_ms[index_p99] : 0f;

        long runtime_memory_delta_bytes = (end_total_memory_bytes - start_total_memory_bytes);

        string json_line = $"{{\"average_fps\":{average_fps:F2},\"avg_frame_time_ms\":{average_frame_time_ms:F2},\"p99_frame_time_ms\":{p99_frame_time_ms:F2},\"runtime_memory_delta_bytes\":{runtime_memory_delta_bytes}}}";
        Debug.Log("METRICS_JSON:" + json_line);

        string directory_path = Application.persistentDataPath;
        string file_path = Path.Combine(directory_path, "metrics.json");
        try { File.WriteAllText(file_path, json_line); Debug.Log("metrics_file_path:" + file_path); }
        catch (System.Exception e) { Debug.LogWarning("metrics_file_write_failed:" + e.Message); }

#if !UNITY_EDITOR
        Application.Quit(); // auto-exit after sampling in builds
#endif
    }
}