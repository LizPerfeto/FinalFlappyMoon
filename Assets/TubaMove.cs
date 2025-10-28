using UnityEngine;

public class TubaMove : MonoBehaviour
{
    public float moveTuba = 1;
    public float DeleteTuba = -15;
    void Start()
    {
        
    }

  
    void Update()
    {
        transform.position = transform.position + (Vector3.left * moveTuba)* Time.deltaTime;  

        if(transform.position.x < DeleteTuba)
        {
            Destroy(gameObject);
        }
    }
}
