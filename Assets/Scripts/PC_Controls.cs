using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PC_Controls : MonoBehaviour
{

    // Mouse actions
    public InputActionReference mouseH;
    public InputActionReference mouseV;

    // Keyboard actions
    public InputActionReference moveFront;
    public InputActionReference moveBack;
    public InputActionReference moveLeft;
    public InputActionReference moveRight;
    public InputActionReference panUp;
    public InputActionReference panDown;

    // Camera
    public Transform cameraTransform;

    // Movement speed
    public float lookSpeed = 1f;
    public float moveSpeed = 1f;

    // Status variables
    private bool movingFront = false;
    private bool movingBack = false;
    private bool movingLeft = false;
    private bool movingRight = false;
    private bool panningUp = false;
    private bool panningDown = false;

    // Start is called before the first frame update
    void Start()
    {

        // Locks cursor
        Cursor.visible = false;

        // Binds mouse actions
        mouseH.action.performed += HandleMouseH;
        mouseV.action.performed += HandleMouseV;

        // Binds keyboard actions
        moveFront.action.performed += HandleMoveFront;
        moveBack.action.performed += HandleMoveBack;
        moveLeft.action.performed += HandleMoveLeft;
        moveRight.action.performed += HandleMoveRight;
        panUp.action.performed += HandlePanUp;
        panDown.action.performed += HandlePanDown;

    }

    // Mouse input handler
    void HandleMouseH(InputAction.CallbackContext obj)
    {

        Vector3 CameraRotation = cameraTransform.transform.rotation.eulerAngles;
        CameraRotation.y += obj.ReadValue<float>() * lookSpeed * Time.deltaTime;
        cameraTransform.transform.rotation = Quaternion.Euler(CameraRotation);
    
    }

    void HandleMouseV(InputAction.CallbackContext obj)
    {

        Vector3 CameraRotation = cameraTransform.transform.rotation.eulerAngles;
        CameraRotation.x -= obj.ReadValue<float>() * lookSpeed * Time.deltaTime;
        cameraTransform.transform.rotation = Quaternion.Euler(CameraRotation);

    }

    // Keyboard input handlers
    void HandleMoveFront(InputAction.CallbackContext obj)
    { this.movingFront = !this.movingFront; }

    void HandleMoveBack(InputAction.CallbackContext obj)
    { this.movingBack = !this.movingBack; }

    void HandleMoveLeft(InputAction.CallbackContext obj)
    { this.movingLeft = !this.movingLeft; }

    void HandleMoveRight(InputAction.CallbackContext obj)
    { this.movingRight = !this.movingRight; }

    void HandlePanUp(InputAction.CallbackContext obj)
    { this.panningUp = !this.panningUp; }

    void HandlePanDown(InputAction.CallbackContext obj)
    { this.panningDown = !this.panningDown; }

    // Movement updates
    void MoveFront() { cameraTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime); }

    void MoveBack() { cameraTransform.Translate(Vector3.back * moveSpeed * Time.deltaTime); }

    void MoveLeft() { cameraTransform.Translate(Vector3.left * moveSpeed * Time.deltaTime); }

    void MoveRight() { cameraTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime); }

    void PanUp() { cameraTransform.Translate(Vector3.up * moveSpeed * Time.deltaTime); }

    void PanDown() { cameraTransform.Translate(Vector3.down * moveSpeed * Time.deltaTime); }

    // Update is called once per frame
    void Update()
    {

        // Performs keyboard movement update
        if (this.movingFront) { MoveFront(); }
        if (this.movingBack) { MoveBack(); }
        if (this.movingLeft) { MoveLeft(); }
        if (this.movingRight) { MoveRight(); }
        if (this.panningUp) { PanUp(); }
        if (this.panningDown) { PanDown(); }

    }

}
