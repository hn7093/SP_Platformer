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

    private bool canHealStemina = true;
    private bool canUseStemina = true;

    void FixedUpdate()
    {
        if (!stamina.IsFull() && canHealStemina)
        {
            stamina.Add(stamina.passiveValue * Time.deltaTime);
            // 사용불가 해제
            if (stamina.IsFull())
            {
                canUseStemina = true;
                stamina.SetGray(false);
            }
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
        Heal(100);
        CharacterManager.Instance.Player.movement.ResetPosition();
    }
    public void TakeDamage(int amount)
    {
        health.Subtract(amount);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        // 사용 스테미너 확인
        if (stamina.curValue - amount < 0f || !canUseStemina)
        {
            return false;
        }
        // 전부 다썼으면 다 찰때까지 사용불가
        if (stamina.curValue < 1f)
        {
            canUseStemina = false;
            stamina.SetGray(true);
        }
        stamina.Subtract(amount);
        return true;
    }

    public void ActiveStemina(bool active)
    {
        canHealStemina = active;
    }
}
