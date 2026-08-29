using UnityEngine;

public class playerObserveState : playerBaseState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void enterState(playerStateManager player)
    {
        controls.allowable_speed_sprint = 0.0f;
        controls.allowable_speed_walk = 0f;
    }
    public override void updateState(playerStateManager player)
    {
        
    }
}
