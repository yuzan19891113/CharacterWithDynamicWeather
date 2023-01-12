using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public bool isForword;
    public bool isBackward;
    public bool isLeftMove;
    public bool isRightMove;
    public bool isUpmove;
    public bool isDownMove;
    public bool isLookUp;
    public bool isLookDown;
    public bool isLookLeft;
    public bool isLookRight;


    void Update() {
        isForword = Keyboard.current.wKey.isPressed;
        isBackward = Keyboard.current.sKey.isPressed;
        isLeftMove = Keyboard.current.aKey.isPressed;
        isRightMove = Keyboard.current.dKey.isPressed;
        isUpmove = Keyboard.current.eKey.isPressed;
        isDownMove = Keyboard.current.qKey.isPressed;
        isLookLeft = Keyboard.current.leftArrowKey.isPressed;
        isLookRight = Keyboard.current.rightArrowKey.isPressed;
        isLookUp = Keyboard.current.upArrowKey.isPressed;
        isLookDown = Keyboard.current.downArrowKey.isPressed;
    }
}
