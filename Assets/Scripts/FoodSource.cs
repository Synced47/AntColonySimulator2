using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public int foodAmount = 100;
    public float pickupRadius = 1f;

    public bool TakeFood()
    {
        if (foodAmount <= 0)
            return false;
        foodAmount--;
        if (foodAmount == 0)
            Destroy(gameObject);
        return true;
    }
}
