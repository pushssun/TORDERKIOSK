using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnData : MonoBehaviour
{
    [SerializeField] private GameObject _itemPf;
    [SerializeField] private GameObject _addPf;
    [SerializeField] private GameObject _cart;
    [SerializeField] private Button _cartButton;
    [SerializeField] private Button _closeCartButton;
    [SerializeField] private Transform _addSpawnPoint;
    [SerializeField] private Transform[] _spawnPoint;

    private MenuDatabase _database;

    // Start is called before the first frame update
    void Start()
    {
        _database = GameManager.Instance.MenuDatabase;

        _cartButton.GetComponent<Button>().onClick.AddListener(OnClickCart);

        for (int i = 0; i < System.Enum.GetValues(typeof(Type)).Length; i++)
        {
            for(int j = 0; j < _database.Menus.Length; j++)
            {
                if(_database.Menus[j].Type == (Type)i)
                {
                    _itemPf.transform.GetChild(0).GetComponent<Image>().sprite = _database.Menus[j].Image; 
                    _itemPf.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _database.Menus[j].Name; 
                    _itemPf.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,###}¿ø",_database.Menus[j].Price); 
                    GameObject spawnItem = Instantiate(_itemPf, _spawnPoint[i]);
                    int index = j;
                    spawnItem.GetComponent<Button>().onClick.AddListener(() => OnClickMenu(index));
                }
            }
        }
        _closeCartButton.onClick.AddListener(OnClickCloseCart);
    }
    
    private void OnClickCart()
    {
        _cart.SetActive(!_cart.activeSelf);

    }
    public void OnClickCloseCart()
    {
        StartCoroutine(ClickCloseCartRoutine());
    }

    private IEnumerator ClickCloseCartRoutine()
    {
        yield return null;

        _cart.SetActive(false);
        ChangeMenuColumnCount(3);
    }
    private void OnClickMenu(int index)
    {
        _cart.SetActive(true);
        ChangeMenuColumnCount(2);

        OnClickItemButton(index);
    }

    private void OnClickItemButton(int index)
    {
        _addPf.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _database.Menus[index].Name;
        _addPf.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,###}", _database.Menus[index].Price);

        int isSpawn = IsSpawnItem(_addPf);

        if (isSpawn == -1)
        {
            GameManager.Instance.UpdateText(1, _database.Menus[index].Price);
            GameObject addItem = Instantiate(_addPf, _addSpawnPoint);
            addItem.GetComponent<ClickAddItem>().CartActive += OnClickCloseCart; 
        }
        else
        {
            _addSpawnPoint.GetChild(isSpawn).GetComponent<ClickAddItem>().OnClickPlusButton();
        }

    }

    private int IsSpawnItem(GameObject item)
    {
        int index = _addSpawnPoint.childCount;
        for (int i = 0; i < index; i++)
        {
            if (GetMenu(item).Equals(GetMenu(_addSpawnPoint.GetChild(i).gameObject)))
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

    private void ChangeMenuColumnCount(int count)
    {
        for (int i = 0; i < _spawnPoint.Length; i++)
        {
            _spawnPoint[i].GetComponent<GridLayoutGroup>().constraintCount = count;
        }
    }
}
