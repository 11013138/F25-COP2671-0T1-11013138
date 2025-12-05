using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // variables
    [SerializeField] private float moveSpeed = 5f; 

    private Rigidbody2D _rigidbody; 
    private Vector2 _movement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get rigidbody component
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // get input from arrow keys or WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // set movement direction, diagonal input 
        _movement = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        // set rigidbody velocity
        _rigidbody.linearVelocity = _movement * moveSpeed;
    }
}
