using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDrawArrow : MonoBehaviour
{
    public Canvas canvas;
    public Image lineImage;
    public Image arrowImage;

    public bool isActive;

    private void Start()
    {

    }

    private void Update()
    {
        if (isActive) UpdateArrow();
        IsActive(isActive);
    }

    public void IsActive(bool _isActive)
    {
        lineImage.gameObject.SetActive(_isActive);
        arrowImage.gameObject.SetActive(_isActive);
    }

    public void UpdateArrow()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        //work out scale
        float distanceFromMouse = Mathf.Sqrt(Mathf.Pow(mousePos.x - transform.position.x, 2) + Mathf.Pow(mousePos.y - transform.position.y, 2));
        //float scaleFactor = distanceFromMouse / scale;
        lineImage.rectTransform.sizeDelta = new Vector2(distanceFromMouse, lineImage.rectTransform.sizeDelta.y);

        //set the rotation of the arrow towards the mouse
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        //image position
        lineImage.rectTransform.localPosition = new Vector3(/*-image.rectTransform.sizeDelta.x / 2*/0, -lineImage.rectTransform.sizeDelta.y / 2, 0);

        //update arrow position
        Vector3 arrowPos = new Vector3(distanceFromMouse, 0, 0);
        arrowImage.rectTransform.localPosition = arrowPos;
    }
}
