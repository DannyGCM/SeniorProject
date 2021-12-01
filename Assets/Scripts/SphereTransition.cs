using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class SphereTransition : MonoBehaviour
{

    public InputActionReference homeButton;

    public InputActionReference gripButton;

    public Rigidbody cameraMove;

    public Transform homeEnvironment;

    public Transform tourEnvironment;

    public Transform BuildingContainer;

    public Transform CameraContainer;

    Transform Camera;

    Transform Building;

    Transform BuildingModel;

    Transform tourSkysphere;

    Renderer tourSkysphereRenderer;

    public bool testing;

    public double maxDist = 0.45;

    double dist;

    public bool inTour;

    public GameObject _Manager;

    Animator BuildingVisualsAnimator;

    Animator SphereChangeAnimator;
    Animator ArrowAppearAnimator;

    bool beenInTour = false;
    public bool started = false;

    public bool grip = false;

    // Start is called before the first frame update
    public void NewStart()
    {
        homeButton.action.performed += HomeClicked;
        


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
        
        BuildingVisualsAnimator = BuildingModel.GetComponent<Animator>();

        SphereChangeAnimator = tourSkysphere.GetComponent<Animator>();

        // Insert listeners for grab and release of every building
        //InsertGrabListeners(BuildingContainer);


        started = true;

    }
    void HomeClicked(InputAction.CallbackContext obj)
    {
        // If home clicked when in tour, go home
        if (inTour == true) {
            inTour = false;
            homeEnvironment.GetChild(1).gameObject.SetActive(true);
            _Manager.GetComponent<ImageCycle>().ClearButtons();
        }
        else
        {
            if (beenInTour == true)
            {
                SphereChangeAnimator.SetBool("BuildingNear", true);
                inTour = true;
                DisableHome(tourSkysphereRenderer.material.GetTexture("_BaseMap").name);
            }
            

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
        if (started == true) {
            // Set dist variable
            dist = Vector3.Distance(BuildingModel.position, Camera.position);

            // 
            if (inTour == false)
            {

                if (dist < maxDist)
                {
                    SphereChangeAnimator.SetBool("BuildingNear", true);
                }
                else
                {
                    SphereChangeAnimator.SetBool("BuildingNear", false);
                }
            }
            
            

        }
    }

    // This adds listeners to every building so that we know what image to put on the sphere
    public void InsertGrabListener(Transform b)
    {
        XRGrabInteractable grab;
        string imageName;

        grab = b.GetChild(0).GetComponent<XRGrabInteractable>();
            
        imageName = (string)Variables.Object(b.GetChild(0).GetChild(0).GetChild(0).gameObject).Get("imageName");

        Transform buildingVis = b.GetChild(0).GetChild(0);

        grab.selectEntered.AddListener(delegate { GrabbableGrabbed(imageName, buildingVis); } );
        grab.selectExited.AddListener(delegate { GrabbableReleased(imageName, buildingVis); } );

    }
   

    public void GrabbableReleased(string imgName, Transform buildingVisuals)
    {
        
        if (imgName == "")
        {
            imgName = tourSkysphereRenderer.material.GetTexture("_BaseMap").name;
        }
        buildingVisuals.GetComponent<Animator>().SetBool("InHand", false);
        
        if (dist < maxDist)
        {
            //Debug.Log(imgName);
            DisableHome(imgName);
            beenInTour = true;
        }
        
    }
    public async Task DisableHome(string imgName)
    {
        Debug.Log(imgName);
        // Lock transparency to 1
        tourSkysphereRenderer.material.color = new Color(1, 1, 1, 1);

        _Manager.GetComponent<ImageCycle>().SpawnButtons(_Manager.GetComponent<ImageCycle>().FindImageInTxt(imgName));
        inTour = true;
        // Do camera fadeout BuildingVisualsAnimatoration
        //SphereChangeAnimator.SetBool("BuildingNear", true);
        await Task.Delay(1000);
        
        // Disable map
        homeEnvironment.Find("Map").gameObject.SetActive(false);
    }
    public void GrabbableGrabbed(string imgName, Transform buildingVisuals)
    {
        
            BuildingModel = buildingVisuals.Find("Model");
            buildingVisuals.GetComponent<Animator>().SetBool("InHand", true);

            // Place material on skysphere
            _Manager.GetComponent<ImageCycle>().BuildSkysphere(tourSkysphere, imgName);

        

    }
    
}