using UnityEngine;

public class StartPostion : MonoBehaviour
{
    public LayerMask collisionMask;
    private bool _started = false;
    private GameObject _innerGameObject;
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GameState == 1)
            return;
        var layer = other.gameObject.layer;
        if (collisionMask == (collisionMask | (1 << layer)))
        {
            _innerGameObject = gameObject;
            GameManager.Instance.GameState = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_innerGameObject == other?.gameObject)
        {
            GameManager.Instance.GameState = 2;
        }
    }
}
