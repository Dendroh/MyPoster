using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProductController : MonoBehaviour
{
    public int id;
    public int count;
    PaymentUIScript script;
    public void init()
    {
        script = GameObject.Find("4.Payment Canvas").GetComponent<PaymentUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.clickProduct(id, count); });
    }
}