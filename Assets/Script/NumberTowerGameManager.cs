using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberTowerGameManager : MonoBehaviour
{
    private static NumberTowerGameManager Instance = null;

    private float m_PlayerHP = 1f;
    [SerializeField] private List<LevelScriptable> m_AllLevels = new List<LevelScriptable>();

    [SerializeField] private Transform m_MainRoomCenterPos;
    [SerializeField] private Transform m_MainRoom;
    [SerializeField] private Transform m_RoomParent;
    [SerializeField] private ContainedObjectData m_Player;
    private GameObject m_RoomPrefab;
    private int m_CurrentColume = 0;
    private int m_CurrentRoomBreak = 0;
    private float m_PlayerIdleLength = 1;
    private float m_PlayerWinLength = 1;
    private float m_PlayerLoseLength = 1;
    private List<int> m_RoomsCountInColume = new List<int>();
    private Dictionary<NumberTowerPrefab,GameObject> m_AllRoomContent = new Dictionary<NumberTowerPrefab, GameObject>();

    private int m_LevelIndex = 0;
    private bool m_CanAct = true;

    [Header("MainMenu")]
    [SerializeField] private GameObject m_MainMenu;
    [SerializeField] private Button m_StartGameBtn;
    [SerializeField] private Button m_ExitBtn;


    [Header("LevelSelect")]
    [SerializeField] private GameObject m_LevelSelect;
    [SerializeField] private Transform m_LevelGrid;
    [SerializeField] private Button m_LevelSelectBackBtn;
    private GameObject m_LevelBtnPrefab;

    [Header("Lose")]
    [SerializeField] private GameObject m_LosePanel;
    [SerializeField] private Button m_LoseRetryBtn;
    [SerializeField] private Button m_LoseToMainMenuBtn;

    [Header("Win")]
    [SerializeField] private GameObject m_WinPanel;
    [SerializeField] private Button m_WinNextBtn;
    [SerializeField] private Button m_WinToMainMenuBtn;

    public static NumberTowerGameManager GetInstance()
    {
        return Instance;
    }


    void Awake()
    {
        if (NumberTowerGameManager.GetInstance() != null && NumberTowerGameManager.GetInstance() != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        m_RoomPrefab = Resources.Load<GameObject>("Prefab/Room");
        m_LevelBtnPrefab = Resources.Load<GameObject>("Prefab/LevelBtn");
        m_AllRoomContent.Add( NumberTowerPrefab.WhitePosion, Resources.Load<GameObject>("Prefab/WhitePosion"));
        m_AllRoomContent.Add( NumberTowerPrefab.Death, Resources.Load<GameObject>("Prefab/Death"));
        m_AllRoomContent.Add( NumberTowerPrefab.PurplePosion, Resources.Load<GameObject>("Prefab/PurplePosion"));

        OnClickBackFromLevelSelect();
        m_StartGameBtn.onClick.AddListener(OnClickStartGame);
        m_ExitBtn.onClick.AddListener(OnClickExitGame);
        m_LevelSelectBackBtn.onClick.AddListener(OnClickBackFromLevelSelect);
        m_LoseToMainMenuBtn.onClick.AddListener(OnClickBackFromLevelSelect);
        m_WinToMainMenuBtn.onClick.AddListener(OnClickBackFromLevelSelect);
        m_LoseRetryBtn.onClick.AddListener(()=>OnClickLevelBtn(m_LevelIndex));
        m_WinNextBtn.onClick.AddListener(()=>OnClickLevelBtn(m_LevelIndex+1));

        for (int i = 0; i < m_AllLevels.Count; i++)
        {
            var newLevelBtn = Instantiate(m_LevelBtnPrefab, m_LevelGrid);
            int index = i;
            newLevelBtn.GetComponent<LevelBtn>().Button.onClick.AddListener(() => OnClickLevelBtn(index));
            newLevelBtn.GetComponent<LevelBtn>().LevelText.text = (index + 1).ToString();
        }
        AnimationClip[] clips = m_Player.ImageAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Idle":
                    m_PlayerIdleLength = clip.length;
                    break;
                case "Win":
                    m_PlayerWinLength = clip.length;
                    break;
                case "Lose":
                    m_PlayerLoseLength = clip.length;
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_CanAct)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform != null)
            {
                hit.transform.TryGetComponent<RoomController>(out var room);

                if (room != null)
                {

                    if (room.GetColume() == m_CurrentColume)
                    {
                        // click room
                        room.OnClick();
                    }

                }

            }

        }
    }

    public void ClearAllLevel(){
        m_AllLevels.Clear();
    }

    public void AddLevelScriptable(LevelScriptable levelScriptable){
        m_AllLevels.Add(levelScriptable);
    }

    private void TurnOffAllPanel(){
        m_MainMenu.SetActive(false);
        m_LevelSelect.SetActive(false);
        m_LosePanel.SetActive(false);
        m_WinPanel.SetActive(false);
    }

    private IEnumerator BackToMainRoom()
    {
        float waitTime = 0;
        float targetWaitTime = IsPlayerDead() ? m_PlayerLoseLength : m_PlayerWinLength;
        while (waitTime < targetWaitTime)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        m_Player.NumberText.text = m_PlayerHP.ToString("0.#");
        waitTime = 0;
        while (waitTime < 0.5f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        if(!IsPlayerDead()){
            m_CurrentRoomBreak++;
            if(m_CurrentRoomBreak >= m_RoomsCountInColume[m_CurrentColume]){
                // colume complete
                m_CurrentRoomBreak = 0;
                m_CurrentColume++;
                m_MainRoom.position += new Vector3(7,0,0);

                if(m_CurrentColume>=m_RoomsCountInColume.Count){
                    // Level Complete
                    m_WinPanel.SetActive(true);
                    m_WinNextBtn.gameObject.SetActive(m_LevelIndex+1 < m_AllLevels.Count);
                }
            }
            m_Player.transform.position = m_MainRoomCenterPos.position;
            m_CanAct = true;
            
        }else{
            // player dead
            m_LosePanel.SetActive(true);
        }
    }
    private void OnClickBackFromLevelSelect()
    {
        TurnOffAllPanel();
        m_MainMenu.SetActive(true);
    }

    private void OnClickStartGame()
    {
        TurnOffAllPanel();
        m_LevelSelect.SetActive(true);
    }

    private void OnClickExitGame()
    {
        Application.Quit();
    }

    private void OnClickLevelBtn(int index)
    {
        if(index >= m_AllLevels.Count){
            // max level reached
            return;
        }
        TurnOffAllPanel();
        m_CurrentColume = 0;
        m_CurrentRoomBreak = 0;
        m_RoomsCountInColume.Clear();
        for (int i = 0; i < m_RoomParent.childCount; i++)
        {
            Destroy(m_RoomParent.GetChild(i).gameObject);  
        }
        m_MainRoom.position = m_RoomParent.position - new Vector3(7,0,0);
        m_Player.transform.position = m_MainRoomCenterPos.position;
        m_CanAct = true;
        m_Player.ImageAnimator.Play("Idle");
        m_LevelIndex = index;

        LevelScriptable level = m_AllLevels[index];
        m_PlayerHP = level.PlayerHp;
        m_Player.NumberText.text = m_PlayerHP.ToString("0.#");
        for (int colume = 0; colume < level.Columes.Count; colume++)
        {
            m_RoomsCountInColume.Add(level.Columes[colume].Rooms.Count);
            for (int room = 0; room < level.Columes[colume].Rooms.Count; room++)
            {
                SpawnRoom(level.Columes[colume].Rooms[room], colume, room);
            }
        }

    }

    private void SpawnRoom(NumberTowerRoomData roomData, int colume, int room)
    {
        Transform newRoom = Instantiate(m_RoomPrefab, m_RoomParent).transform;
        newRoom.position = new Vector3((colume + 1) * 7, room * 4, 0);
        newRoom.GetComponent<RoomController>().Init(roomData, colume, m_AllRoomContent[roomData.Prefab]);
    }

    public void SetIsActing(){
        m_CanAct = false;
    }

    public void GoToRoom(NumberTowerRoomData content, Vector3 playerNewPos)
    {
        m_Player.transform.position = playerNewPos;


        switch (content.Operations)
        {
            case NumberTowerOperations.Enemy:
                if (m_PlayerHP > content.Number)
                {
                    // win
                    m_PlayerHP += content.Number;
                    m_Player.ImageAnimator.Play("Win");
                }
                else
                {
                    // lose
                    m_PlayerHP -= content.Number;
                    m_Player.ImageAnimator.Play("Lose");
                }
                break;
            case NumberTowerOperations.Add:
                m_PlayerHP += content.Number;
                m_Player.ImageAnimator.Play("Win");
                break;
            case NumberTowerOperations.Minus:
                if (m_PlayerHP > content.Number)
                {
                    m_PlayerHP -= content.Number;
                    m_Player.ImageAnimator.Play("Win");
                }
                else
                {
                    // lose
                    m_PlayerHP -= content.Number;
                    m_Player.ImageAnimator.Play("Lose");
                }
                break;
            case NumberTowerOperations.Multiply:
                m_PlayerHP *= content.Number;
                m_Player.ImageAnimator.Play("Win");

                break;
            case NumberTowerOperations.Divide:
                m_PlayerHP /= content.Number;
                m_Player.ImageAnimator.Play("Win");

                break;
            default:
                break;
        }

        StartCoroutine(BackToMainRoom());

    }


    public bool IsPlayerDead()
    {
        return m_PlayerHP <= 0;
    }
}
