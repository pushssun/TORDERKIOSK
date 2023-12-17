using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickAddItem : MonoBehaviour
{
    public Action CartActive;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _deleteButton;
    private int price;
    // Start is called before the first frame update
    void Start()
    { 
        price = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.Replace(",",""));
        _minusButton.onClick.AddListener(OnClickMinusButton);
        _plusButton.onClick.AddListener(OnClickPlusButton);
        _deleteButton.onClick.AddListener(OnClickDeleteButton);
    }

    private void OnClickMinusButton()
    {
        int count = int.Parse(transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
        count--;
        if (count == 0)
        {
            Destroy(gameObject);
            CartActive?.Invoke();
        }
        else
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count.ToString();
        }

        GameManager.Instance.UpdateText(-1, -price);
    }

    public void OnClickPlusButton()
    {
        int count = int.Parse(transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
        count++;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count.ToString();

        GameManager.Instance.UpdateText(1, price);
    }

    public void OnClickDeleteButton()
    {
        int count = int.Parse(transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
        int price = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.Replace(",", ""));

        GameManager.Instance.UpdateText(-count, -price);

        Destroy(gameObject);
    }
}
