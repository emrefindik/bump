using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetector : MonoBehaviour {

    private HashSet<GameObject> _interactiveTriangles;
    public HashSet<GameObject> InteractiveTriangles
    {
        get { return _interactiveTriangles; }
    }

    private HashSet<GameObject> _frozenTriangles;
    public HashSet<GameObject> FrozenTriangles
    {
        get { return _frozenTriangles; }
    }

    // Use this for initialization
    void Start () {
        _interactiveTriangles = new HashSet<GameObject>();
        _frozenTriangles = new HashSet<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TriangleClickHandler>().IsInteractive)
            _interactiveTriangles.Add(collision.gameObject);
        else
            _frozenTriangles.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TriangleClickHandler>().IsInteractive)
            _interactiveTriangles.Remove(collision.gameObject);
        else
            _frozenTriangles.Remove(collision.gameObject);
    }
}
