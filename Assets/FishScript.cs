using UnityEngine;

public class FishScript : MonoBehaviour
{
    [SerializeField] private float _velocity = 1.5f;
    [SerializeField] private float _rotationSpeed = 10f;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_rb != null)
                _rb.linearVelocity = Vector2.up * _velocity;
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null) return;
        transform.rotation = Quaternion.Euler(0f, 0f, _rb.linearVelocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthManager hm = HealthManager.instance;
        if (hm == null)
        {
            hm = UnityEngine.Object.FindAnyObjectByType<HealthManager>();
            if (hm != null)
            {
                HealthManager.instance = hm;
            }
        }

        if (hm != null)
        {
            hm.TakeDamage();
        }
    }
}