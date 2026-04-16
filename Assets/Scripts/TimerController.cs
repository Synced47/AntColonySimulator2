using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public Text timerText;
    public float lifetime;
    
    void Update()
    {
        lifetime += Time.deltaTime;
        timerText.text = lifetime.ToString("F2");
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Ant agents have mostly converged on the pheromone trail at " + lifetime + " seconds");
        }
    }
}
