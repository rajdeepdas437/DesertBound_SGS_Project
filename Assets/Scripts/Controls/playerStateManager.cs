using UnityEngine;

public class playerStateManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public playerBaseState currentState;
    public playerDialogueState dialogueState = new playerDialogueState();
    public playerNormalState normalState = new playerNormalState();
    public playerObserveState observeState = new playerObserveState();
    void Start()
    {
        playerBaseState.controls = GetComponent<ExtraPlayerControls>();
        currentState = normalState;
        currentState.enterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.updateState(this);
    }

    void swtchState(playerBaseState state)
    {
        currentState = state;
        currentState.enterState(this);
    }
}
