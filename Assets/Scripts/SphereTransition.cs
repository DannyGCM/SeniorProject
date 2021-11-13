using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SphereTransition : MonoBehaviour
{

    public InputActionReference homeButton;

    public Rigidbody cameraMove;

    public Transform homeEnvironment;

    public Transform tourEnvironment;

    public float transitionSpeed = 10;

    public float teleportDelay = 3;

    public Transform BuildingContainer;

    public Transform CameraContainer;

    Transform Camera;

    Transform Building;

    Transform BuildingModel;

    Transform tourSkysphere;

    Renderer tourSkysphereRenderer;

    public bool testing;

    public double expandRate = 4;

    public double maxDist = 0.65;

    public double minDist = 0.4;

    public double fadeRate = 2;

    double expandPower;

    double fadePower;

    double dist;

    public bool grabbed;

    public GameObject _Manager;

    Animator anim;



    // Start is called before the first frame update
    void Start()
    {
        homeButton.action.performed += HomeClicked;

        fadePower = -fadeRate;

        // Selects current rig
        bool isVR = onAndroid();

        // If we are testing, allow for override of device detected
        if (!testing)
        {
            // Sets a camera to be active based on the device detected
            CameraContainer.GetChild(1).gameObject.SetActive(isVR);
            CameraContainer.GetChild(0).gameObject.SetActive(!isVR);
        }

        // Set camera to be whichever is detected as active
        Camera = FindActiveCamera(CameraContainer);

        // Get buildingcontainer children
        // Add loop to add an array of buildings
        Building = BuildingContainer.GetChild(0);
        BuildingModel = Building.GetChild(0).GetChild(0);
        tourSkysphere = tourEnvironment.GetChild(0).GetChild(0);

        // Initialize renderer component of the skysphere
        tourSkysphereRenderer = tourSkysphere.GetComponent<Renderer>();

        grabbed = false;

        anim = BuildingModel.GetComponent<Animator>();
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

    // Determines if the current platform is Android or not
    private bool onAndroid()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Transform FindActiveCamera(Transform cameraContainer)
    {
        Transform c;
        // Check which camera is active
        if (cameraContainer.GetChild(1).gameObject.activeSelf)
        {
            c = cameraContainer.GetChild(1).GetChild(0).GetChild(0);
        }
        else
        {
            c = cameraContainer.GetChild(0).GetChild(0).GetChild(0);
        }

        return c;
    }
    // Update is called once per frame
    void Update()
    {
        // Set dist variable
        dist = Vector3.Distance(BuildingModel.position, Camera.position);

        // 

        if (dist < maxDist)
        {

            // Adjust transparencies
            double fadeVar = (Math.Pow(dist, fadePower) - Math.Pow(maxDist, fadePower));
            tourSkysphereRenderer.material.color = new Color(1, 1, 1, (float)(fadeVar));

        }
        else
        {
            tourSkysphereRenderer.material.color = new Color(1, 1, 1, 0);
            //tourSkysphere.localScale = new Vector3(1, 1, 1);
        }

       
    }

    public void GrabbableReleased()
    {

        if (dist < minDist)
        {
            // Lock transparency to 1
            tourSkysphereRenderer.material.color = new Color(1, 1, 1, 1);
            _Manager.GetComponent<ImageCycle>().BuildSkysphere(tourSkysphere);
        }
        anim.SetBool("InHand", false);

    }
    public void GrabbableGrabbed()
    {
        // Play animation
        
        //Animator animator = BuildingModel.GetComponent<Animator>();

        
        anim.SetBool("InHand", true);
        // Place material on skysphere
        _Manager.GetComponent<ImageCycle>().BuildSkysphere(tourSkysphere);  
    }
}
