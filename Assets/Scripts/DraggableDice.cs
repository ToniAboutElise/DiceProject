using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableDice : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private int randX;
    private int randY;
    private int randZ;

    protected bool canRotate = false;
    protected bool canRotateWithAcceleration = true;
    protected bool waitingForSingleRoll = false;

    public AudioSource diceAudioSource;
    protected bool canPlayAudio = true;

    public DiceMaterials diceMaterials;

    [System.Serializable]
    public struct DiceMaterials
    {
        public Material white;
        public Material red;
        public Material black;
        public Material alien;
        public Material blue;
    }

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

    private void OnMouseUp()
    {
        //StartCoroutine(RollDice());
        StartCoroutine(SingleRoll());
    }

    protected IEnumerator SingleRoll()
    {
        waitingForSingleRoll = true;
        yield return new WaitForSeconds(0.4f);
        waitingForSingleRoll = false;
    }

    protected IEnumerator RollDice()
    {
        canRotateWithAcceleration = false;

        randX = Random.Range(5, 20);
        randY = Random.Range(5, 20);
        randZ = Random.Range(5, 20);

        GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
        yield return new WaitForSeconds(0.2f);
        canRotate = true;
        //transform.Rotate(new Vector3(90 * randX, 90 * randY, 0), Space.Self);
        //transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(90 * randX, 90 * randY, 90 * randZ, transform.localRotation.w), 4 * Time.deltaTime);
        yield return new WaitForSeconds(0.3f);
        canRotate = false;
        canRotateWithAcceleration = true;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

    }

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

    protected IEnumerator AudioCoolDownCR()
    {
        yield return new WaitForSeconds(0.2f);
        canPlayAudio = true;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A) && canRotateWithAcceleration == true)
        {
            StartCoroutine(RollDice());
        }
#endif

        if(Input.acceleration.x > 3f && canRotateWithAcceleration == true)
        {
            StartCoroutine(RollDice());
        }

        if(canRotate == true)
        transform.Rotate(new Vector3(90*Time.deltaTime * randX, 90*Time.deltaTime * randY, 90*Time.deltaTime * randZ), Space.Self);

        if(GetComponent<Rigidbody>().velocity.x > 3f && canRotate == true && canRotateWithAcceleration == true)
        {
            StartCoroutine(RollDice());
        }
    }

}
