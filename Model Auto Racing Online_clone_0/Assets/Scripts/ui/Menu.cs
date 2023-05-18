using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    [SerializeField] MenuType _type;

    [SerializeField] 
    protected MenuManager _menuManager;

    private Canvas _canvas;

    public MenuType Type => _type;

    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();

        if (!_menuManager)
        {
            Debug.LogWarning($"MenuManager References not set in {gameObject.name}");
            return;
        }

        _menuManager.AddMenu(this);
    }

    public virtual void SetEnable(int value)
    {
        _canvas.enabled = true;
        _canvas.sortingOrder=value;
    }

    public virtual void SetDisable()
    {
            DisableCanvas();
            return;

    }

    public void DisableCanvas()
    {
        _canvas.enabled = false;
        _canvas.sortingOrder = -1;
    }

    protected void OnButtonPressed(Button button, UnityAction buttonListener)
    {
        if (!button)
        {
            Debug.LogWarning($"There is a 'Button' that is not attached to the '{gameObject.name}' script,  but a script is trying to access it.");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(buttonListener);
    }
}