using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelListItemUI : MonoBehaviour
{
    public string Title;
    public string Score;
    public string Time;

    public bool Completed;

    public LevelDefinition AssociatedLevel;

    [SerializeField]
    private TextMeshProUGUI titleTextField;
    [SerializeField]
    private TextMeshProUGUI scoreTextField;
    [SerializeField]
    private TextMeshProUGUI timeTextField;
    [SerializeField]
    private Transform completedTag;

    private void Start()
    {
        titleTextField.text = Title;
        scoreTextField.text = Score;
        timeTextField.text = Time;

        completedTag.gameObject.SetActive(Completed);
    }
}
