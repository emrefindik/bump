using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionMarkerHandler : MonoBehaviour {
    private const int TRIANGLES_LAYER = 9;
    public const int TRACES_LAYER = 13;

    private HashSet<GameObject> _traces;
    public HashSet<GameObject> Traces
    {
        get { return _traces; }
    }

    private HashSet<GameObject> _triangles;
    public HashSet<GameObject> Triangles
    {
        get { return _triangles; }
    }

    // Use this for initialization
    void Start()
    {
        _traces = new HashSet<GameObject>();
        _triangles = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == TRACES_LAYER)
            _traces.Add(collision.gameObject);
        else if (collision.gameObject.layer == TRIANGLES_LAYER)
            _triangles.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == TRACES_LAYER)
            _traces.Remove(collision.gameObject);
        else if (collision.gameObject.layer == TRIANGLES_LAYER)
            _triangles.Remove(collision.gameObject);
    }
}
