using UnityEngine;
using Character;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Core;
using Environment;
namespace Farming
{
    [RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private GameObject gardenHoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private GameObject waterSourceObject;
        [SerializeField] private ProgressBar waterLevelUI;
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.1f;

        [SerializeField] private ProgressBar staminaLevelUI;
        [SerializeField] private float staminaLevel = 1f;
        [SerializeField] private float staminaPerUse = 0.1f;
        [SerializeField] private float staminaRegenPerSecond = 0.05f;
        [SerializeField] private TMP_Text lowStaminaText;

        //MovementController moveController;
        AnimatedController animatedController;

        [SerializeField] private List<FarmTile> farmTiles; // List of all farm tiles

    [Header("Reward Settings")]
    [SerializeField] private int fundsReward = 50;
    [SerializeField] private int harvestReward = 1;
    [SerializeField] private int seedRewardAmount = 3;
    [SerializeField] private SeedData rewardSeed;
    [SerializeField] private RewardFeedUI rewardFeedUI;
    [SerializeField] private ParticleSystem rewardFireworks;
    [SerializeField] private AudioSource rewardAudioSource;
    [SerializeField] private AudioClip rewardClip;

        private bool rewardGiven = false;

        [SerializeField] private TMP_Text congratulationsText; // TMP Text to display the congratulations message

        private float congratulationsDuration = 3f; // Duration to show the congratulations message (in seconds)
        [SerializeField] private TMP_Text waterRefillText;
        [SerializeField] private TMP_Text needSeedText;
        private WaitForSeconds messageDelay;
        private AudioClip generatedRewardClip;

        private void Start()
        {
            //moveController = GetComponent<MovementController>();
            
            // TODO: Consider Debug.Assert vs RequireComponent(typeof(...))
             //Debug.Log(animatedController);
            waterLevel = Mathf.Clamp01(waterLevel);
            staminaLevel = Mathf.Clamp01(staminaLevel);

            if (gardenHoe != null) gardenHoe.SetActive(false);
            if (waterCan != null) waterCan.SetActive(false);
            if (animatedController == null)
            {
                animatedController = GetComponent<AnimatedController>();
            }

            Debug.Assert(tileSelector, "Farmer requires a TileSelector");
            Debug.Assert(staminaLevelUI, "Farmer requires a staminaLevelUI");
            if (staminaLevelUI != null)
            {
                staminaLevelUI.setText("Stamina");
                staminaLevelUI.Fill = staminaLevel;
            }

            if (animatedController == null)
            {
                Debug.LogError("Farmer requires an AnimatedController! Animation will not play.");
            }

            Debug.Assert(waterLevelUI, "Farmer requires an waterLevel");
            if (waterLevelUI != null)
            {
                waterLevelUI.setText("Water Level");
                waterLevelUI.Fill = waterLevel;
            }

            messageDelay = new WaitForSeconds(congratulationsDuration);
            rewardFeedUI = EnsureRewardFeed();
            rewardFireworks = EnsureRewardFireworks();
            rewardAudioSource = EnsureRewardAudioSource();

            if (lowStaminaText != null) lowStaminaText.gameObject.SetActive(false);
            if (waterRefillText != null) waterRefillText.gameObject.SetActive(false);
            if (needSeedText != null) needSeedText.gameObject.SetActive(false);
            if (congratulationsText != null) congratulationsText.gameObject.SetActive(false);

            // Collect all tiles in the scene
            farmTiles = new List<FarmTile>(Object.FindObjectsByType<FarmTile>(FindObjectsSortMode.None));
        }
        
        public void SetTool(string tool)
        {
            if (gardenHoe != null) gardenHoe.SetActive(false);
            if (waterCan != null) waterCan.SetActive(false);
            switch (tool)
            {
                case "GardenHoe": gardenHoe.SetActive(true); break;
                case "WaterCan": waterCan.SetActive(true); break;
            }
        }
       
        public void TryTileInteraction()
        {
            if (tileSelector == null)
            {
                return;
            }

            FarmTile tile = tileSelector.GetSelectedTile();
            if (tile == null) return;

            // updates the condition, play the anim after
            if (tile.HasMaturePlant())
            {
                tile.Interact(); // Harvest happens inside
                return;          // STOP so water is not reduced
            }

            switch (tile.GetCondition)
            {
                case FarmTile.Condition.Grass:
                    if (!TryUseStamina(staminaPerUse))
                    {
                        DisplayLowStamina();
                        return;
                    }
                    animatedController.SetTrigger("Till");
                    tile.Interact();
                    break;
                case FarmTile.Condition.Tilled:
                    if (!TryUseStamina(staminaPerUse))
                    {
                        DisplayLowStamina();
                        return;
                    }
                    if (!TryUseWater())
                    {
                        DisplayWaterLow();
                        return;
                    }

                    animatedController?.SetTrigger("Water");
                    tile.Interact();
                    break;
                case FarmTile.Condition.Watered:
                    if (!TryUseStamina(staminaPerUse))
                    {
                        DisplayLowStamina();
                        return;
                    }
                    tile.Interact();
                    break;
                case FarmTile.Condition.Planted:
                    if (!TryUseStamina(staminaPerUse))
                    {
                        DisplayLowStamina();
                        return;
                    }
                    if (!TryUseWater())
                    {
                        DisplayWaterLow();
                        return;
                    }

                    animatedController?.SetTrigger("Water");
                    tile.Interact();

                    if (GameManager.Instance != null && GameManager.Instance.GetTotalSeeds() <= 0)
                    {
                        DisplayLowSeed();
                    }
                    break;
                default:
                    break;
            }

            // Check if all tiles are watered
            CheckWinCondition();
        }

        public void DisplayLowSeed()
        {
            if (needSeedText == null) return;
            needSeedText.SetText("No seeds left. Visit the store to restock.");
            needSeedText.gameObject.SetActive(true);
            StartCoroutine(HideSeedMessage());
        }

        public void DisplaySeasonLocked(SeedData seed, SeasonManager.Season currentSeason)
        {
            if (needSeedText == null || seed == null)
            {
                return;
            }

            needSeedText.text = $"{seed.seedName} grows in {seed.GetSeasonSummary()}, not {currentSeason}.";
            needSeedText.gameObject.SetActive(true);
            StartCoroutine(HideSeedMessage());
        }

        private IEnumerator HideSeedMessage()
        {
            yield return messageDelay;
            if (needSeedText != null)
            {
                needSeedText.gameObject.SetActive(false);
            }
        }

        public void DisplayLowStamina()
        {
            if (lowStaminaText == null) return;
            lowStaminaText.SetText("Too tired to perform this action!");
            lowStaminaText.gameObject.SetActive(true);
            StartCoroutine(HideStaminaMessage());
        }

        private IEnumerator HideStaminaMessage()
        {
            yield return messageDelay;
            if (lowStaminaText != null)
            {
                lowStaminaText.gameObject.SetActive(false);
            }
        }

        public void DisplayWaterLow()
        {
            if (waterRefillText == null) return;
            waterRefillText.SetText("Water low.");
            waterRefillText.gameObject.SetActive(true);
            StartCoroutine(HideWaterMessage());
        }
        
        public void DisplayWaterRefilled()
        {
            if (waterRefillText == null) return;
            waterRefillText.SetText("Water refilled.");
            waterRefillText.gameObject.SetActive(true);
            StartCoroutine(HideWaterMessage());
        }

        private IEnumerator HideWaterMessage()
        {
            // Wait for the specified duration
            yield return messageDelay;

            // Hide the message after the wait
            if (waterRefillText != null)
            {
                waterRefillText.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject == waterSourceObject))
            {
                if (waterLevel < 1f)
                {
                    waterLevel = 1f;
                    if (waterLevelUI != null)
                    {
                        waterLevelUI.Fill = waterLevel;
                    }

                    DisplayWaterRefilled();  
                }
            }
        }

