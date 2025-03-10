using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectDamageTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
        }
    }
}
