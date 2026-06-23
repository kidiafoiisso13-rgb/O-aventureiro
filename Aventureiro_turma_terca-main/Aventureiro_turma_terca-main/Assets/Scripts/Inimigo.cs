using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [SerializeField] private int velocidade;
    [SerializeField] private float distanciaMax;
    [SerializeField] private float tempoEspera;
    [SerializeField] private GameObject orbe;
    private GameObject jogador;
    private Vector2 destino;
    private float tempoParado;
    private bool esperando = false;
    private int modo = 0; // 0 -> Patrulha | 1 -> Perseguição
    private Animator animador;
    private SpriteRenderer render;
    public int vida = 5;
    void Start()
    {
        jogador = GameObject.FindWithTag("Player");
        animador = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(modo == 0) Patrulha();
        else Perseguicao();
        animador.SetBool("esperando", esperando);
        render.flipX = destino.x < transform.position.x;
        ControleVida();
    }

    void ControleVida()
    {
        if(vida <= 0)
        {
            Instantiate(orbe, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void Patrulha()
    {
        if (esperando)
        {
            tempoParado -= Time.deltaTime;

            if(tempoParado <= 0)
            {
                esperando = false;
                NovoDestino();
            }

            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino,
            velocidade * Time.deltaTime
        );

        float distancia = Vector2.Distance(transform.position, destino);

        if(distancia < 0.1f)
        {
            esperando = true;
            tempoParado = tempoEspera;
        }
    }

    void NovoDestino()
    {
        Vector2 origem = transform.position;
        
        float x = Random.Range(-distanciaMax,distanciaMax);
        float y = Random.Range(-distanciaMax,distanciaMax);

        destino = origem + new Vector2(x, y);
    }

    void Perseguicao()
    {
        transform.position = Vector2.MoveTowards(transform.position, jogador.transform.position, velocidade * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D colisao)
    {
        if (colisao.gameObject.tag == "Player")
        {
            modo = 1;
        }
        if (colisao.gameObject.tag == "ataqueJogador")
        {
            vida--;
        }
    }

    void OnTriggerExit2D(Collider2D colisao)
    {
        if (colisao.gameObject.tag == "Player")
        {
            modo = 0;
        }
    }
}
