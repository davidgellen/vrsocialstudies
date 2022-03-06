using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public LayerMask collisionMask;
    public GameObject shattered;

    private bool _destroy = false;

    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
        _destroy = false;
    }

    void Update()
    {
        transform.Translate(Vector3.back * GameManager.Instance.GameSpeed * Time.deltaTime, Space.World);

        if (transform.position.z <= -3f) 
        {
            transform.localPosition = Vector3.zero;
            GameManager.Instance.SpawnNextWall = true;
            GameManager.Instance.PlayConfirmSound();
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        var layer = other.gameObject.layer;
        if (!_destroy && collisionMask == (collisionMask | (1 << layer)))
        {
            Debug.Log(other.collider.name);
            _destroy = true;
            GameManager.Instance.DecreaseLives();
            gameObject.SetActive(false);
        }
    }
}
