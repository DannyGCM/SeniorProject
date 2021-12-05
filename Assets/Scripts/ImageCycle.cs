using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;
using System.Threading.Tasks;

// Brock Wilson
// 12/4/21
// Florida Southern College
// Senior Project

public class ImageCycle : MonoBehaviour
{

    Renderer rend;

    public Transform skySphere;

    public Transform rotationPlane;

    public Transform rotationPlaneContainer;

    public string resourcesSkysphereImageDirectory = "Textures/Sky";

    public TextAsset mapAsset;

    public int mapAssetImageIndex = 0;

    public int mapAssetOffsetIndex = 1;

    public int mapAssetAudioIndex = 2;

    public int mapAssetDescriptionIndex = 3;

    public int mapAssetButtonsStartIndex = 1;

    public InputActionReference rClick = null;

    public InputActionReference lClick = null;

    int globalClicks = 0;

    public Transform DescriptionPanel;


    // Contains all text from mapAsset
    string[][] lines;

    private Object[] texSphere;

    int rotPlaneIteration = 0;

    XRSimpleInteractable globalButton = null;


    // Start is called before the first frame update
    public void Start()
    {
        
        // Read txt file to build our button map
        lines = BuildLinesFromText(mapAsset.text);

        // Get the renderer component of Transform we want
        rend = skySphere.GetComponent<Renderer>();
        rend.material.color = new Color(1, 1, 1, 0);

        // Load all the skysphere textures
        texSphere = Resources.LoadAll(resourcesSkysphereImageDirectory, typeof(Texture2D));

        rClick.action.started += delegate { AnnoyingButtonFunction(globalButton); };
        lClick.action.started += delegate { AnnoyingButtonFunction(globalButton); };
    }
    // Removes all whitespace from lines
    string[][] BuildLinesFromText(string text)
    {
        string[] rows = text.Split("\n"[0]);
        string[][] final = new string[rows.Length][];
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = rows[i].Replace(" ", string.Empty);
            var temp = rows[i].Trim().Split(","[0]);
            if (temp[0] == "") temp[0] = "/";
            final[i] = temp;
        }
        /*for (int i = 0; i < final.Length; i++)
        {
            for (int j = 0; j < final[i].Length; j++)
            {
                Debug.Log(final[i][j]);
            }
        }*/
        return final;
    }

    // Returns array of strings representing the line found in text file
    // Function cleaned
    public string[] FindImageInTxt(string imgToFind)
    {
        for (int i = 0; i < lines.Length; i++)
        { 
            if (lines[i][mapAssetImageIndex].Trim().ToUpper() == imgToFind.Trim().ToUpper())
            {
                return lines[i]; // return line that contains information to build new environment
            }
        }
        Debug.LogError("No entry found in file: " + mapAsset.name + " searching by name: " + imgToFind);
        return null;
    }
    public string[] FindItemInTxt(string text, int index)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i][index].Trim().ToUpper() == text.Trim().ToUpper())
            {
                return lines[i]; // return line that contains information to build new environment
            }
        }
        Debug.LogError("No entry found in file: " + mapAsset.name + " searching by name: " + text + " at index: "+ index);
        return null;
    }

    // Returns index of the picture image that is found in our files
    // Function cleaned
    private int FindImgIndex(string imgName, Object[] texSphere)
    {
        imgName = imgName.Trim();
        for (int i = 0; i < texSphere.Length; i++)
        {
            string texName = texSphere[i].name.Trim();
            
            if (imgName.ToUpper() == texName.ToUpper())
                return i;
        }
        Debug.LogError("No texture found in files with name: " + imgName);
        return -1;
    }

    // Deletes all of the buttons in the environment
    // Function cleaned
    public void ClearButtons()
    {
        for (int i = 0; i < rotationPlaneContainer.childCount; i++)
            Destroy(rotationPlaneContainer.GetChild(i).gameObject);
    }
    public void ResetAudio()
    {

    }
    public void DisableDescription()
    {
        DescriptionPanel.gameObject.SetActive(false);
    }

    // Takes in string arrays from mapAsset that contains angles/imgnames and spawns the buttons associated with the angles.
    // Function cleaned
    void SpawnButtons(string[] buttonsLine)
    {
        int numberOfButtons = buttonsLine.Length / 2;
        XRSimpleInteractable[] interactables = new XRSimpleInteractable[numberOfButtons];

        // Extract angles
        for (int i = 0; i < numberOfButtons; i++)
        {
            // Get the angle at even indexes of line and assign it to roty
            float rotationy = float.Parse(buttonsLine[i*2]);

            // Create instance of our rotationPlane prefab
            Transform rotationPlaneClone = Instantiate(rotationPlane);

            // Set this instance's parent to be in a container for organization
            rotationPlaneClone.parent = rotationPlaneContainer;
            // Set position of rotation plane to be bound to parent
            rotationPlaneClone.position =  rotationPlaneContainer.position;
            // With this instance, rotate it to degree specified from txt file
            rotationPlaneClone.transform.Rotate(0, rotationy, 0);
            // Set this instance's name to be 'rotationPlane[integer]'.
            rotationPlaneClone.name = rotationPlane.name + (i + rotPlaneIteration); // rotPlaneIteration is used to ensure that no old names can be reused once a button is deleted. This is primarily used for debugging

            // Set this index of the interactables array to the rotation plane's XRinteractable host gameobject
            interactables[i] = rotationPlaneClone.Find("Interaction").GetComponent<XRSimpleInteractable>();
            
        }
        // Add event listeners to all buttons in the canvas
        for (int i = 0; i < numberOfButtons; i++)
        {
            XRSimpleInteractable identifier = interactables[i];
            
            //interactables[i].selectEntered.AddListener(delegate { ButtonClicked(identifier); });
       
            Animator ArrowAnimator = interactables[i].gameObject.GetComponent<Animator>();
            interactables[i].hoverEntered.AddListener(delegate { RunArrow(ArrowAnimator, true, identifier); } );
            interactables[i].hoverExited.AddListener(delegate { RunArrow(ArrowAnimator, false, identifier); });
        }
        rotPlaneIteration += numberOfButtons;
    }

    // Takes an input rotation (presumably of the button pressed) and tries to find that rotation in the line provided
    // Function cleaned
    private string FindButtonClicked(string[] buttonsLine, Quaternion rotation)
    {
        for (int i = 0; i < buttonsLine.Length; i += 2)
        {
            // Convert our text file angle to quaternion and compare
            if (rotation == Quaternion.Euler(0, float.Parse(buttonsLine[i]), 0))
                // Return the imgname that follows the rotation angle
                return buttonsLine[i+1];
        }
        Debug.LogError("Could not find the angle: Quaternion(" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w + ") in the line provided.");
        return null;
    }

    public string[] StringArraySlice(string[] list, int fromIndex, int toIndex)
    {
        int lengthOfSlicedList = toIndex - fromIndex;
        string[] slicedList = new string[lengthOfSlicedList];
        for (int i = 0; i < lengthOfSlicedList; i++)
        {
            slicedList[i] = list[fromIndex + i];
        }
            

        return slicedList;
    }

    // Called when a listener function is triggered by a button being clicked. Functions similar to OnClick()
    public void ButtonClicked(XRSimpleInteractable button)
    {
        // Get the parent rotation plane of the button that was pressed
        Transform rotationPlane = button.transform.parent.transform;

        // Get name of image that is currently applied to our material
        string imgName = rend.material.GetTexture("_BaseMap").name;
        
        // Eliminate whitespace from imgName
        imgName = imgName.Trim();

        // Find the line in the text file that has this imgName as the host image we are currently in
        string[] line = FindImageInTxt(imgName);
        
        // From the line found, find an angle in the text that matches the one of the button that was clicked and return the image associated
        string[] buttonsLine = StringArraySlice(line, mapAssetButtonsStartIndex, line.Length); // Buttonsline only contains the button information
        string nextImg = FindButtonClicked(buttonsLine, rotationPlane.rotation);
        Debug.Log(nextImg);
        // Eliminate whitespace from the string nextImg
        nextImg = nextImg.Trim();

        // Load the skysphere buttons and texture
        LoadSkysphere(nextImg);
    }

    public void LoadSkysphere(string imgName)
    {
        imgName = imgName.Trim();
        ClearButtons();

        string[] line = FindImageInTxt(imgName);

        string[] buttonsLine = StringArraySlice(line, mapAssetButtonsStartIndex, line.Length);

        string audio = StringArraySlice(line, mapAssetAudioIndex, mapAssetAudioIndex+1)[0];

        string description = StringArraySlice(line, mapAssetDescriptionIndex, mapAssetDescriptionIndex+1)[0];

        FindAndSetTextureOfSkySphere(imgName);
        SpawnButtons(buttonsLine);
        SpawnAudio(audio);
        SpawnDescription(description);
    }

    public void SpawnAudio(string audio)
    {

    }
    public void SpawnDescription(string description)
    {

    }

    public void FindAndSetTextureOfSkySphere(string imgName)
    {
        //Debug.Log(imgName + " idk ");
        imgName = imgName.Trim();
        // Find the index of the image in file
        int imgIndex = FindImgIndex(imgName, texSphere);
        // Set skysphere to texture found with index
        rend.material.SetTexture("_BaseMap", (Texture2D)texSphere[imgIndex]);
    }

    void RunArrow(Animator ani, bool animating, XRSimpleInteractable button)
    {
        if (ani)
        {
            ani.SetBool("Hover", animating);
            if (animating == true)
            {
                globalButton = button;
            }
            
        } 
    }
    void AnnoyingButtonFunction(XRSimpleInteractable button)
    {
        globalClicks += 1;
        Debug.Log("clicked: " + globalClicks);
        button = globalButton;
        if (button)
        {
            button.transform.Find("Collider").GetComponent<MeshCollider>().enabled = false;
            //Debug.Log(button.transform.parent.name + " yeet ");
            ButtonClicked(button);
        }
        
    }


    // Update is called once per frame
    void Update()
    {   
        
    }
}

