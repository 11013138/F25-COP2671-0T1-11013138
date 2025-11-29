using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour
{

    [SerializeField] private Button hoeButton;
    [SerializeField] private Button waterButton;
    [SerializeField] private Button seedButton;
    [SerializeField] private Button harvestButton;

    [SerializeField] private FarmingController farmingController;

    private Button selectedButton;
    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.7f, 0.9f, 1f, 1f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (farmingController == null)
            farmingController = Object.FindFirstObjectByType<FarmingController>();

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (hoeButton != null)
            hoeButton.onClick.AddListener(OnHoeClicked);

        if (waterButton != null)
            waterButton.onClick.AddListener(OnWaterClicked);

        if (seedButton != null)
            seedButton.onClick.AddListener(OnSeedClicked);

        if (harvestButton != null)
            harvestButton.onClick.AddListener(OnHarvestClicked);
    }

    private void OnHoeClicked()
    {
        SelectButton(hoeButton);
        if (farmingController != null)
            farmingController.OnHoeButtonClicked();
    }

    private void OnWaterClicked()
    {
        SelectButton(waterButton);
        if (farmingController != null)
            farmingController.OnWaterButtonClicked();
    }

    private void OnSeedClicked()
    {
        SelectButton(seedButton);
        if (farmingController != null)
            farmingController.OnSeedButtonClicked();
    }

    private void OnHarvestClicked()
    {
        SelectButton(harvestButton);
        if (farmingController != null)
            farmingController.OnHarvestButtonClicked();
    }

    private void SelectButton(Button button)
    {

        // deselect previous button
        if (selectedButton != null)
        {
            var colors = selectedButton.colors;
            colors.normalColor = normalColor;
            selectedButton.colors = colors;
        }

        // select new button
        selectedButton = button;
        if (selectedButton != null)
        {
            var colors = selectedButton.colors;
            colors.normalColor = selectedColor;
            selectedButton.colors = colors;
        }
    }
}
