using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class PlanetSelectUI : MonoBehaviour
{
    public string PlanetName;
    public Sprite PlanetSprite;
    public Color PlanetAtmosphereColor;
    public bool PlanetDiscovered;
    public float PlanetScale = 1.0f;

    public LevelCategory LevelCategory;

    [SerializeField]
    private Image PlanetRenderer;
    [SerializeField]
    private Image PlanetAtmosphereRenderer;
    [SerializeField]
    private TextMeshProUGUI PlanetTitleText;
    [SerializeField]
    private Transform PlanetHighlightRing;
    [SerializeField]
    private Button PlanetButton;

    [SerializeField]
    private LevelSelectUI levelSelectUI;

    private Vector3 startingPosition;

    private PlanetSelectUI[] otherPlanets;

    public void Awake()
    {
        PlanetDiscovered = LevelUnlocks.GetUnlockStatus(new LevelLocation { Category = LevelCategory, Index = 0 });

        UpdateValues();
        startingPosition = transform.localPosition;

        otherPlanets = Transform.FindObjectsOfType<PlanetSelectUI>().Where(x=> x != this).ToArray();
    }

    public void UpdateValues() 
    {
        PlanetRenderer.sprite = PlanetSprite;
        PlanetAtmosphereRenderer.color = PlanetDiscovered ? PlanetAtmosphereColor : Color.clear;
        PlanetTitleText.text = PlanetDiscovered ? PlanetName : "?";
        PlanetButton.interactable = PlanetDiscovered;

        transform.localScale = Vector3.one * PlanetScale;
    }

    public void OnClick()
    {
        if (!PlanetDiscovered) return;

        if (levelSelectUI.gameObject.activeSelf)
        {
            levelSelectUI.Close();

            transform.localPosition = startingPosition;

            _toggleOtherPlanets(true);

            PlanetHighlightRing.gameObject.SetActive(false);
        }
        else
        {
            levelSelectUI.Open(LevelCategory);

            transform.position = levelSelectUI.PlanetPositionReference.position;

            _toggleOtherPlanets(false);

            PlanetHighlightRing.gameObject.SetActive(true);
        }
    }

    private void _toggleOtherPlanets(bool active)
    {
        foreach (var ps in otherPlanets)
        {
            ps.gameObject.SetActive(active);
        }
    }
}
