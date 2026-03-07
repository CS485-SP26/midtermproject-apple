using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public enum GameplayRewardType
    {
        Funds,
        Harvest,
        Seeds,
        Bonus
    }

    public readonly struct GameplayReward
    {
        public readonly GameplayRewardType Type;
        public readonly int Amount;
        public readonly string Label;

        public GameplayReward(GameplayRewardType type, int amount, string label)
        {
            Type = type;
            Amount = amount;
            Label = label;
        }
    }

    public class RewardFeedUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private float visibleSeconds = 4f;

        private readonly StringBuilder builder = new StringBuilder(128);
        private Coroutine hideRoutine;

        private void Awake()
        {
            EnsureText();
            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(false);
            }
        }

        public void ShowRewards(IReadOnlyList<GameplayReward> rewards)
        {
            if (rewards == null || rewards.Count == 0)
            {
                return;
            }

            EnsureText();
            if (rewardText == null)
            {
                return;
            }

            builder.Clear();
            builder.Append("Rewards Unlocked");

            for (int i = 0; i < rewards.Count; i++)
            {
                GameplayReward reward = rewards[i];
                builder.Append('\n')
                    .Append("+ ")
                    .Append(reward.Label)
                    .Append(": ")
                    .Append(reward.Amount);
            }

            rewardText.text = builder.ToString();
            rewardText.gameObject.SetActive(true);

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(HideRoutine());
        }

        private System.Collections.IEnumerator HideRoutine()
        {
            yield return new WaitForSecondsRealtime(visibleSeconds);

            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(false);
            }
        }

        private void EnsureText()
        {
            if (rewardText != null)
            {
                return;
            }

            rewardText = GetComponentInChildren<TMP_Text>(true);
            if (rewardText != null)
            {
                return;
            }

            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            GameObject panelObject = new GameObject("RewardFeedPanel", typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(canvas.transform, false);

            RectTransform panelTransform = panelObject.GetComponent<RectTransform>();
            panelTransform.anchorMin = new Vector2(1f, 1f);
            panelTransform.anchorMax = new Vector2(1f, 1f);
            panelTransform.pivot = new Vector2(1f, 1f);
            panelTransform.anchoredPosition = new Vector2(-32f, -32f);
            panelTransform.sizeDelta = new Vector2(320f, 140f);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.13f, 0.08f, 0.85f);

            GameObject textObject = new GameObject("RewardFeedText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(panelObject.transform, false);

            RectTransform textTransform = textObject.GetComponent<RectTransform>();
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = new Vector2(14f, 14f);
            textTransform.offsetMax = new Vector2(-14f, -14f);

            rewardText = textObject.GetComponent<TextMeshProUGUI>();
            rewardText.fontSize = 22f;
            rewardText.color = new Color(1f, 0.96f, 0.78f);
            rewardText.alignment = TextAlignmentOptions.TopLeft;
        }
    }
}