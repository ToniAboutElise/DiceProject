using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used to change background texture

public class TextureSetter : MonoBehaviour
{
    public Renderer targetRenderer;
    public Texture2D texture;

    public void SetTexture()
    {
        targetRenderer.material.SetTexture("_BaseMap", texture);
    }
}