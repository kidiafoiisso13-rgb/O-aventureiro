using UnityEngine;

public class Comuns : MonoBehaviour
{
    public void TrocarTela()
    {
        InterfaceControl.instancia.TrocarTela();
    }

    public void DesativarTransicao()
    {
        gameObject.SetActive(false);
    }
}
