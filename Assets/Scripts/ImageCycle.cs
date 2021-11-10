using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.UI;

public class ImageCycle : MonoBehaviour
{

    Renderer rend;

    public Transform skySphere;

    public Transform rotationPlane;

    public Transform rotationPlaneContainer;

    public string resourcesSkysphereImageDirectory = "Textures/Sky";

    public TextAsset mapAsset;

    string[] lines;

    private Object[] texSphere;


    int rotPlaneIteration = 0;

    // Starting spawn skybox name
    public string spawnImage = "csc0";

    // Start is called before the first frame update
    public void Start()
    {
        // Read txt file to build our button map
        lines = mapAsset.text.Split("\n"[0]);
        
        // Get the renderer component of Transform we want
        rend = skySphere.GetComponent<Renderer>();

        // Load all the skysphere textures
        texSphere = Resources.LoadAll(resourcesSkysphereImageDirectory, typeof(Texture2D));
        int foundImgInd = FindImgIndex(spawnImage, texSphere);
        rend.material.SetTexture("_MainTex", (Texture2D)texSphere[foundImgInd]);

        // Assemble buttons
        SpawnButtons(FindImageInTxt(spawnImage));

    }

    // Returns array of strings representing the line found in text file
    private string[] FindImageInTxt(string imgName)
    {
        
        for (int i = 0; i < lines.Length; i++)
        {
            // Split up our lines into useable information
            string[] line = lines[i].Split(","[0]);
            
            // Find line that matches our current location
            if (line.Length > 1)
            {
                
                if (line[1].Trim() == imgName.Trim())
                    return line;
            }
                
        }

        Debug.Log("Null sent");
        return null;
    }
   
    private int FindImgIndex(string imgName, Object[] texSphere)
    {
        imgName = imgName.Trim();
        for (int i = 0; i < texSphere.Length; i++)
        {
            string texName = texSphere[i].name.Trim();
            
            if (imgName == texName)
            {
                return i;
            }
        }
        return -1;
    }


    private void ClearButtons()
    {
        for (int i = 0; i < rotationPlaneContainer.childCount; i++)
        {
            Destroy(rotationPlaneContainer.GetChild(i).gameObject);
        }
    }

    private void SpawnButtons(string[] line)
    {
        Button[] buttons = new Button[(line.Length - 2) / 2];
        int trueind = 0;
        
        // Button positions begin at index 2
        for (int i = 2; i < line.Length; i = i+2)
        {
            trueind = (i - 2) / 2;
            int rotationy = int.Parse(line[i]);

            // Create instance of our rotationPlane prefab
            Transform rotationPlaneClone = Instantiate(rotationPlane);

            // Set this instance's parent to be in a container for organization
            rotationPlaneClone.parent = rotationPlaneContainer;
            // Set position of rotation plane to be bound to parent
            rotationPlaneClone.position =  rotationPlaneContainer.position;
            // With this instance, rotate it to degree specified from txt file
            rotationPlaneClone.transform.Rotate(0, rotationy, 0);
            // Set this instance's name to be 'rotationPlane[integer]'.
            rotationPlaneClone.name = rotationPlane.name + (trueind + rotPlaneIteration); // rotPlaneIteration is used to ensure that no old names can be reused once a button is deleted. This is primarily used for debugging

            buttons[trueind] = rotationPlaneClone.GetChild(0).GetChild(0).GetComponent<Button>();
            
        }
        // Add event listeners to all buttons in the canvas
        for (int i = 0; i < buttons.Length; i++)
        {
            
            Button identifier = buttons[i];
            
            buttons[i].onClick.AddListener(delegate { ButtonClicked(identifier); });

            
        }
        rotPlaneIteration += (line.Length - 2) / 2;
    }

    // Takes an input rotation (presumably of the button pressed) and tries to find that rotation in the txt file
    private string FindButtonClicked(string[] line, Quaternion rotation)
    {
        for (int i = 2; i < line.Length; i += 2)
        {
           
            // Convert our text file angle to quaternion and compare
            if (rotation == Quaternion.Euler(0, int.Parse(line[i]), 0))
            {
                // Return the imgname that follows the rotation angle
                return line[i + 1];
            }
        }
        return null;
    }

    // Called when a listener function is triggered by a button being clicked. Functions similar to OnClick()
    public void ButtonClicked(Button button)
    {
        // Get the parent rotation plane of the button that was pressed
        Transform rotationPlane = button.transform.parent.parent.transform;

        // Get name of image that is currently applied to our material
        string imgName = rend.material.GetTexture("_MainTex").name;

        // Eliminate whitespace from imgName
        imgName = imgName.Trim();

        // Find the line in the text file that has this imgName as the origin (An origin is the image space you are currently in, non-origins are images you can click to)
        string[] line = FindImageInTxt(imgName);

        // From the line found, find an angle in the text that matches the one of the button that was clicked and return the image associated
        string nextImg = FindButtonClicked(line, rotationPlane.rotation);

        // Eliminate whitespace from the string nextImg
        nextImg = nextImg.Trim();

        // Look through our Images file within project and find an image name that matches our nextImg string
        int foundImgInd = FindImgIndex(nextImg, texSphere);

        // Get rid of old buttons
        ClearButtons();

        // Place the image that we found in our files onto our material
        rend.material.SetTexture("_MainTex", (Texture2D)texSphere[foundImgInd]);
       
        // Set nextImg to be the new origin image and spawn the buttons found from the line associated with the origin image
        SpawnButtons(FindImageInTxt(nextImg));
    }

    public void BuildSkysphere(Transform ImageBubble)
    {

        Renderer ImageBubbleRenderer= ImageBubble.GetComponent<Renderer>();

        Texture img = ImageBubbleRenderer.material.GetTexture("_MainTex");

        string imgName = img.name.Trim();

        rend.material.SetTexture("_MainTex", img);

        // Get rid of old buttons
        ClearButtons();

        // Set nextImg to be the new origin image and spawn the buttons found from the line associated with the origin image
        SpawnButtons(FindImageInTxt(imgName));
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
}

