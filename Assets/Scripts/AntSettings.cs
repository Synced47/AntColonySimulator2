using UnityEngine;

[CreateAssetMenu(fileName = "AntSettings", menuName = "Ant/Settings")]
public class AntSettings : ScriptableObject
{
    public float speed = 6f;
    public float turnSpeed = 0.15f;
    public float turnAngleDegrees = 45f;
    
    public float returnBias = 0.7f;
    
    public float senseDistance = 3f;
    
    public float depositAmount = 0.05f;
    public float pheromoneDecayRate = 0.95f;
    public float rivalAvoidThreshold = 0.05f;

    public float randomMovement = 0.08f;
    public float wanderFrequency = 0.5f;
    
    public float turnAngle => turnAngleDegrees * Mathf.Deg2Rad;
}
