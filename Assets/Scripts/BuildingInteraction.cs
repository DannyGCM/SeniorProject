using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingInteraction : MonoBehaviour
{

    public Transform BuildingContainer;

    public Transform CameraContainer;

    Transform Camera;

    Transform Building;

    Transform BuildingModel;

    Transform ImageBubble;

    Renderer ImageBubbleRenderer;

    public double expandRate = 4;

    public double maxDist = 0.65;

    public double minDist = 0.4;

    public double fadeRate = 2;

    double expandPower;

    double fadePower;

    double dist;

    public bool grabbed;


    // Start is called before the first frame update
    void Start()
    {
        expandPower = -expandRate;
        fadePower = -fadeRate;

        /*// Selects current rig
        bool isVR = onAndroid();

        // Removes either PC or VR controls
        CameraContainer.GetChild(1).gameObject.SetActive(isVR);
        CameraContainer.GetChild(0).gameObject.SetActive(!isVR);

        if (!isVR)
        {
            Camera = CameraContainer.GetChild(0).GetChild(0).GetChild(0);
        }
        else {
            Camera = CameraContainer.GetChild(0).GetChild(0).GetChild(0);
        }*/

        Camera = CameraContainer.GetChild(0).GetChild(0).GetChild(0);

        Building = BuildingContainer.GetChild(0);
        BuildingModel = Building.GetChild(0);
        ImageBubble = Building.GetChild(1);

        ImageBubbleRenderer = ImageBubble.GetComponent<Renderer>();

        grabbed = false;
    }

    // Determines if the current platform is Android or not
    private bool onAndroid() {
        Debug.Log("yeet");
        if (Application.platform == RuntimePlatform.Android) {
            return true;
        }
        else {
            return false;
        }
    }
 

    // Update is called once per frame
    void Update()
    {
        
        dist = Vector3.Distance(BuildingModel.position, Camera.position);

        if (dist < maxDist) {

            float scaleVar = (float)(Math.Pow(dist, expandPower) - Math.Pow(maxDist, expandPower));

            ImageBubble.localScale = new Vector3(scaleVar+1, scaleVar+1, scaleVar+1);

            // Adjust transparencies
            double fadeVar = (Math.Pow(dist, fadePower) - Math.Pow(maxDist, fadePower));
            ImageBubbleRenderer.material.color = new Color(1, 1, 1, (float)(fadeVar));

        }
        else
        {
            ImageBubbleRenderer.material.color = new Color(1,1,1,0);
            ImageBubble.localScale = new Vector3(1,1,1);
        }
        
    }

    public void Released()
    {
        if (dist < minDist)
        {
            // Place material on skysphere

            // Trigger transition
            Debug.Log("released at min");
        }


    }
}
