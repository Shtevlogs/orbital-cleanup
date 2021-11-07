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
    public TextMeshProUGUI LevelUnlockText;

    public void Open(LevelCategory category)
    {
        for(var i = 0; i < LevelListContent.childCount; i++)
        {
            Destroy(LevelListContent.GetChild(i).gameObject);
        }

        gameObject.SetActive(true);

        PlanetTitle.text = category.ToString();

        var progress = LevelUnlocks.LevelUnlockCount(category);
        
        if(progress == null || progress[0] >= progress[1])
        {
            LevelUnlockText.text = "All Unlocked!";
        }
        else
        {
            LevelUnlockText.text = "Next Unlock: " + progress[0] + "/" + progress[1];
        }

        var levelList = LevelUnlocks.GetLevelList(category);

        for(var i = 0; i < levelList.Count; i++)
        {
            var levelDef = levelList[i];
            var newListItem = Instantiate(ListItemPrefab, LevelListContent);
            var levelLocation = new LevelLocation { Category = category, Index = i };
            var levelData = SaveData.Load(levelLocation);

            //a little failsafe so the initial levels are always unlocked
            var unlocked = LevelUnlocks.GetUnlockStatus(levelLocation) || (i == 0 && category == LevelCategory.Earth);

            newListItem.Title = unlocked ? levelDef.Name : "?";
            newListItem.Score = levelData.HighScore == -1 ? "None" : levelData.HighScore.ToString();
            newListItem.Time = levelData.BestTime == -1 ? "None" : levelData.BestTime.ToString();
            newListItem.Completed = levelData.Completed;

            var newListItemButton = newListItem.GetComponent<Button>();
            newListItemButton.onClick.AddListener(()=> {
                GameSceneLoader.Instance.LoadLevelScene(levelLocation);
            });
            newListItemButton.interactable = unlocked;
        }

        LevelListContent.gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
