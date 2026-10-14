using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public string escenaJuego = "Nivel1";

    public void Jugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaJuego);
    }

    public void Salir()
    {
#if UNITY_EDITOR
        // En el editor, detiene Play.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
