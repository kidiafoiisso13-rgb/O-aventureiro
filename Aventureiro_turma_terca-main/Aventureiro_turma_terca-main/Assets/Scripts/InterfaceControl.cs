using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterfaceControl : MonoBehaviour
{
    public static InterfaceControl instancia {get; private set; }
    [Header("-- TELAS --")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuOpcoes;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject menuDePause;
    [SerializeField] private GameObject transicao;

    [Header("-- Botões & Icones & textos --")]
    [SerializeField] private Button btnPause;
    [SerializeField] private Sprite iconeBtnPause;
    [SerializeField] private Sprite iconeBtnPlay;
    [SerializeField] private Text orbesTxt;
    [SerializeField] private AudioSource btnClick;
    [SerializeField] private AudioSource somTransicao;
    private bool pause = false;
    private bool jogando = false;
    private int id_tela_atual = 0; // 0 -> Menu | 1 -> Opções | 2 -> HUD

    void Awake()
    {
        if(instancia != null && instancia != this) //&& -> AND || -> OR
        {
            Destroy(gameObject);
        }
        else
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        menuPrincipal.SetActive(true);
        transicao.SetActive(false);
        menuOpcoes.SetActive(false);
        HUD.SetActive(false);
        menuDePause.SetActive(false);
    }

    void Update()
    {
        if(HUD.activeSelf && jogando)
            orbesTxt.text = GameManager.instancia.qtdOrbes();
    }

    public void ResetarCena()
    {
        Time.timeScale = 1;
        GameManager.instancia.ResetarCena();
        pause = false;
        menuDePause.SetActive(false);
    }

    public void CliqueNoBotao()
    {
        btnClick.Play();
    }

    public void Pausar()
    {
        pause = !pause;
        if (pause)
        {
            btnPause.image.sprite = iconeBtnPlay;
            // menu de pause
            menuDePause.SetActive(true);
            // congelar o tempo
            Time.timeScale = 0;
        }
        else
        {
            btnPause.image.sprite = iconeBtnPause;
            menuDePause.SetActive(false);
            Time.timeScale = 1;
        }
    }
    
    public void VoltarParaMenu()
    {
        Time.timeScale = 1;
        transicao.SetActive(true);
        id_tela_atual = 0;
    }
    public void Jogar()
    {
       transicao.SetActive(true);
       somTransicao.Play();
       id_tela_atual = 2;
    }
    public void Opcoes()
    {
        transicao.SetActive(true);
        somTransicao.Play();
        id_tela_atual = 1;
    }

    public void TrocarTela()
    {
        if(id_tela_atual == 0)
        {
            pause = false;
            HUD.SetActive(false);
            menuDePause.SetActive(false);
            menuPrincipal.SetActive(true);
            SceneManager.LoadScene("MENU");
        }
        else if(id_tela_atual == 1)
        {
            menuOpcoes.SetActive(!menuOpcoes.activeSelf);
        }
        else if(id_tela_atual == 2)
        {
            HUD.SetActive(true);
            menuPrincipal.SetActive(false);
            SceneManager.LoadScene("Principal");
            jogando = true;
        }
    }

    public void Sair()
    {
        Application.Quit();
    }
}
