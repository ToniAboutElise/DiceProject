using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    //Dice prefabs
    public GameObject diceD3;
    public GameObject diceD4;
    public GameObject diceD6;
    public GameObject diceD8;
    public GameObject diceD10;
    public GameObject diceD12;
    public GameObject diceD20;

    //Dice position spawners
    public Transform[] diceSpawnTransform;

    //Spawned dice list
    public List<DraggableDice> diceList;

    //Final result UI elements
    public GameObject finalResultDisplayUI;
    public Text finalResultText;
    public Text resultText;

    //Dice limit variables
    public int diceLimit;
    public Animation diceLimitMessage;

    //Boolean to manage if a roll is allowed
    public bool canRoll = true;

    //Sound effect that will play if too many rerolls are needed
    public AudioSource failAudioSource;

    //Clear button to delete all dice from the scene and the diceList
    public Button clearButton;
    
    //Button list used to set buttons as non interactable during specific moments to avoid bugs
    public List<Button> diceButtonList;

    //Fail sentences variables
    public List<GameObject> failSentences;
    int currentFail = 0;

    //float used to wait for rerolls until a fail happens
    protected float resultWaitExpiration = 0;

    //Current color, set to white by default
    public string currentDiceColor = "white";

    //Hides specified UI elements and sets dice limit depending on the platform
    private void Start()
    {
        finalResultDisplayUI.SetActive(false);
        SetDiceLimit();
    }

    //Die Spawner
    public void SpawnDice(string dice)
    {
        if (HasDiceLimitBeenReached() == true)
        {
            HasDiceLimitBeenReached();
            return;
        }

        GameObject diceInstance = null;
        switch (dice)
        {
            case "D3":
                diceInstance = Instantiate(diceD3);
                break;
            case "D4":
                diceInstance = Instantiate(diceD4);
                break;
            case "D6":
                diceInstance = Instantiate(diceD6);
                break;
            case "D8":
                diceInstance = Instantiate(diceD8);
                break;
            case "D10":
                diceInstance = Instantiate(diceD10);
                break;
            case "D12":
                diceInstance = Instantiate(diceD12);
                break;
            case "D20":
                diceInstance = Instantiate(diceD20);
                break;
        }

        int rand = Random.Range(0, diceSpawnTransform.Length);

        diceInstance.transform.position = diceSpawnTransform[rand].position;
        diceInstance.transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
        diceList.Add(diceInstance.GetComponent<DraggableDice>());
        diceInstance.GetComponent<DraggableDice>().diceManager = this;
        ChangeDiceColor(currentDiceColor);
    }

    //Change color method
    public void ChangeDiceColor(string color)
    {
        switch (color)
        {
            case "white":
                currentDiceColor = "white";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.white;
                }
                break;
            case "red":
                currentDiceColor = "red";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.red;
                }
                break;
            case "black":
                currentDiceColor = "black";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.black;
                }
                break;
            case "alien":
                currentDiceColor = "alien";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.alien;
                }
                break;
            case "blue":
                currentDiceColor = "blue";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.blue;
                }
                break;
        }
    }

    //Method to be used on UI Roll Button
    public void RollButton()
    {
        if(canRoll == true && diceList.Count != 0)
        {
            clearButton.interactable = false;

            foreach(Button b in diceButtonList)
            {
                b.interactable = false;
            }

            canRoll = false;
            foreach(DraggableDice d in diceList)
            {
                d.SingleRollCall();
            }
            resultWaitExpiration = 0;
            StartCoroutine(WaitForResult());
        }
    }

    //Checks if dice result is valid. If not, a reroll takes place
    public IEnumerator WaitForResult()
    {
        bool allResultsReady = true;

        yield return new WaitForSeconds(2.6f);

        foreach (DraggableDice dd in diceList)
        {
            int result;
            System.Int32.TryParse(dd.result, out result);

            if (dd.result == null || (result <= 0 || result > 20))
            {
                allResultsReady = false;
                if((result <= 0 || result > 20))
                {
                    dd.SingleRollCall();
                }
            }
        }

        resultWaitExpiration += 0.15f;

        if(resultWaitExpiration >= 1)
        {
            ClearDices();
            StopCoroutine(WaitForResult());
        }
        else
        {
            if (allResultsReady == true)
            {
                DisplayResult();
                StopCoroutine(WaitForResult());
            }
            else
            {
                StartCoroutine(WaitForResult());
            }
        }
    }


    //Displays final result if all die have a valid result int
    public void DisplayResult()
    {
        resultText.text = "";
        int finalResult = 0;
        foreach(DraggableDice dd in diceList)
        {
            if(dd.result == null)
            {
                dd.SingleRollCall();
                StartCoroutine(WaitForResult());
                return;
            }
            resultText.text += dd.result + "(" + dd.dieName + ") ";
            if(dd.result != null)
            {
                int result;
                System.Int32.TryParse(dd.result, out result);
                Debug.Log(result);
                finalResult += result;
            }
            else
            {
                ClearDices();
            }
        }
        finalResultText.text = finalResult.ToString();
        finalResultDisplayUI.SetActive(true);
        canRoll = true;
    }

    //Clears dice
    public void ClearDices()
    {
            if(resultWaitExpiration >= 1)
            {
                StartCoroutine(FailSentenceSequence());
                failAudioSource.Play();
                resultWaitExpiration = 0;
            }

            DraggableDice[] dices = FindObjectsOfType<DraggableDice>();

            foreach(DraggableDice d in dices)
            {
                Destroy(d.gameObject);
            }
            diceList.Clear();
            StopCoroutine(WaitForResult());
            canRoll = true;
            clearButton.interactable = true;

            foreach (Button b in diceButtonList)
            {
                b.interactable = true;
            }
    }

    //Makes fail sentence appear if too many rerolls take place
    public IEnumerator FailSentenceSequence()
    {
        failSentences[currentFail].SetActive(true);
        yield return new WaitForSeconds(5);
        failSentences[currentFail].SetActive(false);
        if(currentFail < failSentences.Count-1) 
        { 
            currentFail++;
        }
    }


    //Load URL method to make website appear
    public void LoadURL(string url)
    {
        Application.OpenURL(url);
    }

    //Sets dice limit depending on platform
    protected void SetDiceLimit()
    {
#if UNITY_STANDALONE_WIN || UNITY_WEBGL
        diceLimit = 60;
#elif UNITY_ANDROID
        diceLimit = 30;
#endif
    }

    //Checks if max amount of dice have already been spawned
    protected bool HasDiceLimitBeenReached()
    {
        if(diceList.Count == diceLimit)
        {
            if(diceLimitMessage.isPlaying == false)
            { 
                diceLimitMessage.Play();
            }
            return true;
        }
        else
        {
            Debug.Log(diceList.Count + " | " + diceLimit);
            return false;
        }
    }

    //Quits app
    public void ExitApp()
    {
        Application.Quit();
    }

    //Update method continuously checks accelerometer for shake roll
    private void Update()
    {
        if (Input.acceleration.x > 3f)
        {
            RollButton();
        }
    }
}