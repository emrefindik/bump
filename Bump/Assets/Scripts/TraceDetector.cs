using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceDetector : MonoBehaviour {

    private HashSet<GameObject> _traces;
    public HashSet<GameObject> Traces
    {
        get { return _traces; }
    }

    // Use this for initialization
    void Start()
    {
        _traces = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == RepositionMarkerHandler.TRACES_LAYER)
            _traces.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == RepositionMarkerHandler.TRACES_LAYER)
            _traces.Remove(collision.gameObject);
    }
}
