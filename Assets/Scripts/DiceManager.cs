using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public GameObject diceD3;
    public GameObject diceD4;
    public GameObject diceD6;
    public GameObject diceD8;
    public GameObject diceD10;
    public GameObject diceD12;
    public GameObject diceD20;
    public Transform[] diceSpawnTransform;
    public List<DraggableDice> diceList;

    public GameObject finalResultDisplayUI;
    public Text finalResultText;
    public Text resultText;

    public int diceLimit;
    public Animation diceLimitMessage;

    public bool canRoll = true;

    public AudioSource failAudioSource;

    public Button clearButton;
    public List<Button> diceButtonList;

    public List<GameObject> failSentences;
    int currentFail = 0;

    protected float resultWaitExpiration = 0;

    public string currentDiceColor = "white";

    private void Start()
    {
        finalResultDisplayUI.SetActive(false);
        SetDiceLimit();
    }

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

    public void ChangeDiceColor(string color)
    {
        bool textureHasBeenSet = false;
        switch (color)
        {
            case "white":
                currentDiceColor = "white";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.white;
                    if(textureHasBeenSet == false)
                    {
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "red":
                currentDiceColor = "red";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.red;
                    if (textureHasBeenSet == false)
                    {
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "black":
                currentDiceColor = "black";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.black;
                    if (textureHasBeenSet == false)
                    {
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "alien":
                currentDiceColor = "alien";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.alien;
                    if (textureHasBeenSet == false)
                    {
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "blue":
                currentDiceColor = "blue";
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.blue;
                    if (textureHasBeenSet == false)
                    {
                        textureHasBeenSet = true;
                    }
                }
                break;
        }
    }

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

    public void LoadURL(string url)
    {
        Application.OpenURL(url);
    }

    protected void SetDiceLimit()
    {
#if UNITY_STANDALONE_WIN || UNITY_WEBGL
        diceLimit = 60;
#elif UNITY_ANDROID
        diceLimit = 30;
#endif
    }

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

    public void ExitApp()
    {
        Application.Quit();
    }


    private void Update()
    {
        if (Input.acceleration.x > 3f)
        {
            RollButton();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RollButton();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeDiceColor("white");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeDiceColor("red");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeDiceColor("black");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            failAudioSource.Play();
        }
#endif
    }
}