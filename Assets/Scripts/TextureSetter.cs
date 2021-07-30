using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSetter : MonoBehaviour
{
    public Renderer targetRenderer;
    public Texture2D texture;

    public void SetTexture()
    {
        targetRenderer.material.SetTexture("_BaseMap", texture);
    }
}