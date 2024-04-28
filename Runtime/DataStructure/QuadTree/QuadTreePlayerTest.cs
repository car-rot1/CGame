using UnityEngine;

public class QuadTreePlayerTest : MonoBehaviour
{
    public float speed;
    public QuadTreeTest quadTreeTest;

    private Transform[] _points;
    private int _length;

    private Transform _tr;
    private SphereCollider _sphereCollider;

    private void Awake()
    {
        _points = new Transform[100];
        
        _tr = transform;
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        var direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        _tr.position += speed * Time.deltaTime * direction;

        for (var i = 0; i < _length; i++)
        {
            _points[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }
        _length = quadTreeTest.Root.GetAllPointNonAlloc(_tr.position, _sphereCollider.radius, _points);
        for (var i = 0; i < _length; i++)
        {
            _points[i].GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
