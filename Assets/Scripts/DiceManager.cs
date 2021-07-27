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

    public string currentDiceColor = "white";

    private void Start()
    {
        finalResultDisplayUI.SetActive(false);
    }

    public void SpawnDice(string dice)
    {
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
                foreach(DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.white;
                    if(textureHasBeenSet == false)
                    {
                        currentDiceColor = "white";
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "red":
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.red;
                    if (textureHasBeenSet == false)
                    {
                        currentDiceColor = "red";
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "black":
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.black;
                    if (textureHasBeenSet == false)
                    {
                        currentDiceColor = "black";
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "alien":
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.alien;
                    if (textureHasBeenSet == false)
                    {
                        currentDiceColor = "alien";
                        textureHasBeenSet = true;
                    }
                }
                break;
            case "blue":
                foreach (DraggableDice dd in diceList)
                {
                    dd.GetComponent<MeshRenderer>().material = dd.diceMaterials.blue;
                    if (textureHasBeenSet == false)
                    {
                        currentDiceColor = "blue";
                        textureHasBeenSet = true;
                    }
                }
                break;
        }
    }

    public void RollButton()
    {
        foreach(DraggableDice d in diceList)
        {
            d.SingleRollCall();
        }
        StartCoroutine(WaitForResult());
    }

    public IEnumerator WaitForResult()
    {
        bool allResultsReady = true;

        yield return new WaitForSeconds(2.6f);

        foreach (DraggableDice dd in diceList)
        {
            if(dd.result == null)
            {
                allResultsReady = false;
            }
        }

        if(allResultsReady == true)
        {
            DisplayResult();
            StopCoroutine(WaitForResult());
        }
        else
        {
            StartCoroutine(WaitForResult());
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
            finalResult += int.Parse(dd.result);
        }
        finalResultText.text = finalResult.ToString();
        finalResultDisplayUI.SetActive(true);
    }

    public void ClearDices()
    {
        DraggableDice[] dices = FindObjectsOfType<DraggableDice>();

        foreach(DraggableDice d in dices)
        {
            Destroy(d.gameObject);
        }
        diceList.Clear();
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
#endif
    }

}
