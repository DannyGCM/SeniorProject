using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;


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

    public bool inTour;

    public GameObject _Manager;

    Animator anim;

    public ScriptableObject temp;



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

        inTour = false;

        anim = BuildingModel.GetComponent<Animator>();

        // Insert listeners for grab and release of every building
        InsertGrabListeners(BuildingContainer);

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
        if (inTour == false)
        {
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
        

       
    }

    // This adds listeners to every building so that we know what image to put on the sphere
    private void InsertGrabListeners(Transform buildingContainer)
    {
        XRGrabInteractable grab;
        string imageName;
        
        // Add event listeners to all grabbables in the canvas
        for (int i = 0; i < buildingContainer.childCount; i++)
        {

            grab = buildingContainer.GetChild(i).GetChild(0).GetComponent<XRGrabInteractable>();
            imageName = (string)Variables.Object(buildingContainer.GetChild(i).GetChild(0).GetChild(0).GetChild(0).gameObject).Get("imageName");

            grab.selectEntered.AddListener(delegate { GrabbableGrabbed(imageName); });
            grab.selectExited.AddListener(delegate { GrabbableReleased(imageName); });

        }

    }

    public void GrabbableReleased(string imgName)
    {

        if (dist < minDist)
        {
            // Lock transparency to 1
            tourSkysphereRenderer.material.color = new Color(1, 1, 1, 1);
            
            _Manager.GetComponent<ImageCycle>().SpawnButtons(_Manager.GetComponent<ImageCycle>().FindImageInTxt(imgName));
        }
        anim.SetBool("InHand", false);

        inTour = true;
    }
    public void GrabbableGrabbed(string imgName)
    {

        anim.SetBool("InHand", true);
        // Place material on skysphere
        Debug.Log(imgName);
        _Manager.GetComponent<ImageCycle>().BuildSkysphere(tourSkysphere, imgName);

    }
    
}
