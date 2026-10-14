using UnityEngine;
using UnityEngine.UI;

public class CazadorMonedas : MonoBehaviour
{
    public float velocidad = 4f;
    public int meta = 5;
    public Text textoContador;
    public Text textoVictoria;
    public ControlPartida partida;
    private Rigidbody2D cuerpo;
    private Vector2 direccion;
    private int puntos = 0;
    private bool gano = false;

    void Start()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        if (textoVictoria != null)
            textoVictoria.gameObject.SetActive(false);
        ActualizarContador();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        direccion = new Vector2(x, y).normalized;
        if (gano)
            direccion = Vector2.zero;
    }

    void FixedUpdate()
    {
        cuerpo.linearVelocity = direccion * velocidad;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (gano || !otro.CompareTag("Moneda"))
            return;
        // Evita contar la misma moneda dos veces.
        otro.enabled = false;
        Destroy(otro.gameObject);
        puntos++;
        ActualizarContador();
        if (puntos >= meta)
        {
            gano = true;
            cuerpo.linearVelocity = Vector2.zero;
            if (textoVictoria != null)
                textoVictoria.gameObject.SetActive(true);
            if (partida != null)
                partida.MostrarFin();
        }
    }

    void ActualizarContador()
    {
        if (textoContador != null)
            textoContador.text = "Monedas: " + puntos + " / " + meta;
    }
}
