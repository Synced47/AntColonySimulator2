using System.Collections.Generic;
using UnityEngine;

public class AntColony : MonoBehaviour
{
    
    public int numAnts = 1000;
    public AntSettings antSettings;
    
    public Ant[] ants;
    public int colonyFood = 0;

    private const float worldWidth = 200f;
    private const float worldHeight = 200f;
    private Vector2 colonyPosition;
    private PheromoneMap pheromoneMap = new();
    private List<PheromoneMap> rivalPheromoneMaps = new();

    void Awake()
    {
        colonyPosition = transform.position;
    }
    
    void Start()
    {
        InitialiseAnts();
        SetRivals();
    }
    
    void Update()
    {
        float dt = Time.deltaTime;
        FoodSource[] foodSources = FindObjectsByType<FoodSource>(FindObjectsSortMode.None);
        for (int i = 0; i < numAnts; i++)
        {
            Ant currentAnt = ants[i];
            
            HandleMovement(currentAnt, foodSources, dt);
            DepositPheromone(currentAnt);

            if (!currentAnt.carryingFood)
            {
                MoveTowardsFood(currentAnt, foodSources);
                PickUpFood(currentAnt, foodSources);
            }

            if (currentAnt.carryingFood && Vector2.Distance(currentAnt.position, colonyPosition) < 1f)
                DepositFood(currentAnt);
        }
        pheromoneMap.Evaporate(Mathf.Pow(antSettings.pheromoneDecayRate, dt));
    }

    private void HandleMovement(Ant ant, FoodSource[] foodSources, float dt)
    {
        if (ant.carryingFood)
            ant.angle = FollowReturnPheromone(ant, pheromoneMap.returningTrail);
        else 
            ant.angle = SearchForFood(ant, foodSources);
            
        float rivalPheromoneStrength = DetectRivalPheromones(ant);
        if (rivalPheromoneStrength > antSettings.rivalAvoidThreshold)
            ant.angle += Random.Range(-antSettings.turnSpeed*2f, antSettings.turnSpeed*2f);
            
        if (NearWorldEdge(ant.position))
        {
            Vector2 toColony = (colonyPosition - ant.position).normalized;
            ant.angle = Mathf.Atan2(toColony.y, toColony.x) + Random.Range(-0.5f, 0.5f);
        }

        // Less randomness when going home
        float wanderStrength = ant.carryingFood
            ? antSettings.randomMovement * 0.3f
            : antSettings.randomMovement;
        
        float noise = Mathf.PerlinNoise(Time.time * antSettings.wanderFrequency, ant.noiseOffset);
        float wander = (noise - 0.5f) * 2f * wanderStrength; // -0.5 => *2 to get negative values of the same magnitude
        ant.angle += wander;

        if (ant.carryingFood)
        {
            Vector2 toColony =  (colonyPosition - ant.position).normalized;
            float angleToColony = Mathf.Atan2(toColony.y, toColony.x);

            float newAngle = Mathf.LerpAngle(ant.angle, angleToColony, antSettings.returnBias);
            ant.angle = newAngle;
        }
        
        ant.position += new Vector2(Mathf.Cos(ant.angle), Mathf.Sin(ant.angle)) * (antSettings.speed * dt);
        ant.position.x = Mathf.Clamp(ant.position.x, 0, worldWidth);
        ant.position.y = Mathf.Clamp(ant.position.y, 0, worldHeight);
    }

    private bool NearWorldEdge(Vector2 position, float margin = 2.5f)
    {
        return position.x < margin || position.x > worldWidth - margin || position.y < margin || position.y > worldHeight - margin;
    }

    private void MoveTowardsFood(Ant ant, FoodSource[] foodSources)
    {
        foreach (FoodSource food in foodSources)
        {
            if (Vector2.Distance(ant.position, food.transform.position) < antSettings.senseDistance)
            {
                Vector2 toFood = (food.transform.position - (Vector3)ant.position).normalized;
                float angleToFood = Mathf.Atan2(toFood.y, toFood.x);
                
                ant.angle = Mathf.LerpAngle(ant.angle, angleToFood, antSettings.returnBias);

                break;
            }
        }
    }

    private void PickUpFood(Ant ant, FoodSource[] foodSources)
    {
        foreach (FoodSource food in foodSources)
        {
            if (Vector2.Distance(ant.position, food.transform.position) < food.pickupRadius)
            {
                if (food.TakeFood())
                {
                    ant.carryingFood = true;
                    
                    Vector2 toColony = (colonyPosition - ant.position).normalized;
                    float angleToColony = Mathf.Atan2(toColony.y, toColony.x);
                    
                    ant.angle = Mathf.LerpAngle(ant.angle, angleToColony, antSettings.returnBias);
                    
                    break;
                }
            }
        }
    }

