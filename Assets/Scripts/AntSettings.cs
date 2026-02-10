using UnityEngine;

[CreateAssetMenu(fileName = "AntSettings", menuName = "Ant/Settings")]
public class AntSettings : ScriptableObject
{
    public float speed = 1f;
    public float turnSpeed = 0.1f;
    public float turnAngleDegrees = 30f;
    
    public float returnBias = 0.6f;
    
    public float senseDistance = 2f;
    public float obstacleAvoidanceDistance = 0.75f;
    public LayerMask obstacleLayerMask;
    
    public float depositAmount = 0.001f;
    public float depositAmount_Food = 0.005f;
    public float pheromoneDepositDelay = 2f;
    public float pheromoneDecayRate = 0.97f;
    public float pheromoneThreshold = 0.001f;
    public float rivalAvoidThreshold = 0.05f;

    public float randomMovement = 0.015f;
    public float wanderFrequency = 0.8f;
    
    public float turnAngle => turnAngleDegrees * Mathf.Deg2Rad;
}
