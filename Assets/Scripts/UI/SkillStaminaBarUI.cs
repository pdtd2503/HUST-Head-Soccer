using UnityEngine;
using UnityEngine.UI;

public class SkillStaminaBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSkillController playerSkillController;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (playerSkillController == null || fillImage == null)
        {
            return;
        }

        fillImage.fillAmount = playerSkillController.GetSkillChargeRatio();
    }
}