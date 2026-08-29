using Autohand;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtraPlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    public VRStaminaManager staminabar;
    public float jumpForce = 5f;
    public float allowable_speed_sprint;
    public float allowable_speed_walk;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        JumpTrigger();
        Run();
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && staminabar.currentStamina > 5f)
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
        if (Input.GetKeyDown(KeyCode.Space) && staminabar.currentStamina > 7f)
        {
            AutoHandPlayer.Instance.Jump(jumpForce);
            staminabar.currentStamina -=7f;
        }
    }
}
