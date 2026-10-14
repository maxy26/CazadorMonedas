using UnityEngine;

public class CazadorMonedas : MonoBehaviour
{
    public float velocidad = 4f;
    public int meta = 5;
    private Rigidbody2D cuerpo;
    private Vector2 direccion;
    private int puntos = 0;
    private bool gano = false;

    void Start()
    {
        cuerpo = GetComponent<Rigidbody2D>();
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
        if (puntos >= meta)
        {
            gano = true;
            cuerpo.linearVelocity = Vector2.zero;
        }
    }

    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 24;
        estilo.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 420, 40),
            "Monedas: " + puntos + " / " + meta, estilo);
        if (gano)
            GUI.Label(new Rect(20, 65, 500, 40),
                "¡Ganaste! Recogiste todas las monedas.", estilo);
    }
}
