using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelateEffect : MonoBehaviour
{
    public Shader pixelateShader;
    [Range(50, 500)]
    public float pixelDensity = 100;

    private Material pixelateMaterial;

    void Start()
    {
        if (!pixelateShader)
        {
            Debug.LogError("Please assign a pixelate shader in the inspector");
            enabled = false;
            return;
        }

        pixelateMaterial = new Material(pixelateShader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (pixelateMaterial != null)
        {
            pixelateMaterial.SetFloat("_PixelDensity", pixelDensity);

            // Duplica el renderizado en un solo paso para ambas lentes en VR
            RenderTexture tempRender = RenderTexture.GetTemporary(src.width, src.height, 0);
            Graphics.Blit(src, tempRender, pixelateMaterial);
            Graphics.Blit(tempRender, dest);
            RenderTexture.ReleaseTemporary(tempRender);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    void OnDestroy()
    {
        if (pixelateMaterial)
        {
            DestroyImmediate(pixelateMaterial);
        }
    }
}