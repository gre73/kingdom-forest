using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthBar; 
    
    private Animator animator;
    private HeroKnight heroKnight;
    private PlayerAttack playerAttack;

    void Start()
    {
        currentHealth = maxHealth;
        
        animator = GetComponent<Animator>();
        heroKnight = GetComponent<HeroKnight>();
        playerAttack = GetComponent<PlayerAttack>();
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (heroKnight != null && heroKnight.isDead) return;

        currentHealth -= damage;
        
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (heroKnight != null)
        {
            heroKnight.isDead = true; 
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        
        if (playerAttack != null)
        {
            playerAttack.enabled = false; 
        }

        // 4. Запускаємо анімацію смерті
        if (animator != null)
        {
            animator.SetBool("noBlood", heroKnight.noBlood);
            animator.SetTrigger("Death");
        }

        Debug.Log("Гравець помер! Гру закінчено.");
    }
}