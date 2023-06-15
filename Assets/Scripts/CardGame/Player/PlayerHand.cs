using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; //for editor
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public bool cardHover; //if cards can hover
    public GameObject cardInfo;

    [Header("Alignment Attributes")]
    public float maxRotation = 10f; //max rotation for cards in hand
    public float spacingX = 100f;
    [Range(0, 1)]
    public float spacingY = 1f;
    public float initialHeightY = 98f;
    public float endCardHeight = 8f;

    // Start is called before the first frame update
    void Start()
    {
        cardHover = true;
    }

    // Update is called once per frame
    void Update()
    {
        AlignCards();
    }

    public void AlignCards()
    {
        //set each card at a rotation
        int numOfCards = transform.childCount;

        if (numOfCards > 3)
        {
            gameObject.GetComponent<HorizontalLayoutGroup>().enabled = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                //rotation of cards
                float rotation = maxRotation - (((maxRotation * 2) / (numOfCards - 1)) * i);
                transform.GetChild(i).eulerAngles = new Vector3(0f, 0f, rotation);

                //position of cards
                float posX = ((spacingX / 2) * (numOfCards - 1) * -1 + (i * spacingX));
                float posY = initialHeightY - (Mathf.Pow(posX * 0.1f * spacingY, 2));

                //quick fix
                if (i == 0 || i == transform.childCount - 1)
                {
                    posY -= endCardHeight;
                }

                //update the card's position
                transform.GetChild(i).localPosition = new Vector3(posX, posY, 0);
            }
        }
        else
        {
            gameObject.GetComponent<HorizontalLayoutGroup>().enabled = true;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
        }
    }
}
