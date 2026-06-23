using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para o efeito de glitch/dash
using TMPro; // ADICIONADO: Necessário para reconhecer o componente de Texto TextMeshPro

public class Jogador : MonoBehaviour
{
    [SerializeField] private int velocidade;
    [SerializeField] private Rigidbody2D fisica;
    [SerializeField] private Animator animador;
    [SerializeField] private AudioSource somAtaque;

    private Vector2 direcao;
    private Vector2 ultimaDirecao;
    private bool atacando = false;

    // --- NOVAS VARIÁVEIS PARA A REALIDADE BUGADA (GDD) ---
    [Header("Mecânicas de Glitch")]
    [SerializeField] private float velocidadeDash = 20f;
    [SerializeField] private float duracaoDash = 0.2f;
    [SerializeField] private float cooldownDash = 1f;
    private bool podeDarDash = true;
    private bool estaDandoDash = false;
    private SpriteRenderer spriteRenderer;
    // -----------------------------------------------------

    [Header("Interface do Jogo (UI)")]

    public GameObject textoVitoria; 

    void Start()
    {
        fisica = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {

        if (estaDandoDash) return;

        fisica.linearVelocity = direcao * velocidade;

        animador.SetFloat("eixoX", direcao.x);
        animador.SetFloat("eixoY", direcao.y);
        animador.SetFloat("lastX", ultimaDirecao.x);
        animador.SetFloat("lastY", ultimaDirecao.y);
        animador.SetBool("correndo", direcao != Vector2.zero);

        if (direcao != Vector2.zero)
        {
            ultimaDirecao = direcao;
        }
    }

    public void Movimentar(InputAction.CallbackContext contexto)
    {
        direcao = contexto.ReadValue<Vector2>();
    }

    public void Atacar(InputAction.CallbackContext contexto)
    {
        if (estaDandoDash) return; 

        if (contexto.started && !atacando)
        {
            atacando = true;
            animador.SetTrigger("atacando");
            somAtaque.Play();
        }
    }

    public void FimAtack()
    {
        atacando = false;
    }

    // --- NOVA FUNÇÃO: DASH DE FRAGMENTAÇÃO (Mecânica do GDD) ---
    public void DashGlitch(InputAction.CallbackContext contexto)
    {
        if (contexto.started && podeDarDash && !atacando && direcao != Vector2.zero)
        {
            StartCoroutine(ExecutarDashGlitch());
        }
    }

    private IEnumerator ExecutarDashGlitch()
    {
        podeDarDash = false;
        estaDandoDash = true;


        fisica.linearVelocity = direcao.normalized * velocidadeDash;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0f, 1f, 1f, 0.5f); 
        }

        // Desativa colisão com "Paredes Bugadas" temporariamente
        int camadaJogador = gameObject.layer;
        int camadaParedesBugadas = LayerMask.NameToLayer("GlitchPlatform");
        if (camadaParedesBugadas != -1) Physics2D.IgnoreLayerCollision(camadaJogador, camadaParedesBugadas, true);

        yield return new WaitForSeconds(duracaoDash);

        // Retorna ao estado normal
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; 
        }
        if (camadaParedesBugadas != -1) Physics2D.IgnoreLayerCollision(camadaJogador, camadaParedesBugadas, false);

        estaDandoDash = false;

        yield return new WaitForSeconds(cooldownDash);
        podeDarDash = true;
    }
    // -------------------------------------------------------------

    void OnTriggerEnter2D(Collider2D colisao)
    {
        if (colisao.gameObject.tag == "Finish")
        {
            Debug.Log("Jogador venceu o desafio final!");

            if (textoVitoria != null)
            {
                textoVitoria.SetActive(true);
            }

            direcao = Vector2.zero;
            fisica.linearVelocity = Vector2.zero;
        }
        
        if (colisao.gameObject.tag == "orbe")
        {
            Destroy(colisao.gameObject);
            if (GameManager.instancia != null) GameManager.instancia.ColetarOrbe();
        }
    }

    private IEnumerator EsperarEMudarDeFase()
    {
        yield return new WaitForSeconds(2f);

        if (GameManager.instancia != null)
        {
            GameManager.instancia.TrocarCena();
        }
        else
        {
            int proximaCena = SceneManager.GetActiveScene().buildIndex + 1;
            if (proximaCena < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(proximaCena);
            }
            else
            {
                Debug.Log("Você venceu o jogo! Não há próxima fase.");
            }
        }
    }
}