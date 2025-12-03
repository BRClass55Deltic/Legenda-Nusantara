using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveMobile : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image Bgimage;
    private Image joystickimage;

    public Vector2 Inputdir;
    public float offset = 2f;

    public GameObject Obyekyangdigerakan;
    public GameObject obyekyangdiputar;
    public float SpeedMax = 5f;
    public float Zaxis;
    
    // Start is called before the first frame update
    void Start()
    {
        Bgimage = GetComponent<Image>();
        joystickimage = transform.GetChild(0).GetComponent<Image>();
        Inputdir = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        float bgimageSizeX = Bgimage.rectTransform.sizeDelta.x;
        float bgimageSizeY = Bgimage.rectTransform.sizeDelta.y;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Bgimage.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x/=bgimageSizeX;
            pos.y/=bgimageSizeY;

            Inputdir = new Vector2(pos.x, pos.y);
            Inputdir = Inputdir.magnitude>1 ? Inputdir : Inputdir.normalized;
            joystickimage.rectTransform.anchoredPosition = new Vector2(
                Inputdir.x * (bgimageSizeX/offset),
                Inputdir.y * (bgimageSizeY/offset)
                    );
        //rotation
        Zaxis = Mathf.Atan2(Inputdir.x, Inputdir.y) * Mathf.Rad2Deg;
        obyekyangdiputar.transform.eulerAngles = new Vector3(0, Zaxis, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Inputdir = Vector2.zero;
        joystickimage.rectTransform.anchoredPosition = Vector2.zero;
    }

    void LateUpdate()
    {
        Obyekyangdigerakan.transform.Translate(
            Inputdir.x * SpeedMax * Time.deltaTime, 0, Inputdir.y * SpeedMax * Time.deltaTime
        );
    }
}
