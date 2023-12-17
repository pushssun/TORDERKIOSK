using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickCallAddItem : MonoBehaviour
{
    private int price;

    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _deleteButton;
    // Start is called before the first frame update
    void Start()
    {
        price = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        _minusButton.onClick.AddListener(OnClickMinusButton);
        _plusButton.onClick.AddListener(OnClickPlusButton);
        _deleteButton.onClick.AddListener(OnClickDeleteButton);
    }

    private void OnClickMinusButton()
    {
        int count = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        count--;
        if (count == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count.ToString();
        }

    }

    public void OnClickPlusButton()
    {
        int count = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        count++;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();

    }

    public void OnClickDeleteButton()
    {
        int count = int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

        Destroy(gameObject);
    }
}
