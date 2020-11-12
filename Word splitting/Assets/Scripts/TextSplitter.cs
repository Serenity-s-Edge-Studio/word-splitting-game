using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSplitter : MonoBehaviour
{
    [SerializeField]
    private Transform spawnerRoot;
    [SerializeField]
    private TextMeshPro textObj;
    [SerializeField]
    private TextMeshPro characterPrefab;
    [SerializeField]
    [Range(1, 20)]
    private float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("split", 1);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * 10) * 45; //tweak this to change frequency

        spawnerRoot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void split()
    {
        //_ = textObj.textInfo.characterInfo;
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
            //characterTMP.gameObject.AddComponent<BoxCollider2D>();
            //characterTMP.gameObject.AddComponent<Rigidbody2D>();
        }
        Destroy(textObj.gameObject);
    }

}
