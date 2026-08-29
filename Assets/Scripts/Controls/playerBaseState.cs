using UnityEngine;

public abstract class playerBaseState 
{
    public static ExtraPlayerControls controls ;
    public abstract void enterState(playerStateManager player);
    public abstract void updateState(playerStateManager player);


}
