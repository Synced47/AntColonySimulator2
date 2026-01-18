using UnityEngine;

public class Ant
{
    public Vector2 position;
    public float angle;
    public bool carryingFood;
    public AntColony colony;
    public float noiseOffset;

    public Ant(AntColony colony, Vector2 position)
    {
        this.colony = colony;
        this.position = position;
        angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        carryingFood = false;
        noiseOffset = Random.value * 100f;
    }
}
