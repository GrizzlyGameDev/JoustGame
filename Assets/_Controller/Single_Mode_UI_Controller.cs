﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Single_Mode_UI_Controller : MonoBehaviour
{

    #region Variable
    [SerializeField] private GameObject m_UIPanel;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [SerializeField] private TextMeshProUGUI m_TimeDurationText;
    [SerializeField] private TextMeshProUGUI m_WaveText;

    private int m_TimeLeft = 0;
    private int[] m_Wave = {0,0};
    private bool m_CountdownToggle = false;
    #endregion

    #region Getter & Setter
    public GameObject UIPanel
    {
        get
        {
            return m_UIPanel;
        }

        set
        {
            m_UIPanel = value;
        }
    }
    public TextMeshProUGUI ScoreText
    {
        get
        {
            return m_ScoreText;
        }

        set
        {
            m_ScoreText = value;
        }
    }
    public TextMeshProUGUI TimeDurationText
    {
        get
        {
            return m_TimeDurationText;
        }

        set
        {
            m_TimeDurationText = value;
        }
    }
    public TextMeshProUGUI WaveText
    {
        get
        {
            return m_WaveText;
        }

        set
        {
            m_WaveText = value;
        }
    }
    public int TimeLeft
    {
        get
        {
            return m_TimeLeft;
        }

        set
        {
            m_TimeLeft = value;
        }
    }
    public int[] Wave
    {
        get
        {
            return m_Wave;
        }

        set
        {
            m_Wave = value;
        }
    }
    public bool CountdownToggle
    {
        get
        {
            return m_CountdownToggle;
        }

        set
        {
            m_CountdownToggle = value;
        }
    }
    #endregion

    public void OnEnable()
    {
        EventManager.StartListening(E_EventName.Set_Initial, SetWaveValue);
        EventManager.StartListening(E_EventName.Set_Initial, StartCountdown);

        EventManager.StartListening(E_EventName.Resume_Game, ToggleCountdown);
        EventManager.StartListening(E_EventName.Pause_Game, ToggleCountdown);

        EventManager.StartListening(E_EventName.Start_Spawn, ToggleCountdown);
        EventManager.StartListening(E_EventName.Wave_Complete, ToggleCountdown);
        EventManager.StartListening(E_EventName.Wave_Setup, SetWaveValue);
    }

    private void StartCountdown(EventParam obj)
    {
        StopAllCoroutines();
        StartCoroutine("Countdown");
    }

    private void SetWaveValue(EventParam obj)
    {
        try
        {
            Dictionary<E_ValueIdentifer, object> eo = obj.EventObject;

            object currentWaveValue;
            object totalWaveValue;
            object timeLeftValue;
            if (eo.TryGetValue(E_ValueIdentifer.Current_Wave_Int, out currentWaveValue) &&
                eo.TryGetValue(E_ValueIdentifer.Total_Wave_Int, out totalWaveValue) && 
                eo.TryGetValue(E_ValueIdentifer.Time_Left_Int, out timeLeftValue))
            {
                Wave[0] = (int)currentWaveValue;
                Wave[1] = (int)totalWaveValue;
                TimeLeft = (int)timeLeftValue;
                SetUIText();
            }
            else
            {
                EventManager.EventDebugLog("Value does not exist");
            }

            EventManager.FinishEvent(obj.EventName);
        }
        catch (Exception e)
        {
            EventManager.EventDebugLog(e.ToString());
        }
    }

    private void ToggleCountdown(EventParam obj)
    {
        try
        {
            Dictionary<E_ValueIdentifer, object> eo = obj.EventObject;

            object countdownToggle;
            if (eo.TryGetValue(E_ValueIdentifer.Countdown_Toggle_Bool, out countdownToggle))
            {
                CountdownToggle = (bool)countdownToggle;
            }
            else
            {
                EventManager.EventDebugLog("Value does not exist");
            }

            EventManager.FinishEvent(obj.EventName);
        }
        catch (Exception e)
        {
            EventManager.EventDebugLog(e.ToString());
        }
    }

    void SetUIText()
    {
        int minutes = TimeLeft / 60;
        int seconds = TimeLeft % 60;
        TimeDurationText.SetText(String.Format("Time: {0}:{1}", minutes, seconds));
        WaveText.SetText(String.Format("Wave: {0}/{1}", Wave[0], Wave[1]));
    }

    IEnumerator Countdown()
    {
        while (CountdownToggle)
        {
            SetUIText();

            if (TimeLeft == 0)
            {
                EventManager.TriggerEvent(E_EventName.Game_Over);
            }
            yield return new WaitForSeconds(1f);
            TimeLeft--;
        }       
    }

}