    private void DepositFood(Ant ant)
    {
        ant.carryingFood = false;
        colonyFood++;
    }

    private void DepositPheromone(Ant ant)
    {
        int gridX = pheromoneMap.WorldToGridX(ant.position.x, worldWidth);
        int gridY = pheromoneMap.WorldToGridY(ant.position.y, worldHeight);

        if (ant.carryingFood)
            pheromoneMap.returningTrail[gridX, gridY] += antSettings.depositAmount;
        else
            pheromoneMap.searchingTrail[gridX, gridY] += antSettings.depositAmount;
    }

    private void InitialiseAnts()
    {
        ants = new Ant[numAnts];

        for (int i = 0; i < numAnts; i++)
        {
            Vector2 position = colonyPosition + Random.insideUnitCircle * 2f;
            ants[i] = new Ant(this, position);
        }
    }

    private float FollowReturnPheromone(Ant ant, float[,] map)
    {
        float forward = CheckMap(ant, 0, map);
        float left = CheckMap(ant, antSettings.turnAngle, map);
        float right = CheckMap(ant, -antSettings.turnAngle, map);
        
        if (left > forward && left > right) return ant.angle + antSettings.turnSpeed;
        if (right > forward && right > left) return ant.angle - antSettings.turnSpeed;
        return ant.angle;
    }

    private float CheckMap(Ant ant, float offsetAngle, float[,] map)
    {
        float angle = ant.angle + offsetAngle;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 checkPosition = ant.position + direction * antSettings.senseDistance;

        int gridX = pheromoneMap.WorldToGridX(checkPosition.x, worldWidth);
        int gridY = pheromoneMap.WorldToGridY(checkPosition.y, worldHeight);
        
        return map[gridX, gridY];
    }

    private float SearchForFood(Ant ant, FoodSource[] foodSources)
    {
        float foodSignal = CheckMap(ant, 0, pheromoneMap.searchingTrail);
        if (foodSignal > 0.01f) return FollowReturnPheromone(ant, pheromoneMap.searchingTrail);
        return ant.angle + Random.Range(-antSettings.randomMovement, antSettings.randomMovement);
    }

    private float DetectRivalPheromones(Ant ant)
    {
        float rivalPheromoneStrength = 0f;
        foreach (PheromoneMap rival in rivalPheromoneMaps)
        {
            int gridX = rival.WorldToGridX(ant.position.x, worldWidth);
            int gridY = rival.WorldToGridY(ant.position.y, worldHeight);
            
            rivalPheromoneStrength = Mathf.Max(rivalPheromoneStrength, rival.returningTrail[gridX, gridY]);
            rivalPheromoneStrength = Mathf.Max(rivalPheromoneStrength, rival.searchingTrail[gridX, gridY]);
        }
        return rivalPheromoneStrength;
    }

    private void SetRivals()
    {
        AntColony[] allColonies = FindObjectsByType<AntColony>(FindObjectsSortMode.None);
        foreach (AntColony colony in allColonies)
        {
            if (colony != this)
            {
                rivalPheromoneMaps.Add(colony.pheromoneMap);
            }
        }
    }
    
    
    void OnDrawGizmosSelected()
    {
        if (pheromoneMap == null)
        {
            Debug.LogError("Pheromone Map is null");
            return;
        }

        Debug.Log("Drawing Gizmos");
        int sampleStep = 8;
        float cellWidth = worldWidth / (float)PheromoneMap.Width;
        float cellHeight = worldHeight / (float)PheromoneMap.Height;

        for (int x = 0; x < PheromoneMap.Width; x += sampleStep)
        {
            for (int y = 0; y < PheromoneMap.Height; y += sampleStep)
            {
                float returning = pheromoneMap.returningTrail[x, y];
                float searching = pheromoneMap.searchingTrail[x, y];
                
                if (returning < 0.001f && searching < 0.001f) continue;
                
                Color c = new Color(returning, searching, 0f, Mathf.Max(returning, searching));
                Gizmos.color = c;
                
                float worldX = ((float)x / PheromoneMap.Width) * worldWidth;
                float worldY = ((float)y / PheromoneMap.Height) * worldHeight;
                
                Gizmos.DrawCube(new Vector3(worldX, worldY, 0.0f), new Vector3(cellWidth * sampleStep, cellHeight * sampleStep, 0.1f));
            }
        }
    }
}
