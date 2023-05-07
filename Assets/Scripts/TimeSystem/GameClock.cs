using System.Xml.Schema;
using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timeText = null;

    [SerializeField]
    private TextMeshProUGUI _dateText = null;

    [SerializeField]
    private TextMeshProUGUI _seasonText = null;

    [SerializeField]
    private TextMeshProUGUI _yearText = null;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(
        int gameYear,
        Season gameSeason,
        int gameDay,
        string gameDayOfWeek,
        int gameHour,
        int gameMinute,
        int gameSecond)
    {
        gameMinute -= gameMinute % 10;

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        _timeText.SetText($"{gameHour} : {gameMinute:00}{((gameHour >= 12) ? "pm" : "am")}");
        _dateText.SetText($"{gameDayOfWeek}. {gameDay}");
        _seasonText.SetText(gameSeason.ToString());
        _yearText.SetText($"Year {gameYear}");
    }
}
