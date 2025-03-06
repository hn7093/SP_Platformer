using System;
using UnityEngine;
public interface IDamagable
{
    void TakeDamage(int amount);
}
public class PlayerStat : MonoBehaviour, IDamagable
{
    public Stat health;
    public Stat stamina;
    public event Action onTakeDamage;

    void FixedUpdate()
    {
        if (!stamina.IsFull())
        {
            stamina.Add(stamina.passiveValue * Time.deltaTime);
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        stamina.Add(amount);
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }
    public void TakeDamage(int amount)
    {
        health.Subtract(amount);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        // 사용 스테미너 확인
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }
}
