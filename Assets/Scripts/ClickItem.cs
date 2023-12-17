using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickItem : MonoBehaviour
{
    private Button _itemButton;

    [SerializeField] private GameObject _addItemPf;
    [SerializeField] private Transform _spawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        _itemButton = GetComponent<Button>();
        _itemButton.onClick.AddListener(OnClickItemButton);


    }

    private void OnClickItemButton()
    {
        _addItemPf.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _itemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        int index = IsSpawnItem(_addItemPf);
        if (index == -1)
        {
            Instantiate(_addItemPf, _spawnPoint);
        }
        else
        {
            _spawnPoint.GetChild(index).GetComponent<ClickCallAddItem>().OnClickPlusButton();
        }

    }

    private int IsSpawnItem(GameObject item)
    {
        int index = _spawnPoint.childCount;
        for (int i = 0; i < index; i++)
        {
            if (GetMenu(item).Equals(GetMenu(_spawnPoint.GetChild(i).gameObject)))
            {
                return i;
            }
        }
        return -1;
    }

    private string GetMenu(GameObject item)
    {
        return item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }


}
