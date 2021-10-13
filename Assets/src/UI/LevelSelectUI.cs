using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    public LevelListItemUI ListItemPrefab;

    public Transform PlanetPositionReference;
    public Transform LevelListContent;
    public TextMeshProUGUI PlanetTitle;

    [SerializeField]
    private List<LevelDefinition> earthLevels = new List<LevelDefinition>();
    [SerializeField]
    private List<LevelDefinition> marsLevels = new List<LevelDefinition>();
    [SerializeField]
    private List<LevelDefinition> gloopLevels = new List<LevelDefinition>();

    public enum LevelCategory
    {
        Earth,
        Mars,
        Gloop
    }

    public void Open(LevelCategory category)
    {
        for(var i = 0; i < LevelListContent.childCount; i++)
        {
            Destroy(LevelListContent.GetChild(i).gameObject);
        }

        gameObject.SetActive(true);

        var levelList = category == LevelCategory.Earth ? earthLevels : (category == LevelCategory.Mars ? marsLevels : gloopLevels);

        foreach(var levelDef in levelList)
        {
            var newListItem = Instantiate(ListItemPrefab, LevelListContent);
            newListItem.Title = levelDef.Name;
            newListItem.Score = "None";
            newListItem.Time = "None";

            var newListItemButton = newListItem.GetComponent<Button>();
            newListItemButton.onClick.AddListener(()=> {
                GameSceneLoader.Instance.LoadLevelScene(levelDef);
            });
        }

        LevelListContent.gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
