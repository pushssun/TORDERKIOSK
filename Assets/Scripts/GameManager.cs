using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public enum GameStep
{
    None,
    Step1,
    Step2,
    Step3
}
[System.Serializable] //직렬화
public class GameData //웹으로 보낼 정보 
{
    public string member_id; //멤버 아이디
    public string kiosk_category_id; //키오스크 종류
    public string play_date; //플레이 날짜
    public int play_stage; //플레이 단계
    public int play_time; //플레이 시간
    public int is_success; //성공 여부
    public int is_game; //게임인지 아닌지 판단
}

public class GameManager : MonoBehaviour
{
    private List<Menu> _orderMenu = new List<Menu>();
    private List<int> _orderMenuCount = new List<int>();
    private List<int> _orderCount = new List<int>();
    private List<string> _orderTime = new List<string>();
    
    private int _orderTotalCount;
    private int _orderTotalPrice;
    private GameStep _gameStep;
    private int playTime;
    private Stopwatch sw;
    private bool isSuccess;
    private bool _saveData;
    private GameData _gameData;
    private string _sceneNameType;
    [SerializeField] private GameObject _finishUI;
    [SerializeField] private TextMeshProUGUI _playTimeTxt;

    public bool Step3Success;
    public static GameManager Instance;
    public int TotalCount;
    public int TotalPrice;
    public MenuDatabase MenuDatabase;

    [SerializeField] private GameObject TotalCountText;
    [SerializeField] private GameObject TotalPriceText;
    [SerializeField] private GameObject _popup;
    [SerializeField] private Button _payButton;
    [SerializeField] private Button _orderedButton;
    [SerializeField] private Transform _totalItemPoint;
    [SerializeField] private Button _orderButton;

    [SerializeField] private GameObject _orderPanel;
    [SerializeField] private TextMeshProUGUI _lastTime;
    [SerializeField] private TextMeshProUGUI _lastMenu;
    [SerializeField] private TextMeshProUGUI _lastCount;

    [SerializeField] private GameObject _payPanel;
    [SerializeField] private GameObject _payPf;
    [SerializeField] private Transform _paySpawnPoint;

    [SerializeField] private GameObject _successPanel;
    [SerializeField] private GameObject _failPanel;


    private void Awake()
    {
        Instance = this;
        _gameData = new GameData();
    }
    private void Start()
    {
        Application.ExternalCall("unityFunction", _gameData.member_id); //웹의 unityFunction의 데이터 값을 _gameData.member_id로 저장
     
        _gameData.play_date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"); //���۽ð� ����

        _orderButton.onClick.AddListener(OnClickOrderButton);
        _payButton.onClick.AddListener(OnClickPayButton);

        //Game
        string sceneName = SceneManager.GetActiveScene().name;
        _sceneNameType = sceneName.Substring(7, 5);
        
        _gameData.kiosk_category_id = sceneName.Substring (0, 6);
        UnityEngine.Debug.Log(sceneName.Substring(11, 1));
        _gameData.play_stage = int.Parse(sceneName.Substring(11, 1));

        if (_sceneNameType.StartsWith("Prac"))
        {
            _gameData.is_game = 0;
        }
        else if(_sceneNameType.StartsWith("Test"))
        {
            _gameData.is_game = 1;
        }
        UnityEngine.Debug.Log(_gameData.is_game);
        _gameStep = (GameStep)char.GetNumericValue(sceneName[sceneName.Length - 1]);

        sw = new Stopwatch();
        sw.Start();


    }

    private void Update()
    {
        if(_finishUI.activeSelf == true)
        {
            sw.Stop();

            switch (_gameStep)
            {
                case GameStep.Step1:
                    isSuccess = (TotalCount > 0);
                    break;
                case GameStep.Step2:
                    isSuccess = (TotalCount > 1);
                    break;
                case GameStep.Step3:
                    isSuccess = Step3Success;
                    break;
            }


            if (isSuccess)
            {
                _successPanel.SetActive(true);
            }
            else
            {
                _failPanel.SetActive(true);
            }

            _gameData.is_success = Convert.ToInt32(isSuccess); //정수로 값보내기

            if (!_saveData)
            {
                SaveData(); //끝나면 정보 보내기

            }
            
        }

        // �ð� ���
        if (_playTimeTxt != null)
        {
            playTime = (int)sw.ElapsedMilliseconds / 1000;
            int minutes;
            int seconds;

            minutes = playTime / 60;
            seconds = playTime % 60;

            if (minutes > 0)
            {
                _playTimeTxt.text = "소요 시간 : " + minutes.ToString() + "분 " + seconds.ToString() + "초";
            }
            else
            {
                _playTimeTxt.text = "소요 시간 : " + seconds.ToString() + "초";
            }

            _gameData.play_time = playTime;//�ҿ�ð� ����
        }
    }

