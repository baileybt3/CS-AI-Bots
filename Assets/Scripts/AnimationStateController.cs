using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float decceleration = 0.5f;
    int VelocityHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        VelocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
               

        if (forwardPressed && (velocity < 1.0f))
        {
            velocity += Time.deltaTime * acceleration;
        }

        if (!forwardPressed && (velocity > 0.0)) 
        {
            velocity -= Time.deltaTime * decceleration;
        }

        if (!forwardPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }
        
        animator.SetFloat(VelocityHash, velocity);
    }
}
