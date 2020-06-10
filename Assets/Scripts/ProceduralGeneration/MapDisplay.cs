using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer;

    [SerializeField] private Texture2D targetTexture;

    public void DrawNoiseMap(float[,] NoiseMap)
    {
        int _Width = NoiseMap.GetLength(0);
        int _Height = NoiseMap.GetLength(1);

        Color[] _ColorMap = new Color[_Width * _Height];

        for (int y = 0; y < _Height; y++)
        {
            for (int x = 0; x < _Width; x++)
            {
                _ColorMap[y * _Width + x] = Color.Lerp(Color.black, Color.white, NoiseMap[x, y]);
            }
        }

        targetTexture.Resize(_Width, _Height);
        targetTexture.SetPixels(_ColorMap);
        targetTexture.Apply();

       // textureRenderer.transform.localScale = new Vector3(_Width, 1, _Height);
    }
}
