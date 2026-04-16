using System;
using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public int foodAmount = 100;
    public float pickupRadius = 1f;
    public float lifetime;

    private void Update()
    {
        lifetime += Time.deltaTime;
    }

    public bool TakeFood()
    {
        if (foodAmount <= 0)
            return false;
        foodAmount--;
        if (foodAmount == 0)
        {
            Debug.Log(name + " destroyed at " + lifetime + " seconds");
            Destroy(gameObject);
        }
        return true;
    }
}
