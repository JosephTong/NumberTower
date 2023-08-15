using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public class RoomController : MonoBehaviour
{
    [SerializeField] private Transform m_LeftPos;
    [SerializeField] private Transform m_MidPos;
    [SerializeField] private Transform m_RightPos;
    [SerializeField] private SpriteRenderer m_BG;
    private NumberTowerRoomData m_Content;
    private bool m_IsFinish = false;
    private GameObject m_ContainedObject;
    private ContainedObjectData m_ContainedObjectData;
    private float m_WinLength = 1;
    private float m_LoseLength = 1;

    private int m_Colume = 0;

    public void Init(NumberTowerRoomData Content, int colume){
        m_Colume = colume;
        m_Content = Content;
        m_BG.color = m_Content.BGColor;
        m_ContainedObject = Instantiate(m_Content.Prefab,m_MidPos);
        m_ContainedObject.transform.localPosition = Vector3.zero;
        string numberText = "";

        switch (m_Content.Operations)
        {
            case NumberTowerOperations.Add:
                numberText = "+";
                break;
            case NumberTowerOperations.Minus:
                numberText = "-";
                break;
            case NumberTowerOperations.Multiply:
                numberText = "X";
                break;
            case NumberTowerOperations.Divide:
                numberText = "\u00F7";
                break;
            default:
                break;
        }

        m_ContainedObjectData = m_ContainedObject.GetComponent<ContainedObjectData>();
        m_ContainedObjectData.NumberText.text = numberText+m_Content.Number.ToString("F0");

        AnimationClip[] clips = m_ContainedObjectData.ImageAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Win":
                    m_WinLength = clip.length;
                    break;
                case "Lose":
                    m_LoseLength = clip.length;
                    break;
                default:
                    break;
            }
        }
    }

    public int GetColume(){
        return m_Colume;
    }

    public void OnClick(){
        // room empty
        if(m_IsFinish)
            return;
        NumberTowerGameManager.GetInstance().SetIsActing();
        m_ContainedObject.transform.position = m_RightPos.position;
        NumberTowerGameManager.GetInstance().GoToRoom(m_Content, m_LeftPos.position);
        

        StartCoroutine(AnimationHandle());
        m_IsFinish = true;
    }

    private IEnumerator AnimationHandle(){ 
        float waitTime = 0;
        if(NumberTowerGameManager.GetInstance().IsPlayerDead()){
            m_ContainedObjectData.ImageAnimator.Play("Win");
        }else{
            m_ContainedObjectData.ImageAnimator.Play("Lose");
        }

        float targetWaitTime = NumberTowerGameManager.GetInstance().IsPlayerDead() ? m_LoseLength : m_WinLength;
        while (waitTime < targetWaitTime)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        Destroy(m_ContainedObject);
    }

}


