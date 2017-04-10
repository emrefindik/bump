using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSquareController : MonoBehaviour {

    private const int REPOSITION_MARKER_LAYER = 14;

    private const float SMALL_DIAGONAL_REPLACEMENT = 0.072f;
    private const float LATERAL_REPLACEMENT = 0.215f;
    private const float LARGE_DIAGONAL_REPLACEMENT = 0.267f;

    [SerializeField]
    private Sprite _largeSprite;
    [SerializeField]
    private Sprite _smallSprite;
    public bool IsSmall
    {
        get { return GetComponent<SpriteRenderer>().sprite == _smallSprite; }
    }

    [SerializeField]
    private Transform _arrow;
    public HashSet<GameObject> InteractiveTrianglesPointed
    {
        get { return _arrow.gameObject.GetComponent<BlockDetector>().InteractiveTriangles; }
    }
    public HashSet<GameObject> FrozenTrianglesPointed
    {
        get { return _arrow.gameObject.GetComponent<BlockDetector>().FrozenTriangles; }
    }
    public HashSet<GameObject> DeactivatedTrianglesPointed
    {
        get { return _arrow.GetChild(0).GetComponent<TraceDetector>().Traces; }
    }
    public HashSet<GameObject> DeactivatedTrianglesIntersected
    {
        get { return _physicsChild.GetChild(0).GetComponent<TraceDetector>().Traces; }
    }  

    [SerializeField]
    private Transform _physicsChild;

    private Vector3 _vector;

    private HashSet<GameObject> _squaresCollided;
    public HashSet<GameObject> SquaresCollided
    {
        get { return _squaresCollided; }
    }
    
    // Use this for initialization
	void Start () {
        _vector = new Vector3(0, 0, 0);
        _squaresCollided = new HashSet<GameObject>();
        initialize();
        _repoMarkers = new HashSet<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (BoardController.instance.RawVerticalMovement != 0.0f || BoardController.instance.RawHorizontalMovement != 0.0f)
        {
            _vector.x = 0;
            _vector.y = 0;
            if (BoardController.instance.RawVerticalMovement != 0.0f)
                _vector.z = -45 - ((BoardController.instance.RawVerticalMovement * 45) * (BoardController.instance.RawHorizontalMovement + 2));
            else
                _vector.z = -45 - ((BoardController.instance.RawHorizontalMovement + 1) * 90);
            _arrow.eulerAngles = _vector;

            if (((int)_vector.z) % 90 == 0)
            {
                if (IsSmall)
                {
                    _vector.x = SMALL_DIAGONAL_REPLACEMENT * BoardController.instance.RawHorizontalMovement;
                    _vector.y = SMALL_DIAGONAL_REPLACEMENT * BoardController.instance.RawVerticalMovement;
                }
                else if (GetComponent<SpriteRenderer>().sprite == _largeSprite)
                {
                    _vector.x = LARGE_DIAGONAL_REPLACEMENT * BoardController.instance.RawHorizontalMovement;
                    _vector.y = LARGE_DIAGONAL_REPLACEMENT * BoardController.instance.RawVerticalMovement;
                }
            }
            else
            {
                _vector.x = LATERAL_REPLACEMENT * BoardController.instance.RawHorizontalMovement;
                _vector.y = LATERAL_REPLACEMENT * BoardController.instance.RawVerticalMovement;
            }
            _vector.z = 0;
            _arrow.localPosition = _vector;
        }
	}

    public void initialize()
    {
        _vector.x = SMALL_DIAGONAL_REPLACEMENT;
        _vector.y = -SMALL_DIAGONAL_REPLACEMENT;
        _vector.z = 0;
        _arrow.localPosition = _vector;
        _vector.x = 0;
        _vector.y = 0;
        _vector.z = 90;
        _arrow.eulerAngles = _vector;
        _squaresCollided.Clear();
        setToSmall();
    }

    public void setToSmall()
    {
        GetComponent<SpriteRenderer>().sprite = _smallSprite;

        _vector.x = 0;
        _vector.y = 0;
        _vector.z = 45;
        _physicsChild.localEulerAngles = _vector;

        _vector.x = BoardController.SQRT1_2;
        _vector.y = BoardController.SQRT1_2;
        _vector.z = 1;
        _physicsChild.localScale = _vector;

        if (((int)_arrow.transform.eulerAngles.z) % 90 == 0)
        {
            _vector.x = Mathf.Clamp(_arrow.transform.localPosition.x, -SMALL_DIAGONAL_REPLACEMENT, SMALL_DIAGONAL_REPLACEMENT);
            _vector.y = Mathf.Clamp(_arrow.transform.localPosition.y, -SMALL_DIAGONAL_REPLACEMENT, SMALL_DIAGONAL_REPLACEMENT);
            _vector.z = 0;
            _arrow.transform.localPosition = _vector;
        }
    }

    public void setToLarge()
    {
        GetComponent<SpriteRenderer>().sprite = _largeSprite;

        _vector.x = 0;
        _vector.y = 0;
        _vector.z = 0;
        _physicsChild.localEulerAngles = _vector;

        _vector.x = 1;
        _vector.y = 1;
        _vector.z = 1;
        _physicsChild.localScale = _vector;

        if (((int)_arrow.transform.eulerAngles.z) % 90 == 0)
        {
            _vector.x = Mathf.Clamp(_arrow.transform.localPosition.x, -SMALL_DIAGONAL_REPLACEMENT, SMALL_DIAGONAL_REPLACEMENT) * (LARGE_DIAGONAL_REPLACEMENT / SMALL_DIAGONAL_REPLACEMENT);
            _vector.y = Mathf.Clamp(_arrow.transform.localPosition.y, -SMALL_DIAGONAL_REPLACEMENT, SMALL_DIAGONAL_REPLACEMENT) * (LARGE_DIAGONAL_REPLACEMENT / SMALL_DIAGONAL_REPLACEMENT);
            _vector.z = 0;
            _arrow.transform.localPosition = _vector;
        }
    }

    void OnMouseUpAsButton()
    {
        BoardController.instance.updateSelectedSquare(gameObject);
    }


    private HashSet<GameObject> _repoMarkers;
    public HashSet<GameObject> RepoMarkers
    {
        get { return _repoMarkers; }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == REPOSITION_MARKER_LAYER)
        {
            _repoMarkers.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == REPOSITION_MARKER_LAYER)
        {
            _repoMarkers.Remove(collision.gameObject);
        }
    }
}
