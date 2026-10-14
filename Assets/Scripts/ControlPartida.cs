using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlPartida : MonoBehaviour
{
    public GameObject panelPausa;
    public GameObject panelFin;
    public string escenaMenu = "Menu";
    private bool pausado = false;
    private bool terminado = false;

    void Start()
    {
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
        panelFin.SetActive(false);
    }

    void Update()
    {
        // Esc pausa y reanuda; tras ganar ya no se puede pausar.
        if (!terminado && Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
                Continuar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;
        panelPausa.SetActive(true);
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
    }

    public void MostrarFin()
    {
        terminado = true;
        panelFin.SetActive(true);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }
}