        private void CheckWinCondition()
        {
            if (rewardGiven) return;

            if (farmTiles == null || farmTiles.Count == 0)
            {
                farmTiles = new List<FarmTile>(Object.FindObjectsByType<FarmTile>(FindObjectsSortMode.None));
            }

            bool allPlanted = true;
            foreach (var tile in farmTiles)
            {
                if (!tile.IsEffectivelyPlanted())
                {
                    allPlanted = false;
                    break;
                }
            }

            if (allPlanted)
            {
                DisplayWinMessage();
                AwardFunds();
                /*
                if (GameManager.Instance.seeds > 0)
                {
                    GameManager.Instance.seeds--;
                    GameManager.Instance.AddSeeds(0); // refresh UI
                    Debug.Log("Seeds decreased after planting all tiles.");
                }
                */
                
            }
        }

        public void NotifyTilePlanted()
        {
            CheckWinCondition();
        }

        private void DisplayWinMessage()
        {
            if (congratulationsText == null) return;
            congratulationsText.SetText("Congratulations! Every tile has been planted.");
            congratulationsText.gameObject.SetActive(true);
            StartCoroutine(HideCongratulationsMessage());
        }

        private IEnumerator HideCongratulationsMessage()
        {
            yield return messageDelay;

            if (congratulationsText != null)
            {
                congratulationsText.gameObject.SetActive(false);
            }
        }
        
