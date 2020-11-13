using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSplitter : MonoBehaviour
{
    [SerializeField]
    private Transform spawnerRoot;
    [SerializeField]
    private Transform spawnPos;
    [SerializeField]
    private TextMeshPro characterPrefab;
    [SerializeField]
    private TextMeshPro sentencePrefab;
    [SerializeField]
    [Range(1, 20)]
    private float rotationSpeed;

    private TextMeshPro textObj;
    private bool launched = false;

    private Controls.PlayerActions input;
    // Start is called before the first frame update
    void Start()
    {
        input = new Controls().Player;
        input.Enable();
        input.Shoot.performed += Shoot_performed;
        //Invoke("split", 1);
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (textObj != null)
        {
            if (!launched)
            {
                launched = true;
                //textObj.GetComponent<FixedJoint2D>().connectedBody = null;
                textObj.GetComponent<FixedJoint2D>().enabled = false;
                Rigidbody2D r = textObj.GetComponent<Rigidbody2D>();
                r.simulated = true;
                r.gravityScale = .5f;
                r.AddForce(-textObj.transform.up * 5, ForceMode2D.Impulse);
            }
            else
            {
                split();
            }
        }
        else
        {
            textObj = Instantiate(sentencePrefab, spawnPos.position + new Vector3(0, -1), Quaternion.identity);
            textObj.GetComponent<FixedJoint2D>().connectedBody = spawnerRoot.GetComponent<Rigidbody2D>();
            textObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed) * 45; //tweak this to change frequency
        spawnerRoot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (textObj != null && !launched)
        {
            textObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            textObj.transform.position = spawnPos.position - spawnPos.up;
        }
    }
    private void split()
    {
        textObj.enableAutoSizing = false;
        for (int i = 0; i < textObj.textInfo.characterInfo.Length; i++)
        {
            TMP_CharacterInfo character = textObj.textInfo.characterInfo[i];
            if (character.character.Equals(' ')) continue;
            TextMeshPro characterTMP = Instantiate(characterPrefab);
            characterTMP.text = character.character.ToString();
            Vector3 topLeft = textObj.transform.TransformPoint(character.topLeft);
            Vector3 bottomRight = textObj.transform.TransformPoint(character.bottomRight);
            Rect rect = new Rect(topLeft.x, topLeft.y, Mathf.Abs(topLeft.x - bottomRight.x), Mathf.Abs(topLeft.y - bottomRight.y));
            characterTMP.rectTransform.position = rect.center;
            characterTMP.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            characterTMP.GetComponent<BoxCollider2D>().size = new Vector2(rect.width, rect.height);
            characterTMP.GetComponent<Rigidbody2D>().velocity = textObj.GetComponent<Rigidbody2D>().velocity;
            //characterTMP.gameObject.AddComponent<BoxCollider2D>();
            //characterTMP.gameObject.AddComponent<Rigidbody2D>();
        }
        launched = false;
        Destroy(textObj.gameObject);
    }

}
