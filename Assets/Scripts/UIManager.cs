using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField]
    private Image equippedWeapon;

    [SerializeField]
    private TextMeshProUGUI TM_points;

    [SerializeField]
    private GameObject _stageClearedText;
    [SerializeField]
    private GameObject _winText;


    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicati
            return;
        }

        Instance = this;

        hideStageClearedText();
        hideWinText();
    }


    void Start()
    {
        GameManager.Instance?.UpdatePoints(0);
    }

    public void updatePoints(int totalPoint)
    {
        TM_points.text = totalPoint.ToString();
    }


    public void updateEquipedWeapon(Sprite weaponSprite)
    {
        equippedWeapon.sprite = weaponSprite;
    }


    public void showStageClearedText()
    {
        _stageClearedText.SetActive(true);
    }



    public void showWinText()
    {
        _winText.SetActive(true);
    }


    public void hideStageClearedText()
    {
        _stageClearedText.SetActive(false);
    }



    public void hideWinText()
    {
        _winText.SetActive(false);
    }


}

