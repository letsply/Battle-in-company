using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] UnityEvent[] events;
    private string currentInstruction;
    private int currentEvent = 0;

    private bool isTyping;
    private bool isInBuilding;
    private GameObject player { get => GameObject.FindGameObjectWithTag("Player"); }
    private Rigidbody playerRB { get => GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>(); }

    [SerializeField] private GameObject tutorialInfoBox;
    private TextMeshProUGUI infoText;

    [SerializeField] private GameObject tutorialItem;
    [SerializeField] private Transform itemSpawnPos;

    private void Start()
    {
        infoText = tutorialInfoBox.GetComponentInChildren<TextMeshProUGUI>();

        currentInstruction = "WASD";
        events[0].Invoke();

    }

    private void FixedUpdate()
    {
        CheckIfInstructionIsFullfilled();
    }

    private void CheckIfInstructionIsFullfilled()
    {
        // Let the player progress after time has passed and they did the action
        if (isTyping == false)
        {
            switch (currentInstruction)
            {
                case "WASD":
                    if (playerRB.linearVelocity.x >= 0.1f || playerRB.linearVelocity.z >= 0.1f)
                    {
                        OnInstructionFinnished("Jump");
                    }
                    break;

                case "Jump":
                    if (playerRB.linearVelocity.y >= 0.5f)
                    {
                        OnInstructionFinnished("Int");
                    }
                    break;

                case "Int":
                    if (isInBuilding)
                    {
                        OnInstructionFinnished("Take");
                        Instantiate(tutorialItem,itemSpawnPos.transform.position,Quaternion.identity);
                    }
                    break;

                case "Take":
                    if (player.GetComponentInChildren<Item>())
                    {
                        OnInstructionFinnished("Hit&Throw");
                    }
                    break;

                case "Hit&Throw":
                    if (player.GetComponentInChildren<Item>())
                    {
                        OnInstructionFinnished("End");
                    }
                    break;
            }
          
        }
    }

    // next instruction that happens in tutorial
    private void OnInstructionFinnished(string nextInstruction)
    {
        currentEvent += 1;
        events[currentEvent].Invoke();
        currentInstruction = nextInstruction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isInBuilding = true;
        }
    }

    public void SwitchText(string text)
    {
        StartCoroutine(ISwitchText(text));
    }

    public IEnumerator ISwitchText(string text)
    {
        isTyping = true;

        infoText.text = "";
        tutorialInfoBox.SetActive(true);
        string textOnDisp = "";
        int spacing = 40;
        for(int i = 0; i < text.Length; i++)
        {
            textOnDisp += text.ToCharArray()[i];
            if(i > spacing) { 
                textOnDisp += "\n";
                spacing += 40;
            }
            yield return new WaitForSeconds(0.025f);
            infoText.text = textOnDisp;
        }

        yield return new WaitForSeconds(5f);

        isTyping = false;

        tutorialInfoBox.SetActive(false);
    }

}
