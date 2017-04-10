using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleClickHandler : MonoBehaviour {
    public const int RIGHT_CLICK = 1;

    private bool _pressed;

    private bool _interactive;
    public bool IsInteractive
    {
        get { return _interactive; }
    }

    [SerializeField]
    private Sprite _selectedImage;
    [SerializeField]
    private Sprite _unselectedImage;
    [SerializeField]
    private Sprite _uninteractiveImage;

    public bool Activated
    {
        get { return GetComponent<Collider2D>().enabled && GetComponent<SpriteRenderer>().enabled; }
    }

	// Use this for initialization
	void Start () {
                
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void initialize(bool activated, bool interactive)
    {
        _interactive = interactive || !activated;
        if (_interactive)
            GetComponent<SpriteRenderer>().sprite = _unselectedImage;
        else
            GetComponent<SpriteRenderer>().sprite = _uninteractiveImage;
        activate(activated);
    }

    public void activate(bool activated)
    {
        _pressed = false;
        GetComponent<Collider2D>().enabled = activated;
        GetComponent<SpriteRenderer>().enabled = activated;
        transform.GetChild(0).gameObject.SetActive(!activated);
    }

    public void click()
    {
        if (_interactive && Activated)
        {
            if (_pressed && Input.GetMouseButtonUp(RIGHT_CLICK))
            {
                _pressed = false;
                if (GetComponent<SpriteRenderer>().sprite != _selectedImage)
                {
                    GetComponent<SpriteRenderer>().sprite = _selectedImage;
                }
                else if (BoardController.instance.decrementCoins())
                {
                    GetComponent<SpriteRenderer>().sprite = _unselectedImage;
                    activate(false);
                    // TODO PLAY KACHING SOUND EFFECT
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = _unselectedImage;
                    BoardController.playDeniedSound();
                }
            }
            if (Input.GetMouseButtonDown(RIGHT_CLICK))
            {
                _pressed = true;
            }
        }
    }

    public void unclick()
    {
        if (_interactive && Activated)
        {
            _pressed = false;
            GetComponent<SpriteRenderer>().sprite = _unselectedImage;
        }
    }
}
