using UnityEngine;

public class AnimationController : MonoBehaviour
{   
    private Animator animator; // Reference to the Animator component
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("isMovingUp", true);
        }
        if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("isMovingUp", false);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isMovingDown", true);
        }
        if(!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isMovingDown", false);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("isMovingLeft", true);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("isMovingLeft", false);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("isMovingRight", true);
        }
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("isMovingRight", false);
        }
    }
}
