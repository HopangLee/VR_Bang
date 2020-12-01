using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MonoBehaviour
{
    List<GameObject> cards = new List<GameObject>();

    public GameObject sample_card;
    public Transform IC;

    float offset = 0.14f;

    System.Random rand = new System.Random();

    private Object[] materials;

    // Start is called before the first frame update
    void Start()
    {
        materials = Resources.LoadAll("Materials/Card/Event");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NewCard()
    {
        // 일단 sample_card로 빈 카드 만들기
        GameObject new_card = Instantiate(sample_card, transform.position, transform.rotation * Quaternion.Euler(0, 180f, 0));

        // Material 적용
        new_card.GetComponent<Renderer>().material = (Material)materials[rand.Next(0, materials.Length)];

        // 카드 뒤집혀있어서 돌려줌
        cards.Add(new_card);

        // 카드마다 위치 설정
        for(int i=0; i<cards.Count; i++) {
            float start_pos = -offset/2 * cards.Count;
            cards[i].transform.position = transform.position + new Vector3(start_pos + i*offset, 0, -0.02f);
        }
    }
}
