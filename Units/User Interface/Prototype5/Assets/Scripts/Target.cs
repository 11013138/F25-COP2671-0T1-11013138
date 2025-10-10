using JetBrains.Annotations;
using UnityEngine;

public class Target : MonoBehaviour
{
    // variables
    private Rigidbody targetRb;
    private GameManager gameMangager;
    public ParticleSystem explosionParticle;

    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float maxTorque = 10;
    private float xRange = 4;
    private float ySpawnPos = -6;

    public int pointValue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();

        gameMangager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // upward force multiplied by random speed
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);

        // torque with randomized xyz values
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        // position with randomized x value
        transform.position = RandomSpawnPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }

    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }

    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos);
    }

    private void OnMouseDown()
    {
        if (gameMangager.isGameActive)
        {
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            gameMangager.UpdateScore(pointValue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

        if (!gameObject.CompareTag("Bad"))
        {
            gameMangager.GameOver();
        }
    }
}

