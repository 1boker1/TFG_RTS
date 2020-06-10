using UnityEngine;

namespace Assets.Scripts.ProceduralGeneration
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int Width, int Height, int Seed, float Scale, int Octaves, float Persistance, float Lacunarity, Vector2 Offset)
        {
            float[,] _NoiseMap = new float[Width, Height];

            System.Random _RandomNumber = new System.Random(Seed);

            Vector2[] _OctaveOffsets = new Vector2[Octaves];

            for (int i = 0; i < Octaves; i++)
            {
                float _OffsetX = _RandomNumber.Next(-100000, 100000) + Offset.x;
                float _OffsetY = _RandomNumber.Next(-100000, 100000) + Offset.y;

                _OctaveOffsets[i] = new Vector2(_OffsetX, _OffsetY);
            }

            if (Scale <= 0)
                Scale = 0.001f;

            float _MaxNoiseHeight = float.MinValue;
            float _MinNoiseHeight = float.MaxValue;

            float _HalfWidth = Width * 0.5f;
            float _HalfHeight = Height * 0.5f;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float _Amplitude = 1;
                    float _Frequency = 1;

                    float _NoiseHeight = 0;

                    for (int i = 0; i < Octaves; i++)
                    {
                        float _SampleX = (x - _HalfWidth) / Scale * _Frequency + _OctaveOffsets[i].x;
                        float _SampleY = (y - _HalfHeight) / Scale * _Frequency + _OctaveOffsets[i].y;

                        float _PerlinValue = Mathf.PerlinNoise(_SampleX, _SampleY) * 2 - 1;

                        _NoiseHeight += _PerlinValue * _Amplitude;

                        _Amplitude *= Persistance;
                        _Frequency *= Lacunarity;
                    }

                    if (_NoiseHeight > _MaxNoiseHeight)
                        _MaxNoiseHeight = _NoiseHeight;
                    else if (_NoiseHeight < _MinNoiseHeight)
                        _MinNoiseHeight = _NoiseHeight;

                    _NoiseMap[x, y] = _NoiseHeight;
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _NoiseMap[x, y] = Mathf.InverseLerp(_MinNoiseHeight, _MaxNoiseHeight, _NoiseMap[x, y]);
                }
            }

            return _NoiseMap;
        }

        public static float[,] GenerateSimpleNoiseMap(int Width, int Height, int Seed, float Scale)
        {
            float[,] _NoiseMap = new float[Width, Height];

            if (Scale <= 0)
                Scale = 0.001f;

            float _MaxNoiseHeight = float.MinValue;
            float _MinNoiseHeight = float.MaxValue;

            float _HalfWidth = Width * 0.5f;
            float _HalfHeight = Height * 0.5f;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float _PerlinValue = Mathf.PerlinNoise(x, y) * 2 - 1;

                    _NoiseMap[x, y] = _PerlinValue;
                }
            }

            return _NoiseMap;
        }
    }
}
