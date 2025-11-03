using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ShaderComplexityMvp : MonoBehaviour
{
    public Material stress_material;
    public int initial_layer_count = 32;
    public int operation_count = 200;
    public int texture_sample_count = 2;
    public float branch_factor = 0.2f;

    private readonly List<GameObject> layer_objects = new List<GameObject>();
    private int shader_id_operation_count, shader_id_texture_sample_count, shader_id_branch_factor;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        shader_id_operation_count = Shader.PropertyToID("_operation_count");
        shader_id_texture_sample_count = Shader.PropertyToID("_texture_sample_count");
        shader_id_branch_factor = Shader.PropertyToID("_branch_factor");

        Apply_shader_params();
        Set_layer_count(initial_layer_count);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Overdraw (layer count): W = more layers, S = fewer layers
        if (keyboard.wKey.wasPressedThisFrame) Set_layer_count(layer_objects.Count + 8);
        if (keyboard.sKey.wasPressedThisFrame) Set_layer_count(Mathf.Max(1, layer_objects.Count - 8));

        // ALU work (math in fragment): Up/Down arrows
        if (keyboard.upArrowKey.wasPressedThisFrame)   operation_count += 50;
        if (keyboard.downArrowKey.wasPressedThisFrame) operation_count = Mathf.Max(0, operation_count - 50);

        // Texture pressure (reads per pixel): Left/Right arrows
        if (keyboard.rightArrowKey.wasPressedThisFrame) texture_sample_count += 1;
        if (keyboard.leftArrowKey.wasPressedThisFrame)  texture_sample_count = Mathf.Max(0, texture_sample_count - 1);

        Apply_shader_params();
    }

    private void Apply_shader_params()
    {
        if (stress_material == null) return;
        stress_material.SetInt(shader_id_operation_count, operation_count);
        stress_material.SetInt(shader_id_texture_sample_count, texture_sample_count);
        stress_material.SetFloat(shader_id_branch_factor, branch_factor);
    }

    private void Set_layer_count(int count)
    {
        while (layer_objects.Count < count) layer_objects.Add(Create_fullscreen_sprite_layer());
        while (layer_objects.Count > count)
        {
            var go = layer_objects[layer_objects.Count - 1];
            layer_objects.RemoveAt(layer_objects.Count - 1);
            Destroy(go);
        }
    }

    private GameObject Create_fullscreen_sprite_layer()
    {
        // make a sprite renderer layer (plays nice with your 2D sorting)
        var go = new GameObject("ShaderStressLayer");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,2,2), new Vector2(0.5f,0.5f), 100f);
        sr.material = stress_material;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 5000; // above your cookie/ui (if Canvas is Overlay it still draws on top; that’s OK)

        go.transform.position = new Vector3(0, 0, 0);
        go.transform.localScale = new Vector3(20f, 20f, 1f); // fills ortho Size=5

        // tiny z jitter so blending order varies (more overdraw)
        var p = go.transform.position;
        p.z = Random.Range(-0.1f, 0.1f);
        go.transform.position = p;
        return go;
    }
}