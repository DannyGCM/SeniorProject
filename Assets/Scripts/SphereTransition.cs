using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine.UI;

// Brock Wilson
// 12/4/21
// Florida Southern College
// Senior Project


public class SphereTransition : MonoBehaviour
{

    public InputActionReference lHomeButton;
    public InputActionReference rHomeButton;

    public InputActionReference gripButton;

    public Rigidbody cameraMove;

    public Transform homeEnvironment;

    public Transform tourEnvironment;

    public Transform BuildingContainer;

    public Transform CameraContainer;

    Transform Camera;

    Transform BuildingModel;

    Transform tourSkysphere;

    Renderer tourSkysphereRenderer;

    public bool testing;

    public double maxDist = 0.45;

    double dist;

    public bool inTour;

    public GameObject _Manager;

    Animator SphereChangeAnimator;

    bool beenInTour = false;

    public bool started = false;

    public bool grip = false;

    public InputActionReference rClick = null;
    public InputActionReference lClick = null;

    Transform globalBuildingVisuals;

    string globalImgName;

    public Transform TourInteractables;

    // Start is called before the first frame update
    public void NewStart()
    {
        lHomeButton.action.performed += HomeClicked;
        rHomeButton.action.performed += HomeClicked;

        // Selects current rig
        bool isVR = onAndroid();

        // If we are testing, allow for override of device detected

        if (!testing)
        {
            // Sets active components based on the device detected
            CameraContainer.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(isVR);
            CameraContainer.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(isVR);
            CameraContainer.GetChild(0).gameObject.SetActive(!isVR);

        }

        // Set camera to be whichever is detected as active
        Camera = FindActiveCamera(CameraContainer);

        tourSkysphere = tourEnvironment.GetChild(0).GetChild(0);

        // Initialize renderer component of the skysphere
        tourSkysphereRenderer = tourSkysphere.GetComponent<Renderer>();

        inTour = false;

        SphereChangeAnimator = tourSkysphere.GetComponent<Animator>();

        started = true;

        rClick.action.started += delegate
        {
            AnnoyingClickFunction();
        };

        lClick.action.started += delegate {
            AnnoyingClickFunction();
        };

    }
    void AnnoyingClickFunction()
    {
        if (globalImgName != null && globalBuildingVisuals != null)
        {

            globalBuildingVisuals.parent.GetComponent<MeshCollider>().enabled = false;
            DisableHome(globalImgName);
            globalBuildingVisuals.parent.GetComponent<MeshCollider>().enabled = true;
            globalImgName = null;
            globalBuildingVisuals = null;
        }

    }
    void HomeClicked(InputAction.CallbackContext obj)
    {

        // If home clicked when in tour, go home
        if (inTour == true)
        {
            inTour = false;
            homeEnvironment.GetChild(1).gameObject.SetActive(true);
            _Manager.GetComponent<ImageCycle>().ClearButtons();
            TourInteractables.GetComponent<Animator>().SetBool("NewImage", false);
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

        if (BuildingModel)
        {

            if (inTour == false)
            {
                dist = Vector3.Distance(BuildingModel.position, Camera.position);
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
    public void InsertGrabListener(Transform grabBuilding)
    {
        XRGrabInteractable grab;
        string imageName;

        grab = grabBuilding.GetChild(0).GetComponent<XRGrabInteractable>();
        try
        {
            imageName = (string)Variables.Object(grabBuilding.gameObject).Get("imgName");
        }
        catch
        {
            imageName = "S_SPIVEY_JR";
        }
        if (imageName == "t") imageName = "S_SPIVEY_JR";

        Transform buildingVis = grabBuilding.GetChild(0).GetChild(0);

        grab.selectEntered.AddListener(delegate { GrabbableGrabbed(imageName, buildingVis); });
        grab.selectExited.AddListener(delegate { GrabbableReleased(imageName, buildingVis); });
        grab.hoverEntered.AddListener(delegate { GrabbableHovered(imageName, buildingVis); });
        grab.hoverExited.AddListener(delegate { GrabbableUnHovered(buildingVis); });

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
            DisableHome(imgName);
            beenInTour = true;
        }
        buildingVisuals.parent.GetComponent<MeshCollider>().enabled = true;

    }
    public async Task DisableHome(string imgName)
    {

        beenInTour = true;

        _Manager.GetComponent<ImageCycle>().LoadSkysphere(imgName);
        // Lock transparency to 1
        //tourSkysphereRenderer.material.color = new Color(1, 1, 1, 1);
        SphereChangeAnimator.SetBool("BuildingNear", true);

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
        _Manager.GetComponent<ImageCycle>().FindAndSetTextureOfSkySphere(imgName);

        buildingVisuals.parent.GetComponent<MeshCollider>().enabled = false;
    }

    public void GrabbableHovered(string imgName, Transform buildingVisuals)
    {
        BuildingModel = buildingVisuals.Find("Model");
        globalImgName = imgName;
        globalBuildingVisuals = buildingVisuals;

        buildingVisuals.GetComponent<Animator>().SetBool("Hover", true);
        //Debug.Log(buildingVisuals.Find("Model").GetChild(0).name);

        string name = buildingVisuals.Find("Model").GetChild(0).name;
        string text = name.Replace("_", " ").Replace("(Clone)", "");
        Transform textbox = buildingVisuals.Find("TooltipParent").Find("Tooltip").Find("Canvas").Find("Text");
        textbox.GetComponent<Text>().text = text;

        // If click occurs, call in new environment


    }
    public void GrabbableUnHovered(Transform buildingVisuals)
    {
        globalImgName = null;
        globalBuildingVisuals = null;
        //Debug.Log("unhovered");
        buildingVisuals.GetComponent<Animator>().SetBool("Hover", false);
    }

}