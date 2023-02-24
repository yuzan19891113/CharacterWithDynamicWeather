using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickHandleRight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform handleTransform;
    public float maxRadius;
    public GameObject cinemaCamera;

    private Vector2 _pointDownPosition;

    public Vector2 moveInput => _moveInput;
    private Vector2 _moveInput;
    private CinemachineFreeLook cinemachine;

    void Start()
    {
        cinemachine = cinemaCamera.GetComponent<CinemachineFreeLook>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 distance = eventData.position - _pointDownPosition;
        _moveInput = distance.normalized;

        float dis = Mathf.Clamp(distance.magnitude, 0f, maxRadius);
        distance = dis * _moveInput;

        handleTransform.localPosition = distance;
        cinemachine.m_YAxis.m_InputAxisValue = _moveInput.y * 0.3f;
        cinemachine.m_XAxis.m_InputAxisValue = _moveInput.x * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointDownPosition = eventData.position;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleTransform.localPosition = Vector3.zero;
        _moveInput = Vector2.zero;
        cinemachine.m_YAxis.m_InputAxisValue = 0;
        cinemachine.m_XAxis.m_InputAxisValue = 0;
    }

}
