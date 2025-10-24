using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // show in editor without being public
    [SerializeField] private float moveSpeed = 5f; // movement speed

    private Rigidbody2D _rigidbody; // reference Rigidbody 2D
    private Vector2 _movement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get Rigidbody2D component
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from arrow keys or WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Set movement direction, diagonal input accounted for
        _movement = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        // Set Rigidbody2D's velocity
        _rigidbody.linearVelocity = _movement * moveSpeed;
    }
}
