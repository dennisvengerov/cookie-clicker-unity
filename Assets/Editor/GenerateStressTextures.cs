using System.IO;
using UnityEditor;
using UnityEngine;

public static class Generate_Stress_Textures
{
    [MenuItem("Asset Size/Generate Dummy 4K Textures (3)")]
    public static void Generate_Textures()
    {
        string resources_dir = "Assets/Resources/Stress";
        Directory.CreateDirectory(resources_dir);

        for (int index = 0; index < 128; index++)
        {
            int width = 4096;
            int height = 4096;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color32[width * height];

            // Solid but different colors
            Color32 color = index switch
            {
                0 => new Color32(255, 64, 64, 255),
                1 => new Color32(64, 255, 64, 255),
                _ => new Color32(64, 64, 255, 255)
            };
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            byte[] png = texture.EncodeToPNG();
            string path = Path.Combine(resources_dir, $"stress_{index + 1}_4k.png");
            File.WriteAllBytes(path, png);
            Object.DestroyImmediate(texture);
            Debug.Log("generated_texture:" + path);
        }

        AssetDatabase.Refresh();
    }
}