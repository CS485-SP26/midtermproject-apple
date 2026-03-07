using UnityEngine;
using Environment;

public class SeasonalParticleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private SeasonManager seasonManager;
    [SerializeField] private DayController dayController;

    [Header("Season Presets")]
    [SerializeField] private ParticleSystem springParticles;
    [SerializeField] private ParticleSystem summerParticles;
    [SerializeField] private ParticleSystem fallParticles;
    [SerializeField] private ParticleSystem winterParticles;

    private ParticleSystem currentPreset;

    void LateUpdate()
    {
        if (player == null) return;

        // Follow player
        transform.position = player.position;
    }

    public void UpdateSeason(SeasonManager.Season season)
    {
        ParticleSystem preset = null;

        switch (season)
        {
            case SeasonManager.Season.Spring:
                preset = springParticles;
                break;
            case SeasonManager.Season.Summer:
                preset = summerParticles;
                break;
            case SeasonManager.Season.Fall:
                preset = fallParticles;
                break;
            case SeasonManager.Season.Winter:
                preset = winterParticles;
                break;
        }

        ApplyPreset(preset);
    }

    void ApplyPreset(ParticleSystem preset)
    {
        if (preset == null || particleSystem == null) return;

        currentPreset = preset;

        var main = particleSystem.main;
        var presetMain = preset.main;

        main.startColor = presetMain.startColor;
        main.startSize = presetMain.startSize;
        main.startLifetime = presetMain.startLifetime;

        var emission = particleSystem.emission;
        emission.rateOverTime = preset.emission.rateOverTime;

        particleSystem.Play();
    }

    void Update()
    {
        // Only show sun rays during daytime
        if (seasonManager.RuntimeData != null &&
            seasonManager.RuntimeData.avgTemp > 80) // simple "summer check"
        {
            var emission = particleSystem.emission;

            if (dayController.DayProgressPercent > 0.25f &&
                dayController.DayProgressPercent < 0.75f)
                emission.enabled = true;
            else
                emission.enabled = false;
        }
    }
}