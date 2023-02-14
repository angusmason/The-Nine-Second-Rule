using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public static float timeLeft;
    public static bool counting = false;
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI countup;
    public static float initialTime = 9;
    [SerializeField] PlayerController playerController;

    void Start()
    {
        timeLeft = initialTime;
        counting = false;
    }

    void Update()
    {
        if (counting)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                countdown.text = timeLeft.ToString("F2");
                countup.text = (9 - timeLeft).ToString("F2");
            }
            else
            {
                timeLeft = 0;
                counting = false;
            }
        }
        else
        {
            if (!playerController.finished)
            {
                countdown.text = initialTime.ToString() + ".00";
                countup.text = "0.00";
            }
        }
    }
}
