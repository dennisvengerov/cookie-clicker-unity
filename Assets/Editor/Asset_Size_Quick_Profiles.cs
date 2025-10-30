using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Asset_Size_Quick_Profiles
{
    // Your art lives under the nested Assets folder:
    private const string assets_root_glob = "Assets/Assets";

    private const string product_name_medium = "CookieClicker_Medium";
    private const string product_name_large  = "CookieClicker_Large";

    [MenuItem("Asset Size/Apply Medium Profile")]
    public static void Apply_Medium_Profile() => Apply_Profile(
        max_texture_size: 1024,
        texture_compression: TextureImporterCompression.Compressed,     // compressed & smaller
        platform_name: Platform_Name(),
        platform_texture_format_for_medium: TextureImporterFormat.DXT5, // BC3 on desktop
        audio_sample_rate: 32000,
        audio_quality_0_to_1: 0.45f,
        audio_uncompressed: false,
        mesh_compression_level_0_to_3: 2,
        mesh_read_write_enabled: false,
        enable_mipmaps: false
    );

    [MenuItem("Asset Size/Apply Large Profile")]
    public static void Apply_Large_Profile() => Apply_Profile(
        max_texture_size: 4096,
        texture_compression: TextureImporterCompression.Uncompressed,    // RGBA32 → big VRAM
        platform_name: Platform_Name(),
        platform_texture_format_for_medium: TextureImporterFormat.DXT5,  // ignored for Large
        audio_sample_rate: 44100,
        audio_quality_0_to_1: 0.70f,
        audio_uncompressed: true,                                        // PCM → bigger on disk
        mesh_compression_level_0_to_3: 0,
        mesh_read_write_enabled: true,                                   // extra RAM copy
        enable_mipmaps: true                                             // larger residency
    );

    [MenuItem("Asset Size/Build/Build Medium (Current Scene)")]
    public static void Build_Medium() => Build_With_Profile(is_large: false);

    [MenuItem("Asset Size/Build/Build Large (Current Scene)")]
    public static void Build_Large() => Build_With_Profile(is_large: true);

    private static void Build_With_Profile(bool is_large)
    {
        if (is_large) Apply_Large_Profile(); else Apply_Medium_Profile();

        string product_name = is_large ? product_name_large : product_name_medium;
        PlayerSettings.productName = product_name;

        string output_path = $"Builds/{product_name}";
#if UNITY_EDITOR_OSX
        string exe_path = $"{output_path}/{product_name}.app";
        BuildTarget build_target = BuildTarget.StandaloneOSX;
#elif UNITY_EDITOR_WIN
        string exe_path = $"{output_path}/{product_name}.exe";
        BuildTarget build_target = BuildTarget.StandaloneWindows64;
#else
        string exe_path = $"{output_path}/{product_name}.apk";
        BuildTarget build_target = BuildTarget.Android;
#endif
        var build_options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = exe_path,
            target = build_target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(build_options);
        Debug.Log($"build.result={report.summary.result}");
        Debug.Log($"build.total_size_bytes={report.summary.totalSize}");
        Debug.Log($"build.output_path={exe_path}");
    }

    private static void Apply_Profile(
        int max_texture_size,
        TextureImporterCompression texture_compression,
        string platform_name,
        TextureImporterFormat platform_texture_format_for_medium,
        int audio_sample_rate,
        float audio_quality_0_to_1,
        bool audio_uncompressed,
        int mesh_compression_level_0_to_3,
        bool mesh_read_write_enabled,
        bool enable_mipmaps)
    {
        // TEXTURES
        int textures_changed = 0;
        string[] texture_guids = AssetDatabase.FindAssets($"t:Texture2D {assets_root_glob}");
        foreach (string asset_guid in texture_guids)
        {
            string asset_path = AssetDatabase.GUIDToAssetPath(asset_guid);
            if (!(AssetImporter.GetAtPath(asset_path) is TextureImporter texture_importer)) continue;

            texture_importer.maxTextureSize = max_texture_size;
            texture_importer.textureCompression = texture_compression;
            texture_importer.mipmapEnabled = enable_mipmaps;
            texture_importer.isReadable = mesh_read_write_enabled;

            var platform_settings = new TextureImporterPlatformSettings
            {
                name = platform_name, // "Standalone"
                overridden = true,
                maxTextureSize = max_texture_size,
                format = (texture_compression == TextureImporterCompression.Uncompressed)
                    ? TextureImporterFormat.RGBA32   // truly uncompressed on GPU
                    : platform_texture_format_for_medium
            };
            texture_importer.SetPlatformTextureSettings(platform_settings);

            EditorUtility.SetDirty(texture_importer);
            textures_changed++;
        }

        // AUDIO
        int audio_changed = 0;
        string[] audio_guids = AssetDatabase.FindAssets($"t:AudioClip {assets_root_glob}");
        foreach (string asset_guid in audio_guids)
        {
            string asset_path = AssetDatabase.GUIDToAssetPath(asset_guid);
            if (!(AssetImporter.GetAtPath(asset_path) is AudioImporter audio_importer)) continue;

            var sample_settings = audio_importer.defaultSampleSettings;
            sample_settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            sample_settings.sampleRateOverride = (uint)audio_sample_rate;
            sample_settings.quality = Mathf.Clamp01(audio_quality_0_to_1);
            sample_settings.compressionFormat = audio_uncompressed ? AudioCompressionFormat.PCM : AudioCompressionFormat.Vorbis;

            audio_importer.defaultSampleSettings = sample_settings;
            EditorUtility.SetDirty(audio_importer);
            audio_changed++;
        }

        // MODELS (safe to run even if none)
        int models_changed = 0;
        string[] model_guids = AssetDatabase.FindAssets($"t:Model {assets_root_glob}");
        foreach (string asset_guid in model_guids)
        {
            string asset_path = AssetDatabase.GUIDToAssetPath(asset_guid);
            if (!(AssetImporter.GetAtPath(asset_path) is ModelImporter model_importer)) continue;

            model_importer.meshCompression = (ModelImporterMeshCompression)Mathf.Clamp(mesh_compression_level_0_to_3, 0, 3);
            model_importer.isReadable = mesh_read_write_enabled;
            EditorUtility.SetDirty(model_importer);
            models_changed++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"asset_size_profile_applied textures_changed={textures_changed} audio_changed={audio_changed} models_changed={models_changed}");
    }

    private static string Platform_Name()
    {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
        return "Standalone";
#else
        return "Android";
#endif
    }
}