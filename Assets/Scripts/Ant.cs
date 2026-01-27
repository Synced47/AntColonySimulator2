using UnityEngine;

public class Ant
{
    public Vector2 position;
    public float angle;
    public bool carryingFood;
    public AntColony colony;
    public float noiseOffset;
    public float spawnTime;

    public Ant(AntColony colony, Vector2 position, float angle)
    {
        this.colony = colony;
        this.position = position;
        this.angle = angle;
        carryingFood = false;
        noiseOffset = Random.value * 100f;
    }
}
