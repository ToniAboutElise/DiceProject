using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public List<Rigidbody> dicesRB;
    public Text infoText;
    public GameObject capsule;
    public GameObject dice;

    public GameObject diceD3;
    public GameObject diceD4;
    public GameObject diceD6;
    public GameObject diceD8;
    public GameObject diceD10;
    public GameObject diceD12;
    public GameObject diceD20;
    public Transform[] diceSpawnTransform;
    public List<DraggableDice> diceList;

    public string currentDiceColor = "white";

    protected void DiceAppearToPoint()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            GameObject instance = Instantiate(dice);
            touchPosition.z = dice.transform.position.z;
            instance.transform.localPosition = touch.position;
            infoText.text = touch.position.ToString();
        }
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

    public void ClearDices()
    {
        DraggableDice[] dices = FindObjectsOfType<DraggableDice>();

        foreach(DraggableDice d in dices)
        {
            Destroy(d.gameObject);
        }
        diceList.Clear();
    }

    private void Update()
    {
        DiceAppearToPoint();
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
