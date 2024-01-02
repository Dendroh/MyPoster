using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    //A:65 a:97
    InputField targetField;
    [SerializeField] InputField[] inputFieldList;
    [SerializeField] Button[] numButtons;
    [SerializeField] Button[] alphabetButtons; //a~z 97~122

    //int capital = 32;
    int capital = 0;
    // Start is called before the first frame update
    void Start()
    {
        targetField = inputFieldList[5];

        for (int i = 0; i < numButtons.Length; i++)
            numButtons[i].GetComponentInChildren<Text>().text = i.ToString();
        for (int i = 0; i < alphabetButtons.Length; i++)
            alphabetButtons[i].GetComponentInChildren<Text>().text = ((char)(i+97)).ToString();
        
    }
    public void SetInputTarget(int i)
    {
        targetField = inputFieldList[i];
    }
    public void Keydown(int i)
    {
        if (i < 10) { 
            targetField.text += i.ToString();
        }
        else if (i==11) // capital
        {
            capital = capital == 0 ? 32 : 0; 
            for (int j = 0; j < alphabetButtons.Length; j++)
                alphabetButtons[j].GetComponentInChildren<Text>().text = ((char)(j + 97-capital)).ToString();
        }
        else if(i==12) // _
        {
            targetField.text += "_";
        }
        else if(i==13 && targetField.text != "")// esc
        {
            targetField.text = targetField.text.Substring(0, targetField.text.Length - 1);
        }
        else
        {
            targetField.text += (char)(i - capital);
        }
    }
}
