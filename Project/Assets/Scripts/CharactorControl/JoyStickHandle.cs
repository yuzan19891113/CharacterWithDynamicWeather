using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickHandle : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public Transform handleTransform;
    public float maxRadius;

    private Vector2 _pointDownPosition;

    public Vector2 moveInput => _moveInput;
    private Vector2 _moveInput;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 distance = eventData.position - _pointDownPosition;
        _moveInput = distance.normalized;

        float dis = Mathf.Clamp(distance.magnitude, 0f, maxRadius);
        distance = dis * _moveInput;

        handleTransform.localPosition = distance;
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointDownPosition = eventData.position;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleTransform.localPosition = Vector3.zero;
        _moveInput = Vector2.zero;
    }

}
