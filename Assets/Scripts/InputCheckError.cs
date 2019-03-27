using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCheckError : MonoBehaviour {
    public Button btnCreate;
    public InputField inputField;
    List<string> rusAbc = new List<string>() 
    { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у",
    "ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я"};


    public void EditInput()
    {
        for (int i = 0; i < inputField.text.Length; i++)
        {
            if (rusAbc.Contains(inputField.text[i].ToString()))
            {
                string withoutLast = inputField.text.Substring(0, (inputField.text.Length - 1));
                inputField.text = withoutLast;
            }
        }
        if(!rusAbc.Contains(inputField.text.ToLower()) && inputField.text != "")
            btnCreate.interactable = true;
        else
            btnCreate.interactable = false;
    }
}
