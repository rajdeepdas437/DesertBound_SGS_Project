using UnityEngine;

public class BS_CONTROL_SCRIPT : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    public float speed = 3;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("w"))
        {
            animator.SetBool("is_walking" , true);
            transform.position += Vector3.forward*speed*Time.deltaTime;
        }
        else
        {
            animator.SetBool("is_walking" , false);
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("is_running" , true);
            transform.position += Vector3.forward*speed*2*Time.deltaTime;
        }
        else
        {
            animator.SetBool("is_running" , false);
        }
    }
}
