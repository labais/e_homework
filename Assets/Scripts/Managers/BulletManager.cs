using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager I;

    [SerializeField] private Bullet _prefab;

    private List<Bullet> _pool = new List<Bullet>();
    
    
    private void Awake()
    {
        I = this;
        _prefab.gameObject.SetActive(false);
    }

    public void Shoot(Vector3 start, Vector3 target, Vector3 instantMovement)
    {
        Bullet foundBullet = null;
        
        foreach (var bullet in _pool)
        {
            if (bullet.Free)
            {
                foundBullet = bullet;
                break;
            }
        }

        if (foundBullet == null)
        {
            foundBullet = Instantiate(_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Bullet>();
            _pool.Add(foundBullet);
        }

        foundBullet.Shoot(start, target, instantMovement);


    }


}
