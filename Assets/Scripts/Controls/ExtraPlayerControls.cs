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
    public InputActionReference jumper;
    public InputActionReference sprinter;
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
        bool legacyShift = Input.GetKey(KeyCode.LeftShift);
        bool newAction = sprinter.action.IsPressed();

        if (legacyShift || newAction) {
        // Sprint logic
        }   
        if ((legacyShift || newAction) && staminabar.currentStamina > 5f)
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
        bool legacybar  = (Input.GetKeyDown(KeyCode.Space));
        bool newbar = jumper.action.IsPressed();
        if ( (legacybar || newbar) && staminabar.currentStamina > 7f)
        {
            AutoHandPlayer.Instance.Jump(jumpForce);
            staminabar.currentStamina -=7f;
        }
    }
}
