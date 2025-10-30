using UnityEngine;

public class Asset_Preload : MonoBehaviour
{
    private Texture2D[] loaded_textures;

    private void Start()
    {
        // Load every texture in Assets/Resources/Stress/
        loaded_textures = Resources.LoadAll<Texture2D>("Stress");

        // Touch each texture so Unity ensures data is uploaded and resident
        for (int texture_index = 0; texture_index < loaded_textures.Length; texture_index++)
        {
            Texture2D current_texture = loaded_textures[texture_index];
            if (current_texture == null) { continue; }
            int force_upload = current_texture.width; // access forces residency
        }

        Debug.Log($"preload.loaded_textures_count={loaded_textures.Length}");
    }
}