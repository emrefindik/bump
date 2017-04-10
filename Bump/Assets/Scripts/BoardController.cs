using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardController : MonoBehaviour {

    public const bool T = true;
    public const bool F = false;

    public static BoardController instance;

    public const int ROWS = 8;
    public const int COLUMNS = 8;
    public const int LEVEL_COUNT = 1;
    public const int NO_LEVEL = -1;
    public const int SQUARE_ROTATED_INDEX = 0;
    public const int BLOCK1_ACTIVATED_INDEX = 1;
    public const int BLOCK1_INTERACTIVE_INDEX = 2;
    public const int BLOCK2_ACTIVATED_INDEX = 3;
    public const int BLOCK2_INTERACTIVE_INDEX = 4;
    public const KeyCode RESET_LEVEL = KeyCode.R;
    public const KeyCode MOVE_UP = KeyCode.W;
    public const KeyCode MOVE_DOWN = KeyCode.S;
    public const KeyCode MOVE_LEFT = KeyCode.A;
    public const KeyCode MOVE_RIGHT = KeyCode.D;
    public const KeyCode ROTATE_CLOCKWISE = KeyCode.D;
    public const KeyCode ROTATE_COUNTERCLOCKWISE = KeyCode.A;
    public const KeyCode SWITCH_SIZE = KeyCode.Space;
    public const KeyCode NEXT_LEVEL = KeyCode.Space;

    public static readonly bool[][][] LEVELS = new bool[][][] {
        new bool[][]{
                                        new bool[]{T,T,F,T,F},      new bool[]{F,T,F,T,F},      new bool[]{T,T,T,T,T},      new bool[]{F,T,T,T,T},      new bool[]{T,F,F,T,F},      new bool[]{F,F,F,T,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,F,F},      new bool[]{T,F,F,T,T},      new bool[]{F,F,F,T,T},      new bool[]{T,T,F,F,F},      new bool[]{F,T,F,F,F},      new bool[]{T,T,T,F,F},      new bool[]{F,T,T,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,F,F,T,T},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},
            new bool[]{F,T,T,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F},      new bool[]{F,F,F,F,F}
        }
    };

    public const float MAXIMUM_MOVEMENT_AMOUNT = 2f;
    public const float MINIMUM_MOVEMENT_AMOUNT = 0.01f;
    public const float VELOCITY_RAMP_UP = 1.5f;
    public const float SQRT1_2 = 0.70710678118f;
    private Vector3 CLOCKWISE_ROTATION = new Vector3(0, 0, -90);
    private Vector3 COUNTERCLOCKWISE_ROTATION = new Vector3(0, 0, 90);

    [SerializeField]
    private GameObject _mainSquare;
    public GameObject MainSquare
    { get { return _mainSquare; } }

    [SerializeField]
    private GameObject _squarePrefab;
    [SerializeField]
    private GameObject _repositionMarkerPrefab;

    [SerializeField]
    private int _coins;

    [SerializeField]
    private Transform _startText;
    [SerializeField]
    private Transform _finishText;
    [SerializeField]
    private GameObject _nextLevelPrompt;
    [SerializeField]
    private GameObject _restartPrompt;

    private GameObject _selectedSquare;

    private Vector2 _rawVelocity;
    private float _velocityCoeff;
    private Vector2 _actualVelocity;
    public float RawVerticalMovement
    {
        get { return _rawVelocity.y; }
    }
    public float RawHorizontalMovement
    {
        get { return _rawVelocity.x; }
    }

    private GameObject[] _squares;
    private Vector2 _position;

    private int _nextSquareIndex;
    public int NextSquareIndex
    {
        get
        {
            _nextSquareIndex++;
            return _nextSquareIndex - 1;
        }
    }

    private int _currentLevel;
    private int _levelLoaded;
    private bool _won;

    // Use this for initialization
    void Start () {
        instance = this;
        _selectedSquare = _mainSquare;
        _rawVelocity = new Vector2(0.0f, 0.0f);
        _actualVelocity = new Vector2(0.0f, 0.0f);
        _position = new Vector2();
        _nextSquareIndex = 0;
        float x;
        float y;
        Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);
        for (x = -(COLUMNS - 1) / 2.0f; x < COLUMNS / 2.0f; x += 1.0f)
        {
            for (y = -(ROWS - 1) / 2.0f; y < ROWS / 2.0f; y += 1.0f)
            {
                GameObject repositionMarker = Instantiate(_repositionMarkerPrefab);
                temp.x = x;
                temp.y = y;
                temp.z = 0;
                repositionMarker.transform.position = temp;
                temp.x = 0;
                temp.y = 0;
                temp.z = 45;
                repositionMarker.transform.eulerAngles = temp;
            }
        }
        for (x = -(COLUMNS / 2.0f) + 1; x < (COLUMNS - 1) / 2.0f; x += 1.0f)
        {
            for (y = -(ROWS / 2.0f) + 1; y < (ROWS - 1) / 2.0f; y += 1.0f)
            {
                GameObject repositionMarker = Instantiate(_repositionMarkerPrefab);
                temp.x = x;
                temp.y = y;
                temp.z = 0;
                repositionMarker.transform.position = temp;
                temp.x = 0;
                temp.y = 0;
                temp.z = 45;
                repositionMarker.transform.eulerAngles = temp;
            }
        }
        _squares = new GameObject[ROWS * COLUMNS];
        _velocityCoeff = MINIMUM_MOVEMENT_AMOUNT;
        while (_nextSquareIndex < ROWS * COLUMNS)
        {
            GameObject square = Instantiate(_squarePrefab);
            _squares[_nextSquareIndex] = square;
            square.GetComponent<SquareClickHandler>().setIndex(); // increments _nextSquareIndex            
        }
        _restartPrompt.SetActive(false);
        _nextLevelPrompt.SetActive(false);
        _currentLevel = 0;
        _levelLoaded = NO_LEVEL;
        _won = false;
	}
	
	// Update is called once per frame
	void Update () {
        _rawVelocity.y = 0.0f;
        _rawVelocity.x = 0.0f;
        if (!_won && _mainSquare.GetComponent<MainSquareController>().SquaresCollided.Count == 1 &&
            _mainSquare.GetComponent<MainSquareController>().SquaresCollided.Contains(_squares[ROWS * COLUMNS - 1]))
        {
            _won = true;
            StartCoroutine(win());
        }
        if (_won)
        {
            if (_nextLevelPrompt.activeInHierarchy && Input.GetKeyDown(NEXT_LEVEL))
            {
                _currentLevel++;
                _nextLevelPrompt.SetActive(false);
                _won = false;
            }
            else if (_restartPrompt.activeInHierarchy && Input.GetKeyDown(NEXT_LEVEL))
            {
                _levelLoaded = NO_LEVEL;
                _currentLevel = 0;
                _restartPrompt.SetActive(false);
                _won = false;
            }
        }
        if (!_won)
        {
            if (_levelLoaded != _currentLevel || Input.GetKeyDown(RESET_LEVEL))
                loadLevel();
            if (_selectedSquare == _mainSquare)
            {
                if (Input.GetKey(MOVE_LEFT))
                    _rawVelocity.x -= 1.0f;
                if (Input.GetKey(MOVE_RIGHT))
                    _rawVelocity.x += 1.0f;
                if (Input.GetKey(MOVE_DOWN))
                    _rawVelocity.y -= 1.0f;
                if (Input.GetKey(MOVE_UP))
                    _rawVelocity.y += 1.0f;
            }
            else
            {
                if (Input.GetKeyDown(ROTATE_COUNTERCLOCKWISE) && !_selectedSquare.GetComponent<SquareClickHandler>().MainSquareInside)
                    _selectedSquare.transform.Rotate(COUNTERCLOCKWISE_ROTATION);
                if (Input.GetKeyDown(ROTATE_CLOCKWISE) && !_selectedSquare.GetComponent<SquareClickHandler>().MainSquareInside)
                    _selectedSquare.transform.Rotate(CLOCKWISE_ROTATION);
            }
            if (Input.GetKeyDown(SWITCH_SIZE))
            {
                trySwitchSize();
            }
        }
        if (_rawVelocity.x == 0.0f && _rawVelocity.y == 0.0f)
            _velocityCoeff = MINIMUM_MOVEMENT_AMOUNT;
    }

    void FixedUpdate()
    {
        _actualVelocity.x = _rawVelocity.x * _velocityCoeff;
        _actualVelocity.y = _rawVelocity.y * _velocityCoeff;
        if (_rawVelocity.y != 0.0f && _rawVelocity.x != 0.0f)
        {
            _actualVelocity.x *= SQRT1_2;
            _actualVelocity.y *= SQRT1_2;
        }
        if (_selectedSquare == _mainSquare)
        {
            _mainSquare.GetComponent<Rigidbody2D>().velocity = _actualVelocity;
        }
        _velocityCoeff *= (VELOCITY_RAMP_UP * (MAXIMUM_MOVEMENT_AMOUNT - _velocityCoeff) + _velocityCoeff) / MAXIMUM_MOVEMENT_AMOUNT;
        if (_velocityCoeff > MAXIMUM_MOVEMENT_AMOUNT)
            _velocityCoeff = MAXIMUM_MOVEMENT_AMOUNT;
    }

    public void updateSelectedSquare(GameObject square)
    {
        if (_selectedSquare != _mainSquare)
            _selectedSquare.GetComponent<SpriteRenderer>().enabled = false;

        if (square != null)
            _selectedSquare = square;
        else
            _selectedSquare = _mainSquare;

        if (_selectedSquare != _mainSquare)
            _selectedSquare.GetComponent<SpriteRenderer>().enabled = true;
    }

    public bool decrementCoins()
    {
        if (_coins > 0)
        {
            _coins--;
            return true;
        }
        return false;
    }

    public void trySwitchSize()
    {
        if (_mainSquare.GetComponent<MainSquareController>().IsSmall)
        {
            if (_mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.Count == 1)
            {
                if (_mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First().transform.parent.GetComponent<SquareClickHandler>().TriangleCount < 2)
                {
                    _position.x = _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First().transform.parent.position.x;
                    _position.y = _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First().transform.parent.position.y;
                    _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First().GetComponent<TriangleClickHandler>().activate(false);
                    _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.Remove(_mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First());
                    _mainSquare.GetComponent<Rigidbody2D>().MovePosition(_position);
                    _mainSquare.GetComponent<MainSquareController>().setToLarge();
                }
                else {
                    foreach (GameObject square in _mainSquare.GetComponent<MainSquareController>().SquaresCollided)
                    {
                        if (square.GetComponent<SquareClickHandler>().TriangleCount == 0)
                        {
                            _position.x = square.transform.position.x;
                            _position.y = square.transform.position.y;
                            _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First().GetComponent<TriangleClickHandler>().activate(false);
                            _mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.Remove(_mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.First());
                            _mainSquare.GetComponent<Rigidbody2D>().MovePosition(_position);
                            _mainSquare.GetComponent<MainSquareController>().setToLarge();
                            break;
                        }
                    }
                }
            }
            if (_mainSquare.GetComponent<MainSquareController>().IsSmall)
                playDeniedSound();
            else
                AudioManager.instance.playBite();
        }
        else
        {            
            if (_mainSquare.GetComponent<MainSquareController>().InteractiveTrianglesPointed.Count == 0 && _mainSquare.GetComponent<MainSquareController>().FrozenTrianglesPointed.Count == 0)
            {
                foreach (GameObject deactivatedTriangle in _mainSquare.GetComponent<MainSquareController>().DeactivatedTrianglesPointed)
                {
                    if (!_mainSquare.GetComponent<MainSquareController>().DeactivatedTrianglesIntersected.Contains(deactivatedTriangle))
                    {                        
                        _mainSquare.GetComponent<MainSquareController>().DeactivatedTrianglesPointed.Remove(deactivatedTriangle);
                        deactivatedTriangle.transform.parent.GetComponent<TriangleClickHandler>().activate(true);
                        _mainSquare.GetComponent<MainSquareController>().setToSmall();
                        break;
                    }
                    else
                    {
                        foreach (GameObject repoMarker in _mainSquare.GetComponent<MainSquareController>().RepoMarkers)
                        {
                            if (!repoMarker.GetComponent<RepositionMarkerHandler>().Traces.Contains(deactivatedTriangle) &&
                                repoMarker.GetComponent<RepositionMarkerHandler>().Triangles.Count == 0)
                            {
                                _mainSquare.GetComponent<MainSquareController>().DeactivatedTrianglesPointed.Remove(deactivatedTriangle);
                                _position.x = repoMarker.transform.position.x;
                                _position.y = repoMarker.transform.position.y;
                                _mainSquare.GetComponent<Rigidbody2D>().MovePosition(_position);
                                deactivatedTriangle.transform.parent.GetComponent<TriangleClickHandler>().activate(true);
                                _mainSquare.GetComponent<MainSquareController>().setToSmall();
                                break;
                            }
                        }
                        if (_mainSquare.GetComponent<MainSquareController>().IsSmall) break;
                    }
                }
            }
            if (!_mainSquare.GetComponent<MainSquareController>().IsSmall)
                playDeniedSound();
            else
                AudioManager.instance.playSpit();
        }
    }

    public static void playDeniedSound()
    {
        // TODO PLAY I-IH SOUND EFFECT
    }

    private void loadLevel()
    {
        _squares[0].GetComponent<SquareClickHandler>().initialize(false, false, false, false, false);
        _squares[ROWS * COLUMNS - 1].GetComponent<SquareClickHandler>().initialize(false, false, false, false, false);
        for (int squareIndex = 1; squareIndex < ROWS * COLUMNS - 1; squareIndex++)
            _squares[squareIndex].GetComponent<SquareClickHandler>().initialize(
                LEVELS[_currentLevel][squareIndex - 1][SQUARE_ROTATED_INDEX],
                LEVELS[_currentLevel][squareIndex - 1][BLOCK1_ACTIVATED_INDEX],
                LEVELS[_currentLevel][squareIndex - 1][BLOCK1_INTERACTIVE_INDEX],
                LEVELS[_currentLevel][squareIndex - 1][BLOCK2_ACTIVATED_INDEX],
                LEVELS[_currentLevel][squareIndex - 1][BLOCK2_INTERACTIVE_INDEX]);

        _mainSquare.GetComponent<MainSquareController>().initialize();
        _position.x = _squares[0].transform.position.x;
        _position.y = _squares[0].transform.position.y;
        _mainSquare.GetComponent<Rigidbody2D>().MovePosition(_position);
        _startText.transform.position = _position;
        _position.x = _squares[ROWS * COLUMNS - 1].transform.position.x;
        _position.y = _squares[ROWS * COLUMNS - 1].transform.position.y;
        _finishText.transform.position = _position;
        _levelLoaded = _currentLevel;
        _won = false;
    }

    void OnMouseUpAsButton()
    {
        updateSelectedSquare(null);
    }

    IEnumerator win()
    {
        float originalVolume = transform.GetChild(0).GetComponent<AudioSource>().volume;
        transform.GetChild(0).GetComponent<AudioSource>().volume *= 0.5f;
        AudioManager.instance.playWinJingle();
        yield return new WaitForSeconds(AudioManager.instance.WinJingleLength);
        float newVolume;
        while (transform.GetChild(0).GetComponent<AudioSource>().volume < originalVolume)
        {
            newVolume = transform.GetChild(0).GetComponent<AudioSource>().volume + Time.deltaTime * originalVolume;
            transform.GetChild(0).GetComponent<AudioSource>().volume = Mathf.Min(newVolume, originalVolume);
            yield return null;
        }
        if (_currentLevel < LEVEL_COUNT - 1)
            _nextLevelPrompt.SetActive(true);
        else
            _restartPrompt.SetActive(true);
        yield return null;       
    }

}