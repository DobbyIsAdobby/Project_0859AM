using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Setting")]
    public float dmg = 10f;
    public float speed = 15f;

    [Header("EasterEgg Setting")]
    public bool isEasterEggKey = false;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VehicleHP playerHealth = other.GetComponent<VehicleHP>();
            if (playerHealth != null)
            {
                playerHealth.TakeDmg(dmg);
            }
            if (isEasterEggKey)
            {
                //구현은 나중에
            }

            Destroy(gameObject);
        }
    }
}
