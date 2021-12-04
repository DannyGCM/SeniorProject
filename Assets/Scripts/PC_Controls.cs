using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PC_Controls : MonoBehaviour
{

    // Mouse actions
    public InputActionReference mouseX;
    public InputActionReference mouseY;
    public InputActionReference press;
    public InputActionReference click;
    public InputActionReference hold;
    public InputActionReference release;

    // Movement variables
    public float lookSpeed = 1f;
    public bool invertedCamera = false;

    // Camera
    public Transform cameraTransform;

    // Movement variables
    private bool held = false;

    // Start is called before the first frame update
    void Start()
    {

        // Locks cursor FOR TESTING ONLY /////////////////////////////////////
        // Cursor.visible = false;

        // Binds actions
        mouseX.action.performed += HandleMouseX;
        mouseY.action.performed += HandleMouseY;
        press.action.performed += HandlePress;
        click.action.performed += HandleClick;
        hold.action.performed += HandleHold;
        release.action.performed += HandleRelease;

    }

    void Update()
    { }

    // Mouse X movement when held
    void HandleMouseX(InputAction.CallbackContext obj) {

        if (this.held)
        {
            Vector3 CameraRotation = cameraTransform.transform.rotation.eulerAngles;
            if (!invertedCamera)
            { CameraRotation.y += obj.ReadValue<float>() * lookSpeed * Time.deltaTime; }
            else { CameraRotation.y -= obj.ReadValue<float>() * lookSpeed * Time.deltaTime; }
            cameraTransform.transform.rotation = Quaternion.Euler(CameraRotation);
        }

    }

    // Mouse Y movement when held
    void HandleMouseY(InputAction.CallbackContext obj) {

        if (this.held)
        {
            Vector3 CameraRotation = cameraTransform.transform.rotation.eulerAngles;
            if (!invertedCamera)
            { CameraRotation.x -= obj.ReadValue<float>() * lookSpeed * Time.deltaTime;
            } else { CameraRotation.x += obj.ReadValue<float>() * lookSpeed * Time.deltaTime; }
            cameraTransform.transform.rotation = Quaternion.Euler(CameraRotation);
        }

    }

    // Left click was pressed
    void HandlePress(InputAction.CallbackContext obj)
    {
    }

    // Left click was pressed and released
    void HandleClick(InputAction.CallbackContext obj)
    {
    }

    // Changes held status
    void HandleHold(InputAction.CallbackContext obj) { this.held = true; }
    void HandleRelease(InputAction.CallbackContext obj) { this.held = false; }

    /*

    // Mouse input handler
    void HandleMouseX(InputAction.CallbackContext obj)
    {

        Vector3 CameraRotation = cameraTransform.transform.rotation.eulerAngles;
        CameraRotation.y += obj.ReadValue<float>() * lookSpeed * Time.deltaTime;
        cameraTransform.transform.rotation = Quaternion.Euler(CameraRotation);
    
    }

    void HandleMouseY(InputAction.CallbackContext obj)
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

    } */

}
