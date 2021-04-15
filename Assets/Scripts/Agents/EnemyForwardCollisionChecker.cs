using UnityEngine;

public class EnemyForwardCollisionChecker : MonoBehaviour
{
    public bool IsCool { get; private set; }

    private GameObject _parentGo;

    private void Start()
    {
        IsCool = true;
    }

    public void SetUp(GameObject parentGo)
    {
        _parentGo = parentGo;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == _parentGo) return;
        if (other.CompareTag("Bullet")) return;

        IsCool = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _parentGo) return;
        if (other.CompareTag("Bullet")) return;

        IsCool = true;
    }
}