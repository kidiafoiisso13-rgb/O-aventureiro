using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia {get; private set;}
    [SerializeField] private List<Fase> fases;
    [SerializeField] private AudioSource somOrbe;
    // [SerializeField] private AudioSource somAtaque;
    [SerializeField] private AudioSource btnClick;
    [SerializeField] private AudioSource somTransicao;
    [SerializeField] private Slider _sliderVolume;
    private int cenaAtual = 0;
    public int orbesColetados = 0;
    void Start()
    {
        AjustarVolume();
    }
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
    public String qtdOrbes()
    {
        if(orbesColetados > 9) return "0"+orbesColetados;
        return "00"+orbesColetados;
    }
    public void ResetarCena()
    {
        orbesColetados = 0;
        SceneManager.LoadScene(fases[cenaAtual].nome);
    }
    public void TrocarCena()
    {
        if(orbesColetados == fases[cenaAtual].qtdOrbes)
        {
            cenaAtual++;
            if(cenaAtual < fases.Count-1)
            {
                orbesColetados = 0;
                SceneManager.LoadScene(fases[cenaAtual].nome);
            }
        }
    }
    public void ColetarOrbe()
    {
        orbesColetados++;
        somOrbe.Play();
    }

    public void AjustarVolume()
    {
        somOrbe.volume = _sliderVolume.value;
        // somAtaque.volume = _sliderVolume.value;
        btnClick.volume = _sliderVolume.value;
        somTransicao.volume = _sliderVolume.value;
    }
}
