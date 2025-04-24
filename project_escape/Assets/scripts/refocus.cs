using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Refocus : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField inputField;

    void Start()
    {

        // Find the the text field with the tag 'textfield'
        GameObject textFieldObject = GameObject.FindWithTag("textfield");
        if (textFieldObject != null)
        {
            inputField = textFieldObject.GetComponent<TMP_InputField>();
            if (inputField == null)
            {
                Debug.LogError("TMP_InputField component not found on the GameObject with tag 'textfield'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with tag 'textfield'.");
        }
    }


    // When the user clicks on the text field, refocus on the input field so they dont have to exit the dialogue box to type again
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
