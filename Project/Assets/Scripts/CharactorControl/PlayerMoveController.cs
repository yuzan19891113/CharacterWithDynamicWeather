using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public float moveSpeed = 5;
    public float turnSpeed = 60;

    public PlayerInputController input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (input.isForword)
        {
            transform.Translate(new Vector3(0, 0, moveSpeed * Time.deltaTime));
        }
        if (input.isBackward)
        {
            transform.Translate(new Vector3(0, 0, -moveSpeed * Time.deltaTime));
        }
        if (input.isLeftMove)
        {
            transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
        }
        if (input.isRightMove)
        {
            transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
        }
        if (input.isUpmove)
        {
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        }
        if (input.isDownMove)
        {
            transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
        }
        if (input.isLookLeft)
        {
            transform.Rotate(new Vector3(0, 1, 0), -turnSpeed * Time.deltaTime);
        }
        if (input.isLookRight)
        {
            transform.Rotate(new Vector3(0, 1, 0), turnSpeed * Time.deltaTime);
        }
        if (input.isLookUp)
        {
            transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.deltaTime);
        }
        if (input.isLookDown)
        {
            transform.Rotate(new Vector3(1, 0, 0), turnSpeed * Time.deltaTime);
        }
    }
}