    private void SaveData()
    {
        _saveData = true;
        //JSON으로 변환
        string jsonData = JsonUtility.ToJson(_gameData);

        string url = "https://003operation.shop/kiosk/insertData";

        StartCoroutine(SendDataToWeb(jsonData, url));
    }

    private IEnumerator SendDataToWeb(string jsonData, string url)
    {
        // UTF8로 변환
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // POST 
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(dataBytes);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json"); //json으로 연결
        www.SetRequestHeader("withCredentials", "true"); //withCredentials = true로 설정


        // 웹에서 정보가 올 때까지 기다림
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError("Failed to send data to the web server: " + www.error);
        }
        else
        {
            UnityEngine.Debug.Log("Data sent successfully!");
            //종료
            SetQuit();
        }
    }


    private void OnClickOrderButton()
    {
        StartCoroutine(PopupRoutine());

        int count = 0;
        for(int i=0;i< _totalItemPoint.childCount;i++)
        {
            foreach(Menu menu in MenuDatabase.Menus){
                if (menu.Name.Equals(_totalItemPoint.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text))
                {
                    _orderMenu.Add(menu);
                    _orderMenuCount.Add(int.Parse(_totalItemPoint.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text));
                    _orderTime.Add(DateTime.Now.ToString("HH:mm:ss:tt"));
                    count++;
                }
                
            }
            Destroy(_totalItemPoint.GetChild(i).gameObject);
        }


        _orderCount.Add(count);
        _orderPanel.SetActive(true);
        UpdateOrderPanel();

        UpdateTotal();
    }

    private IEnumerator PopupRoutine()
    {
        _popup.SetActive(true);
        yield return new WaitForSeconds(1f);
        _popup.SetActive(false);

    }

    private void OnClickPayButton()
    {
        _payPanel.SetActive(true);
        for(int i = 0; i < _paySpawnPoint.childCount; i++)
        {
            Destroy(_paySpawnPoint.GetChild(i).gameObject);
        }
        for(int i=0;i<_orderMenu.Count;i++)
        {
            GameObject pf =  Instantiate(_payPf,_paySpawnPoint);
            pf.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _orderTime[i].ToString();
            pf.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _orderMenu[i].Name;
            pf.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0}개",_orderMenuCount[i]);
            pf.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,###}원",_orderMenu[i].Price);
            pf.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,###}원",_orderMenu[i].Price * _orderMenuCount[i]);
            

        }
    }

    private void UpdateOrderPanel()
    {
        string menu = null;
        string count = null;
        for(int i = 0; i < _orderCount[_orderCount.Count -1]; i++)
        {
            menu += _orderMenu[i].Name + '\n';
            count += string.Format("{0:0}개", _orderMenuCount[i]) +'\n';

        }
        _lastTime.text = _orderTime[_orderTime.Count - 1];
        _lastMenu.text = menu;
        _lastCount.text = count;
    }

    private void UpdateTotal()
    {
        TotalCount += _orderTotalCount;
        TotalPrice += _orderTotalPrice;
        TotalCountText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:0}개", TotalCount);
        TotalPriceText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,###}원", TotalPrice.ToString());

        if (TotalCount > 0)
        {
            _payButton.interactable = true;
            _orderedButton.interactable = true;
        }
    }
    public void UpdateText(int count, int price)
    {
        _orderTotalCount += count;
        _orderTotalPrice += price;
        
    }

    //웹에서 정보 받아오는 함수
    public void ReceiveData(string message)
    {
        _gameData.member_id = message;
        UnityEngine.Debug.Log("Received message from JavaScript: " + message);
    }

    public void SetQuit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}