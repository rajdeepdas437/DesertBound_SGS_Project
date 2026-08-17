using Autohand;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtraPlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] float jumpForce = 5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        JumpTrigger();
        Run();
    }

    private static void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            AutoHandPlayer.Instance.maxMoveSpeed = 5f;
        }
        else
        {
            AutoHandPlayer.Instance.maxMoveSpeed = 1.5f;
        }
    }

    private void JumpTrigger()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AutoHandPlayer.Instance.Jump(jumpForce);
        }
    }
}
