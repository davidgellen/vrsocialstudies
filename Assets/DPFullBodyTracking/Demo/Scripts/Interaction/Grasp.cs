using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grasp : MonoBehaviour
{
    [SerializeField, Range(1, 5)] private int _fingersInCollision = 2; 

    private FixedJoint _joint;
    private Rigidbody _rb;

    private Dictionary<Vector3, GameObject> _collisionNormals = new Dictionary<Vector3, GameObject>();

    private void Update()
    {
        if (_collisionNormals.Count >= _fingersInCollision)
        {
            if (!TryGetComponent(out _joint))
            {
                _joint = gameObject.AddComponent<FixedJoint>();
                _joint.connectedBody = _rb;
                _joint.connectedMassScale = 0.2f;
            }
        }
        else 
        {
            if (_joint) 
            {
                Destroy(_joint);
                _joint = null;        
                _rb = null;
                _collisionNormals.Clear();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.childCount != 0 && collision.transform.GetChild(0).gameObject.CompareTag("FingerTip")) 
        {
            var joint = collision.transform.parent.GetComponent<ConfigurableJoint>();
        }

        if (collision.gameObject.CompareTag("FingerTip")) 
        {
            if (!_rb)
            {
                _rb = collision.transform.parent.parent.parent.gameObject.GetComponent<Rigidbody>();
            }

            var normal = collision.GetContact(0).normal;
            if (!_collisionNormals.ContainsKey(normal))
                _collisionNormals.Add(normal, collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.childCount != 0 && collision.transform.GetChild(0).gameObject.CompareTag("FingerTip"))
        {
            var joint = collision.transform.parent.GetComponent<ConfigurableJoint>();
        }

        if (collision.gameObject.CompareTag("FingerTip"))
        {
            var key = _collisionNormals.FirstOrDefault(x => x.Value == collision.gameObject).Key;
            if (key != null)
                _collisionNormals.Remove(key);
        }
    }
}

