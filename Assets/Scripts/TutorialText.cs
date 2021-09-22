using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI main;
    public TextMeshProUGUI upper;
    public TextMeshProUGUI lower;

    public GameObject next;
    public GameObject last;

    public GameObject show;

    public GameObject menu;
    public GameObject menuButton;
    public GameObject hideButton;
    public GameObject bodyValues;
    public GameObject bodyContainer;
    public GameObject timeSlider;
    public GameObject timeText;
    public GameObject cameraToggle;
    public GameObject volumeButton;


    private int clicks;
    private bool showClicked;


    // Start is called before the first frame update
    void Start()
    {
        clicks = 0;
        show.GetComponent<Button>().interactable = false;
        showClicked = false;
        clearText();
        click();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void click()
    {
        if (clicks == 0)
        {
            show.GetComponent<Button>().interactable = false;
            
            clearText();
            main.text = "Welcome to the Astronomical Bodies Tutorial! We are going to go into some of the features that make " +
                "this game more enjoyable! Please use the buttons in the top right to navigate through the tutorial.";
        }
        else if (clicks == 1)
        {
            clearText();
            main.text = "Let's start by pressing the 'Show' button! This will make the interface with all data and astronomical body options appear.";
            show.GetComponent<Button>().interactable = true;
            show.GetComponent<Button>().onClick.AddListener(showPressed);
        }
        else if (clicks == 2)
        {
            if (showClicked)
            {
                clearText();
                main.text = "This is the menu used to interact with what is happening on screen. Let's check out all our options!";
                Disable();
            }
            else
            {
                clicks = 1;
            }
        }
        else if (clicks == 3)
        {
            clearText();
            lower.text = "Right above is all of the astronomical bodies that you can put into the universe. Let's try putting a sun into the world by " + 
                "dragging and dropping the sun icon wherever you would like into the space below.";
            Disable();
            toggleSun(true);

        }

    }

    private void clearText()
    {
        main.text = "";
        upper.text = "";
        lower.text = "";
    }

    public void Disable()
    {

        hideButton.GetComponent<Button>().interactable = false;
        menuButton.GetComponent<Button>().interactable = false;
        foreach (Transform child in bodyContainer.transform)
        {
            if (child.gameObject.GetComponent<DragAndDropCell>().getInteractable() != false)
            {
                child.gameObject.GetComponent<DragAndDropCell>().setInteractable(false);
            }
        }
        foreach (Transform child in bodyValues.transform)
        {
            child.gameObject.GetComponent<UIControllerSetup>().Disable();
        }
    }

    public void Enable()
    {
        hideButton.GetComponent<Button>().interactable = true;
        menuButton.GetComponent<Button>().interactable = true;
        foreach (Transform child in bodyContainer.transform)
        {
            if (child.gameObject.GetComponent<DragAndDropCell>().getInteractable() != false)
            {
                child.gameObject.GetComponent<DragAndDropCell>().setInteractable(true);
            }
        }
        foreach (Transform child in bodyValues.transform)
        {
            child.gameObject.GetComponent<UIControllerSetup>().Enable();
        }
    }

    public void toggleSun(bool value)
    {
        foreach (Transform child in bodyContainer.transform)
        {
            if (child.gameObject.tag.Equals("Sun"))
            {
                child.gameObject.GetComponent<DragAndDropCell>().setInteractable(value);
            }
        }
    }

    public void Next() //Put cap so click++ does not go too far
    {
        clicks++;
        click();
    }


    public void Last() // Add deletion of planets at certain point
    {
        clicks--;
        if (clicks < 0) { clicks = 0; }
        if (clicks < 2) 
        { 
            GameManager.Instance.menuHide();
            showClicked = false;
        }
        click();
    }

    private void showPressed()
    {
        showClicked = true;
        if (clicks != 10) //THIS NEEDS TO BE CHANGED TO MAX CLICK COUNT)
        {
            clicks = 2;
        }
        click();

    }



}
