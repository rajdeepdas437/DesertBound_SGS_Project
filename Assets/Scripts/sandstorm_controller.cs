using UnityEngine;

public class SandstormController : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem sandParticleSystem;
    public Color sandstormFogColor = new Color(0.8f, 0.6f, 0.3f);
    public float targetFogDensity = 0.08f;
    public float fogTransitionSpeed = 0.5f;

    private bool isStormActive = false;

    void Start()
    {
        // Make sure particle storm is turned off at start
        if (sandParticleSystem != null)
        {
            sandParticleSystem.Stop();
        }
    }

    void Update()
    {
        // Smoothly fade in Unity Environment Fog during the storm
        if (isStormActive)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, sandstormFogColor, Time.deltaTime * fogTransitionSpeed);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetFogDensity, Time.deltaTime * fogTransitionSpeed);
        }
    }

    // Call this method from the CaravanLeader's event
    public void StartSandstorm()
    {
        Debug.Log("THE STORM HAS BEGUN!");
        isStormActive = true;

        if (sandParticleSystem != null)
        {
            sandParticleSystem.Play();
        }
    }
}