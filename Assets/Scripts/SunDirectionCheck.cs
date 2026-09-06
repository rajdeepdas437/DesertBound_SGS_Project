using System.Collections;
using UnityEngine;

public class SunDirectionCheck : MonoBehaviour
{
    [SerializeField] GameObject directionalLight;
    [SerializeField] AutoHandPlayerControllerInputSimulator autoHandPlayerControllerInputSimulator;
    private float timelapseDuration = 4f;
    private float timeCounter;
    private bool playerHasEntered=false;
    [SerializeField] Transform Player;
    private Vector3 lockedPosition;
    [SerializeField] Canvas goalCanvas;

    void Start()
    {
        directionalLight.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        timeCounter = 0f;
    }

    void Update()
    {
        if(playerHasEntered && timeCounter<=timelapseDuration)
        {

            timeCounter+=Time.deltaTime;
            lockedPosition = Player.position;
            Player.position=lockedPosition;

            float currentX = Mathf.Lerp( 60f, 120f, timeCounter/timelapseDuration);

            directionalLight.transform.rotation = Quaternion.Euler(currentX, 0f, 0f); 
        }
        if(timeCounter>=timelapseDuration)
        {
            playerHasEntered=false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerHasEntered=true;
            goalCanvas.gameObject.SetActive(true);
        }
    }

    
}


