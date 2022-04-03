using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    public RenderTexture input;

    private Material camTextureHolder;
    public Material shaderMaterial;

    [Range(1.0f, 2.0f)]
    public float FOV = 1.6f;
    [Range(0.0f, 0.3f)]
    public float Disparity = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        
        camTextureHolder = new Material(shaderMaterial);
        camTextureHolder.mainTexture = input;

        float Alpha = (float)1080 / (float)Screen.height * (float)Screen.width * 0.5f / (float)1920;
        shaderMaterial.SetFloat("_Alpha", Alpha);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Config.CONSOLE)
        {
            Graphics.Blit(input, dest = null);
        }
        else
        {
            // shaderMaterial renders the image with Barrel distortion and disparity effect
            Graphics.Blit(camTextureHolder.mainTexture, null, shaderMaterial);
        }

    }

    void OnGUI()
    {
        if (Config.CONSOLE)
        {
            return;
        }

        int labelHeight = 40;
        int boundary = 20;

        GUI.Label(new Rect(Screen.width - boundary - 200, boundary, 200, labelHeight), "FOV");
        FOV = GUI.HorizontalSlider(new Rect(Screen.width - boundary - 200, boundary + labelHeight, 200, labelHeight), FOV, 1.0F, 2.0F);
        shaderMaterial.SetFloat("_FOV", FOV);

        GUI.Label(new Rect(boundary, boundary, 200, labelHeight), "Disparity");
        Disparity = GUI.HorizontalSlider(new Rect(boundary, boundary + labelHeight, 200, labelHeight), Disparity, 0.0F, 0.3F);
        shaderMaterial.SetFloat("_Disparity", Disparity);
    }
}
