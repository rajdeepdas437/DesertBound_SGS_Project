using UnityEngine;

public class playerNormalState : playerBaseState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void enterState(playerStateManager player)
    {
        controls.allowable_speed_sprint = 5.0f;
        controls.allowable_speed_walk = 1.5f;
    }
    public override void updateState(playerStateManager player)
    {
        
    }
}
