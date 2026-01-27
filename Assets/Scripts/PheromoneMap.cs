using UnityEngine;

public class PheromoneMap
{
    public const int Width = 256;
    public const int Height = 256;
    
    public float[,] returningTrail = new float[Width, Height];
    public float[,] searchingTrail = new float[Width, Height];

    public void Evaporate(float rate)
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                returningTrail[x, y] *= rate;
                searchingTrail[x, y] *= rate;
            }
    }
    
    public int WorldToGridX(float x, float worldWidth) => Mathf.Clamp((int)((x / worldWidth) * Width), 0, Width - 1);
    public int WorldToGridY(float y, float worldHeight) => Mathf.Clamp((int)((y / worldHeight) * Height), 0, Height - 1);
}