        private void AwardFunds()
        {
            if (!rewardGiven)
            {
                if (GameManager.Instance == null)
                {
                    Debug.LogWarning("GameManager.Instance is null! Cannot award funds yet.");
                    return;
                }

                rewardGiven = true;
                List<GameplayReward> rewards = new List<GameplayReward>();

                if (fundsReward > 0)
                {
                    GameManager.Instance.AddFunds(fundsReward);
                    rewards.Add(new GameplayReward(GameplayRewardType.Funds, fundsReward, "Funds"));
                }

                if (harvestReward > 0)
                {
                    GameManager.Instance.AddHarvest(harvestReward);
                    rewards.Add(new GameplayReward(GameplayRewardType.Harvest, harvestReward, "Harvest Bonus"));
                }

                SeedData grantedSeed = rewardSeed != null ? rewardSeed : GameManager.Instance.GetDefaultSeed();
                if (grantedSeed != null && seedRewardAmount > 0)
                {
                    GameManager.Instance.AddSeedInventory(grantedSeed, seedRewardAmount);
                    rewards.Add(new GameplayReward(GameplayRewardType.Seeds, seedRewardAmount, grantedSeed.seedName + " Seeds"));
                }

                rewardFeedUI?.ShowRewards(rewards);
                PlayRewardCelebration();
            }
        }

        public void CheckTilesResetToGrass()
        {
            bool allGrass = true;
            foreach (var tile in farmTiles)
            {
                if (tile.GetCondition != FarmTile.Condition.Grass)
                {
                    allGrass = false;
                    break;
                }
            }

            if (allGrass)
            {
                rewardGiven = false; // Reset the reward condition once all tiles are grass again
            }
        }

        private bool TryUseStamina(float amount)
        {
            if (staminaLevel < amount) return false;

            staminaLevel = Mathf.Clamp01(staminaLevel - amount);
            if (staminaLevelUI != null)
            {
                staminaLevelUI.Fill = staminaLevel;
            }

            return true;
        }

        private bool TryUseWater()
        {
            if (waterLevel < waterPerUse)
            {
                return false;
            }

            waterLevel = Mathf.Clamp01(waterLevel - waterPerUse);
            if (waterLevelUI != null)
            {
                waterLevelUI.Fill = waterLevel;
            }

            return true;
        }

        private void RegenerateStamina()
        {
            staminaLevel = Mathf.Clamp01(staminaLevel + staminaRegenPerSecond * Time.deltaTime);
            if (staminaLevelUI != null)
            {
                staminaLevelUI.Fill = staminaLevel;
            }
        }

        private void Update()
        {
            RegenerateStamina();
        }

        private RewardFeedUI EnsureRewardFeed()
        {
            if (rewardFeedUI != null)
            {
                return rewardFeedUI;
            }

            rewardFeedUI = FindFirstObjectByType<RewardFeedUI>();
            if (rewardFeedUI != null)
            {
                return rewardFeedUI;
            }

            GameObject uiObject = new GameObject("RewardFeedUI");
            rewardFeedUI = uiObject.AddComponent<RewardFeedUI>();
            return rewardFeedUI;
        }

        private ParticleSystem EnsureRewardFireworks()
        {
            if (rewardFireworks != null)
            {
                return rewardFireworks;
            }

            GameObject fireworksObject = new GameObject("RewardFireworks");
            fireworksObject.transform.SetParent(transform, false);
            fireworksObject.transform.localPosition = Vector3.up * 2f;

            ParticleSystem particles = fireworksObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.duration = 1.5f;
            main.loop = false;
            main.startLifetime = 1.25f;
            main.startSpeed = 4.5f;
            main.startSize = 0.15f;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, new Color(1f, 0.4f, 0.1f));
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = particles.emission;
            emission.enabled = false;
            emission.SetBursts(new[]
            {
                new ParticleSystem.Burst(0f, 35),
                new ParticleSystem.Burst(0.35f, 35),
                new ParticleSystem.Burst(0.7f, 35)
            });

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            var colorOverLifetime = particles.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.yellow, 0f),
                    new GradientColorKey(new Color(1f, 0.35f, 0.1f), 0.5f),
                    new GradientColorKey(Color.cyan, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            var sizeOverLifetime = particles.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1.2f));

            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return particles;
        }

        private AudioSource EnsureRewardAudioSource()
        {
            if (rewardAudioSource != null)
            {
                return rewardAudioSource;
            }

            rewardAudioSource = GetComponent<AudioSource>();
            if (rewardAudioSource == null)
            {
                rewardAudioSource = gameObject.AddComponent<AudioSource>();
            }

            rewardAudioSource.playOnAwake = false;
            rewardAudioSource.spatialBlend = 0f;
            return rewardAudioSource;
        }

        private void PlayRewardCelebration()
        {
            if (rewardFireworks != null)
            {
                rewardFireworks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                rewardFireworks.Play(true);
            }

            if (rewardAudioSource == null)
            {
                return;
            }

            if (rewardClip == null)
            {
                generatedRewardClip ??= CreateGeneratedRewardClip();
                rewardClip = generatedRewardClip;
            }

            if (rewardClip != null)
            {
                rewardAudioSource.PlayOneShot(rewardClip);
            }
        }

        private AudioClip CreateGeneratedRewardClip()
        {
            const int sampleRate = 44100;
            const float duration = 0.45f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)sampleRate;
                float frequency = time < 0.18f ? 880f : 1320f;
                float envelope = Mathf.Clamp01(1f - (time / duration));
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * envelope * 0.25f;
            }

            AudioClip clip = AudioClip.Create("RewardCelebration", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}

