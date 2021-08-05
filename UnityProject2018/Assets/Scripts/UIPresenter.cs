using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    public enum UIList
    {
        NodeUI,
        EdgeUI
    }

    public UIElement NodeUIElement;
    public UIElement EdgeUIElement;
    public GameObject Panel;

    private static Dictionary<UIList, UIElement> AvailableElements;

    private static UIPresenter _instance;
    public static UIPresenter Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update 
    void Start()
    {
        AvailableElements = new Dictionary<UIList, UIElement>();
        if(NodeUIElement != null)
        {
            AvailableElements.Add(UIList.NodeUI, NodeUIElement);
        }
        if(EdgeUIElement != null)
        {
            AvailableElements.Add(UIList.EdgeUI, EdgeUIElement);
        }
        if(Panel != null)
        {
            ClosePanel();
        }
    }

    public void NotifyUIUpdate(UIList el)
    {
        UIElement element;
        if(AvailableElements.TryGetValue(el, out element))
        {
            element.UpdateUI();
            OpenPanel(element);
        }
    }

    public void OpenPanel(UIElement element)
    {
        if(element != null) {
            element.gameObject.SetActive(true);
        }

    }
    public void ClosePanel()
    {
        foreach(KeyValuePair<UIList, UIElement> entry in AvailableElements) {
            entry.Value.gameObject.SetActive(false);
        }

    }

}
