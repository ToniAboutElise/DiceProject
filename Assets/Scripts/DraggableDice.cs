using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to manage the behaviour of dice individually

public class DraggableDice : MonoBehaviour
{
    public DiceManager diceManager;

    //Variables used for the mouse drag method
    private Vector3 screenPoint;
    private Vector3 offset;

    //Random rotation values
    private int randX;
    private int randY;
    private int randZ;

    //Variables used to avoid rotation instead of waiting for a new roll or reroll
    protected bool canRotate = false;
    protected bool canRotateWithAcceleration = true;
    protected bool waitingForSingleRoll = false;

    //Audio variables
    public AudioSource diceAudioSource;
    protected bool canPlayAudio = true;

    //Dice materials
    public DiceMaterials diceMaterials;

    //Face detection variables to determine result
    public string dieName;
    public string result;

    [System.Serializable]
    public struct DiceMaterials
    {
        public Material white;
        public Material red;
        public Material black;
        public Material alien;
        public Material blue;
    }

    //This method makes a roll for a single die if a double tap is happening
    void OnMouseDown()
    {
        if(waitingForSingleRoll == false)
        { 
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
        else
        {
            StartCoroutine(RollDice());
        }
    }

    //Calls SingleRoll coroutine on mouse up
    private void OnMouseUp()
    {
        StartCoroutine(SingleRoll());
    }

    //Calls RollDice coroutine through this method, to use it easier from another script and button if needed
    public void SingleRollCall()
    {
        StartCoroutine(RollDice());
    }

    //Corroutine to allows a die roll during the specified time
    protected IEnumerator SingleRoll()
    {
        waitingForSingleRoll = true;
        yield return new WaitForSeconds(0.4f);
        waitingForSingleRoll = false;
    }

    //Corroutine to roll die with random rotation variables
    protected IEnumerator RollDice()
    {
        canRotateWithAcceleration = false;

        randX = Random.Range(5, 20);
        randY = Random.Range(5, 20);
        randZ = Random.Range(5, 20);

        GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
        yield return new WaitForSeconds(0.2f);
        canRotate = true;
        yield return new WaitForSeconds(0.3f);
        canRotate = false;
        canRotateWithAcceleration = true;
        yield return new WaitForSeconds(2);
        CheckResult();
    }

    //Checks result
    protected void CheckResult()
    {
        if(result == null)
        {
            StartCoroutine(RollDice());
        }
    }

    //Mouse drag function, automatically translated to regular smartphone drag
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    //Detects if sound effect has to be played
    private void OnCollisionEnter(Collision collision)
    {
        if(diceAudioSource.isPlaying == false && canPlayAudio == true)
        {
            canPlayAudio = false;
            StartCoroutine(AudioCoolDownCR());
            diceAudioSource.pitch = 1;
            float rand = Random.Range(-0.25f, 0.25f);
            diceAudioSource.pitch += rand;
            diceAudioSource.Play();
        }
    }

    //Corroutine used to avoid sound effect replaying constantly
    protected IEnumerator AudioCoolDownCR()
    {
        yield return new WaitForSeconds(0.2f);
        canPlayAudio = true;
    }

    //Checks constantly if die can roll
    private void Update()
    {
        if(canRotate == true)
        transform.Rotate(new Vector3(90*Time.deltaTime * randX, 90*Time.deltaTime * randY, 90*Time.deltaTime * randZ), Space.Self);
    }

}
