using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereTransition : MonoBehaviour
{

    public InputActionReference homeButton;

    public Rigidbody cameraMove;

    public Transform homeSphere;

    public Transform tourSphere;

    public float transitionSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        homeButton.action.performed += HomeClicked;
        
    }
    void HomeClicked(InputAction.CallbackContext obj)
    {
        // Transition is initiated
        if (cameraMove.velocity.y == 0)
        {
            cameraMove.velocity = new Vector3(0, transitionSpeed, 0);
        }
        // Transition is paused
        else
        {
            cameraMove.velocity = new Vector3(0, 0, 0);
        }

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
