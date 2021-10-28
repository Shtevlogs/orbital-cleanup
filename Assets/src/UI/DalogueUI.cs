using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DalogueUI : MonoBehaviour
{
    private static DalogueUI Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public delegate void DialogueFinishDelegate ();
    public static DialogueFinishDelegate OnDialogueFinish;

    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI SpeakerText;
    public Image TalkerImage;

    public AudioReference TextSound;

    public float LettersPerSecond = 10f;
    public int LettersPerLipflap = 3;

    private List<DialoguePhrase> currentDialogue;
    private Sprite lipFlapSprite1;
    private Sprite lipFlapSprite2;

    private bool coroutineActive = false;
    private int lipFlapCounter = 0;

    public void OnClick()
    {
        _showNextDialogue();
    }

    private void _showNextDialogue()
    {
        if(!coroutineActive)
        {
            if(currentDialogue.Count > 0)
            {
                DialogueText.text = string.Empty;
                coroutineActive = true;
                StartCoroutine(_speakAndSpell());
            }
            else
            {
                OnDialogueFinish?.Invoke();
                gameObject.SetActive(false);
            }
        }
        else
        {
            coroutineActive = false;
            DialogueText.text += currentDialogue[0].Dialogue;
            currentDialogue.RemoveAt(0);
            TalkerImage.sprite = TalkerImage.sprite == lipFlapSprite1 ? lipFlapSprite2 : lipFlapSprite1;
            TextSound.Play();
        }
    }

    private IEnumerator _speakAndSpell()
    {
        while(coroutineActive)
        {
            lipFlapCounter++;

            if(lipFlapCounter >= LettersPerLipflap)
            {
                lipFlapCounter = 0;
                //flip lipflap sprite
                TalkerImage.sprite = TalkerImage.sprite == lipFlapSprite1 ? lipFlapSprite2 : lipFlapSprite1;
            }

            //play letter sound
            TextSound.Play();

            var currentPhrase = currentDialogue[0];

            DialogueText.text += currentPhrase.Dialogue[0];

            currentPhrase.Dialogue = currentPhrase.Dialogue.Substring(1);

            if (string.IsNullOrEmpty(currentPhrase.Dialogue))
            {
                coroutineActive = false;
                currentDialogue.RemoveAt(0);
            }
            else
            {
                yield return new WaitForSecondsRealtime(1f / LettersPerSecond);
            }
        }
    }

    public static void StartDialogue(DialoguePhrase[] phrases, Sprite lipflapsprite1, Sprite lipflapsprite2, string speakerName)
    {
        var dialogueCopy = new List<DialoguePhrase>();

        foreach(var phrase in phrases)
        {
            dialogueCopy.Add(new DialoguePhrase { Dialogue = phrase.Dialogue });
        }

        Instance.currentDialogue = dialogueCopy;
        Instance.lipFlapSprite1 = lipflapsprite1;
        Instance.lipFlapSprite2 = lipflapsprite2;
        Instance.TalkerImage.sprite = lipflapsprite1;
        Instance.SpeakerText.text = speakerName;

        Instance.gameObject.SetActive(true);
        Instance._showNextDialogue();
    }
}
