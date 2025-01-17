using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerSetup : MonoBehaviour
{
    public GameObject panel;
    public GameObject trashCan;
    public GameObject labelInputField;
    public GameObject massSlider;
    public GameObject trailSlider;
    public GameObject massInputField, posXInputField, posYInputField, velocityXInputField, velocityYInputField;
    public GameObject massTextField, posXTextField, posYTextField, velocityXTextField, velocityYTextField;
    public GameObject colorPicker;
    public List<Color> colors;
    public Gravity grav;

    public void setup(Gravity g, string name) {
        grav = g;
        labelInputField.GetComponent<InputField>().text = name;
        Slider mSlider = massSlider.GetComponent<Slider>();
        mSlider.value = Mathf.Sqrt(g.GetComponent<Rigidbody2D>().mass);
        mSlider.onValueChanged.AddListener(f => g.changeMass(f * f));
        massInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeMass(float.Parse(f)));
        posXInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changePosX(float.Parse(f) / 100));
        posYInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changePosY(float.Parse(f) / 100));
        velocityXInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeVelocityX(float.Parse(f) / 100));
        velocityYInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeVelocityY(float.Parse(f) / 100));
        colorPicker.GetComponent<Dropdown>().onValueChanged.AddListener((i) => g.changeColor(colors[i]));

        Slider tSlider = trailSlider.GetComponent<Slider>();
        tSlider.value = 5;
        tSlider.onValueChanged.AddListener(l => g.changeTrailLength(l));

    }

    public void trash()
    {
        GameManager.Instance.deleteOnClick(trashCan.gameObject.transform.parent.gameObject.transform.parent.gameObject);
    }

    public void Disable()
    {
        foreach(Transform child in panel.transform)
        {
            if (child.gameObject.GetComponent<Button>() != null)
            {
                child.gameObject.GetComponent<Button>().interactable = false;
            }
            if (child.gameObject.GetComponent<InputField>() != null)
            {
                child.gameObject.GetComponent<InputField>().interactable = false;
            }
            if (child.gameObject.GetComponent<Slider>() != null)
            {
                child.gameObject.GetComponent<Slider>().interactable = false;
            }
        }
    }


    public void Enable()
    {
        foreach (Transform child in panel.transform)
        {
            if (child.gameObject.GetComponent<Button>() != null)
            {
                child.gameObject.GetComponent<Button>().interactable = true;
            }
            if (child.gameObject.GetComponent<InputField>() != null)
            {
                child.gameObject.GetComponent<InputField>().interactable = true;
            }
            if (child.gameObject.GetComponent<Slider>() != null)
            {
                child.gameObject.GetComponent<Slider>().interactable = true;
            }
        }
    }


}
