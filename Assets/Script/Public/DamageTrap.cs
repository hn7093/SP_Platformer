using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrap : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1;
    float lastTime = 0;
    List<IDamagable> damagables = new List<IDamagable>();
    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageInterval);
    }

    void DealDamage()
    {
        for(int i = 0; i<damagables.Count; i++)
        {
            damagables[i].TakeDamage(damage);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagable))
        {
            damagables.Add(damagable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagable))
        {
            damagables.Remove(damagable);
        }
    }
}
