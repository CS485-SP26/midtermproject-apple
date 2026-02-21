using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI fillText;

    public float Fill {set{fillImage.fillAmount = value;}}

    public void setText(string text)
    {
        fillText.text = text;
    }
}
