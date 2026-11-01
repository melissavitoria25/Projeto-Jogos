using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D enemyRb;
    [SerializeField] float enemyXSpeed;

    float enemyDir = 1;

    Material material;

    int dissolveId = Shader.PropertyToID("_DissolveAmount");

    void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        material = GetComponentInChildren<SpriteRenderer>().material;
    }

    void Start()
    {
        StartCoroutine(IEStartDissolveAndDie());
    }

    void FixedUpdate()
    {
        enemyRb.linearVelocityX = enemyXSpeed * enemyDir;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        FlipSprite();
    }

    void FlipSprite()
    {   
        float sinal = Mathf.Sign(enemyRb.linearVelocityX); //se for positivo retorna +1, senão retorna -1
        transform.localScale = new Vector3(-sinal, 1, 1);
        enemyDir *= -1;
    }

    //corrotina: dispara um evento e ela sai e volta, ou para(encerra de vez)
    //Interpolação linear (inicio, fim, quanto andou) onde vc deveria estar entre 0 e 1
    //valores intermediarios entre origem e fim
    IEnumerator IEStartDissolveAndDie()
    {
        float tempoDecorrido = 0;
        float duracao = 2;

        while (tempoDecorrido <= duracao)
        {
            tempoDecorrido += Time.deltaTime;
            float dissolveAmount = Mathf.Lerp(0, 1.1f, tempoDecorrido/duracao);
            material.SetFloat(dissolveId, dissolveAmount);
            yield return null;
        }
    }



}
