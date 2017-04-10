using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareClickHandler : MonoBehaviour
{

    private int _index;
    public int Index
    {
        get { return _index; }
    }

    private bool _mainSquareInside;
    public bool MainSquareInside
    {
        get { return _mainSquareInside; }
    }

    public int TriangleCount
    {
        get
        {
            int count = 0;
            if (transform.GetChild(0).GetComponent<TriangleClickHandler>().Activated)
                count++;
            if (transform.GetChild(1).GetComponent<TriangleClickHandler>().Activated)
                count++;
            return count;
        }
    }

    private Vector3 _vector;

    // Use this for initialization
    void Start()
    {
        _mainSquareInside = false;
        _vector = new Vector3();
        _vector.x = ((BoardController.COLUMNS - 1) * -0.5f) + (_index % BoardController.COLUMNS);
        _vector.y = ((BoardController.ROWS - 1) * 0.5f) - (_index / BoardController.COLUMNS);
        _vector.z = -1.0f;
        transform.position = _vector;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUpAsButton()
    {
        if (transform.GetChild(0).GetComponent<TriangleClickHandler>().IsInteractive &&
            transform.GetChild(1).GetComponent<TriangleClickHandler>().IsInteractive)
            BoardController.instance.updateSelectedSquare(gameObject);
        else
            BoardController.instance.updateSelectedSquare(null);
    }

    void OnMouseOver()
    {
        Vector3 block0Corner = FindObjectOfType<Camera>().WorldToScreenPoint(gameObject.transform.GetChild(2).position);
        Vector3 block1Corner = FindObjectOfType<Camera>().WorldToScreenPoint(gameObject.transform.GetChild(3).position);
        Vector2 mousePosition = Input.mousePosition;
        Vector2 displacement0 = new Vector2(block0Corner.x - Input.mousePosition.x, block0Corner.y - Input.mousePosition.y);
        Vector2 displacement1 = new Vector2(block1Corner.x - Input.mousePosition.x, block1Corner.y - Input.mousePosition.y);
        if (displacement0.sqrMagnitude > displacement1.sqrMagnitude)
        {
            transform.GetChild(1).GetComponent<TriangleClickHandler>().click();
            transform.GetChild(0).GetComponent<TriangleClickHandler>().unclick();
        }
        else
        {
            transform.GetChild(0).GetComponent<TriangleClickHandler>().click();
            transform.GetChild(1).GetComponent<TriangleClickHandler>().unclick();
        }
    }

    void OnMouseExit()
    {
        transform.GetChild(0).GetComponent<TriangleClickHandler>().unclick();
        transform.GetChild(1).GetComponent<TriangleClickHandler>().unclick();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.transform.parent == BoardController.instance.MainSquare.transform)
        {
            _mainSquareInside = true;
            BoardController.instance.MainSquare.GetComponent<MainSquareController>().SquaresCollided.Add(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.transform.parent == BoardController.instance.MainSquare.transform)
        {
            _mainSquareInside = false;
            BoardController.instance.MainSquare.GetComponent<MainSquareController>().SquaresCollided.Remove(gameObject);
        }
    }

    public void initialize(bool squareRotated, bool block1activated, bool block1interactive, bool block2activated, bool block2interactive)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<TriangleClickHandler>().initialize(block1activated, block1interactive);
        transform.GetChild(1).GetComponent<TriangleClickHandler>().initialize(block2activated, block2interactive);
        _vector.x = 0.0f;
        _vector.y = 0.0f;
        if (squareRotated)
            _vector.z = -90.0f;
        else
            _vector.z = 0.0f;
        transform.eulerAngles = _vector;
    }

    public void setIndex()
    {
        _index = BoardController.instance.NextSquareIndex;
    }
}
