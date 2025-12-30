using UnityEngine;

public class NpcDestroy : MonoBehaviour
{
    private Transform playerTransform;
    private float destroyDistance = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        if (transform.position.z < playerTransform.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
