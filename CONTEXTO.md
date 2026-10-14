# CLAUDE.md — Proyecto "Cazador de Monedas" (Unity 6, 2D)

> Archivo de contexto para Claude Code. Objetivo: construir el juego descrito en la guía
> "Mi primer videojuego 2D en Unity" **más las mejoras pedidas por el usuario** (menú principal,
> pausa y pantalla de fin), trabajando 100 % dentro de Unity (sin motores ni librerías externas).
> Léelo completo antes de tocar nada.

---

## 1. Resumen del juego

**Cazador de monedas**: el jugador (cuadrado azul) se mueve con **flechas o WASD** dentro de un
recinto cerrado por cuatro paredes grises, recoge **5 monedas amarillas** y gana.

Flujo de pantallas:

```
Menu ──Jugar──▶ Nivel1 ──Esc──▶ Pausa ──Continuar / Esc──▶ Nivel1
  ▲               │               ├─Reiniciar──▶ Nivel1 (desde 0)
  │               │               └─Menú principal──▶ Menu
  │               └─5 monedas──▶ Fin ──Reiniciar──▶ Nivel1 (desde 0)
  └──────────────────────────────────┴─Menú principal
Menu ──Salir──▶ cierra el juego (en el editor detiene Play)
```

- Fondo oscuro (azul marino), cámara ortográfica fija (igual en ambas escenas).
- HUD arriba a la izquierda: `Monedas: X / 5`.
- Al recoger la 5.ª moneda: aparece `¡Ganaste! Recogiste todas las monedas.` bajo el contador,
  el jugador **se detiene y ya no responde al teclado**, y se muestra el **panel de fin**.
- **Esc** pausa/reanuda la partida (no funciona después de ganar).

Visual esperado de `Nivel1` (Game view, 16:9):

```
Monedas: 0 / 5
┌──────────────────────────────┐   paredes grises
│      ●        ●        ●     │   ● monedas amarillas (arriba: 3)
│                              │
│              ■               │   ■ jugador azul en el centro
│                              │
│        ●            ●        │   ● monedas (abajo: 2)
└──────────────────────────────┘
```

---

## 2. Entorno y versión

| Elemento | Valor |
|---|---|
| Editor | **Unity 6** (proyecto en **6000.4.8f1**). Mantener el mismo editor todo el proyecto. |
| Plantilla | **Universal 2D (URP)** (la que tiene el proyecto). |
| Nombre del proyecto | `CazadorMonedas` |
| Lenguaje | C# |
| Entrada | **Input Manager clásico** (`Input.GetAxisRaw`, `Input.GetKeyDown`) |
| Física | Física 2D (`Rigidbody2D`, `BoxCollider2D`, `CircleCollider2D`) |
| Interfaz | **UGUI** (`com.unity.ugui`, ya incluido): Canvas + `Text` (legacy) + `Button` |

**Reglas de versión (críticas):**
- Unity 6 usa `Rigidbody2D.linearVelocity` y `linearDamping`. **No** usar `velocity` / `drag` (son de Unity 2022/2023).
- Usar siempre componentes **2D**, nunca `Rigidbody` / `BoxCollider` 3D.
- Decimales con punto (`0.7`), nunca coma, en código y datos.
- Texto con `UnityEngine.UI.Text` y la fuente integrada `LegacyRuntime.ttf`. **No** TextMeshPro
  (requeriría importar recursos extra).

---

## 3. Cómo debe trabajar Claude Code (estrategia)

Claude Code **no puede hacer clic en la interfaz de Unity**, así que todo se genera por código:

1. Scripts de juego en `Assets/Scripts/` (sección 6).
2. Script de editor `Assets/Editor/ConstruirCazadorMonedas.cs` que **construye las dos escenas** (sección 7).
3. Ejecutar el constructor (menú **Herramientas → Construir Cazador de Monedas**, o por línea de comandos, sección 8).
4. Validar con el método `Validar` y la lista de la sección 9.

Reglas generales:
- No inventar mecánicas, arte, sonido ni UI extra. Alcance = lo descrito aquí.
- No añadir paquetes. Todo con lo que trae la plantilla.
- No cambiar valores numéricos de las tablas.
- Cambios con **Play apagado**. Para línea de comandos, Unity debe estar **cerrado**.
- Después de escribir scripts, revisar que la compilación no tenga errores.
- Consultar al usuario antes de instalar algo, cambiar ajustes del proyecto o hacer commits.

---

## 4. Estructura final del proyecto

```
CazadorMonedas/
├─ Assets/
│  ├─ Escenas/
│  │  ├─ Menu.unity                ← escena 0 en Build Settings (inicio)
│  │  └─ Nivel1.unity              ← escena 1 en Build Settings
│  ├─ Scripts/
│  │  ├─ CazadorMonedas.cs         ← en Jugador (movimiento, monedas, HUD)
│  │  ├─ ControlPartida.cs         ← en Partida (pausa, fin, reiniciar, ir al menú)
│  │  └─ MenuPrincipal.cs          ← en Menu (Jugar, Salir)
│  ├─ Editor/
│  │  └─ ConstruirCazadorMonedas.cs ← genera sprites, tag, ajustes y ambas escenas
│  ├─ Sprites/
│  │  ├─ Cuadrado.png              ← 64×64 blanco, PPU 64 (= 1 unidad)
│  │  └─ Circulo.png               ← 64×64 blanco circular, PPU 64 (= 1 unidad de diámetro)
│  └─ Scenes/SampleScene.unity     ← de la plantilla; no se usa (fuera de Build Settings)
├─ ProjectSettings/                ← Active Input Handling = Both; tag "Moneda"
└─ Packages/
```

---

## 5. Especificación de escenas

### 5.1 Ajustes de proyecto
- **Active Input Handling = `Both`** (`activeInputHandler: 2` en `ProjectSettings.asset`).
  Con "Input System Package (New)" solo, `Input.GetAxisRaw` y `StandaloneInputModule` lanzan error.
- **Tag** `Moneda` (M mayúscula).
- **Build Settings**: `Assets/Escenas/Menu.unity` (0), `Assets/Escenas/Nivel1.unity` (1).

### 5.2 Main Camera (en Menu y en Nivel1)

| Propiedad | Valor |
|---|---|
| Position | (0, 0, -10) |
| Rotation | (0, 0, 0) |
| Projection | **Orthographic**, Size **5** |
| Clear Flags / Background Type | Solid Color |
| Background | azul marino oscuro (≈ `#142140`, RGB 0.08, 0.13, 0.25) |

### 5.3 Jugador (cuadrado azul) — Nivel1

| Propiedad | Valor |
|---|---|
| Nombre | `Jugador` |
| Sprite | Cuadrado (1×1 unidad) |
| Position / Rotation / Scale | (0, 0, 0) / (0, 0, 0) / **(0.7, 0.7, 1)** |
| Sprite Renderer → Color | azul (≈ `#0080FF`), Order in Layer 0 |
| **Rigidbody 2D** | Dynamic, Simulated ✔, **Gravity Scale 0**, **Linear Damping 0**, **Freeze Rotation Z ✔** |
| **Box Collider 2D** | **Is Trigger desactivado**, Size/Offset por defecto (1×1 / 0,0) |
| Script `CazadorMonedas` | Velocidad = 4, Meta = 5, Texto Contador, Texto Victoria y Partida asignados |

### 5.4 Paredes (4 objetos, gris ≈ `#8C94A6`, Box Collider 2D sólido, sin Rigidbody 2D) — Nivel1

La dimensión se logra con **Scale**, **no** cambiando el Size del collider.

| Objeto | Position (X, Y, Z) | Scale (X, Y, Z) |
|---|---|---|
| ParedSuperior | 0, 4, 0 | 14, 0.4, 1 |
| ParedInferior | 0, -4, 0 | 14, 0.4, 1 |
| ParedIzquierda | -7, 0, 0 | 0.4, 8, 1 |
| ParedDerecha | 7, 0, 0 | 0.4, 8, 1 |

### 5.5 Monedas (5 objetos, círculo amarillo ≈ `#FFD933`) — Nivel1

Cada una: **Scale (0.45, 0.45, 1)**, **Circle Collider 2D con Is Trigger ACTIVADO**, **Tag = `Moneda`**, **sin Rigidbody 2D**.

| Objeto | Position (X, Y, Z) |
|---|---|
| Moneda1 | -4, 2, 0 |
| Moneda2 | 0, 2, 0 |
| Moneda3 | 4, 2, 0 |
| Moneda4 | -3, -2, 0 |
| Moneda5 | 3, -2, 0 |

> Pared = collider sólido (bloquea). Moneda = trigger (detecta sin bloquear).

### 5.6 Interfaz de Nivel1

`Canvas` (Screen Space - Overlay, Canvas Scaler **Constant Pixel Size**) + `EventSystem` con
`StandaloneInputModule`. Objeto vacío `Partida` con `ControlPartida` (Panel Pausa y Panel Fin asignados).
Todo el texto es blanco, fuente `LegacyRuntime.ttf`.

| Elemento | Ancla | Posición / Tamaño | Contenido |
|---|---|---|---|
| TextoContador | arriba-izq. | (20, -20) / 420×40, fuente 24 | `Monedas: X / 5` |
| TextoVictoria | arriba-izq. | (20, -65) / 500×40, fuente 24, **inactivo al inicio** | `¡Ganaste! Recogiste todas las monedas.` |
| PanelPausa | pantalla completa | negro alfa 0.6, **inactivo al inicio** | Título `Pausa` (48) en (0,120); botones **Continuar** (0,30), **Reiniciar** (0,-40), **Menú principal** (0,-110) |
| PanelFin | centro | 420×260, azul oscuro alfa 0.92, **inactivo al inicio** | Título `Fin de la partida` (32) en (0,75); botones **Reiniciar** (0,-5), **Menú principal** (0,-75) |

Botones: 260×50, color azul (≈ `#0080FF`), texto 24 centrado.

### 5.7 Escena Menu

Main Camera (5.2), Canvas + EventSystem (igual que 5.6), objeto `Menu` con `MenuPrincipal`.

| Elemento | Posición / Tamaño | Contenido / Acción |
|---|---|---|
| Titulo | (0, 120) / 800×90, fuente 56 | `Cazador de Monedas` |
| BotonJugar | (0, 0) / 260×50 | `Jugar` → `MenuPrincipal.Jugar` (carga Nivel1) |
| BotonSalir | (0, -70) / 260×50 | `Salir` → `MenuPrincipal.Salir` |

---

## 6. Scripts de juego — `Assets/Scripts/`

Archivo y clase **deben llamarse igual**. Guardar en UTF-8 (para `¡` y `ú`).

### 6.1 `CazadorMonedas.cs` (en Jugador)

```csharp
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
```

### 6.2 `ControlPartida.cs` (en Partida)

```csharp
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
```

### 6.3 `MenuPrincipal.cs` (en Menu)

```csharp
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
```

Notas de diseño (no cambiar):
- `normalized` evita ir más rápido en diagonal.
- `Update` lee teclado; `FixedUpdate` aplica velocidad al cuerpo físico.
- La pausa usa `Time.timeScale = 0` (congela la física). Toda salida de la pausa o cambio de escena
  restablece `Time.timeScale = 1`.
- `Velocidad` y `Meta` son campos públicos: el valor que manda es el del **Inspector**, no el del código.
- Los botones se conectan con listeners **persistentes** (visibles en el `On Click ()` del Inspector).

---

## 7. Constructor de escenas — `Assets/Editor/ConstruirCazadorMonedas.cs`

El archivo del repositorio es la **fuente de verdad** (no se duplica aquí). Es **idempotente**: se puede
ejecutar varias veces y regenera ambas escenas. Hace, en orden:

1. `ConfigurarProyecto`: modo 2D, Active Input Handling 1 → 2 si hace falta, tag `Moneda`.
2. `CrearCarpetas`: `Escenas`, `Scripts`, `Sprites`, `Editor`.
3. `CrearSprite`: `Cuadrado.png` y `Circulo.png` (64×64, PPU 64).
4. `CrearEscena`: `Nivel1` con cámara, jugador, paredes, monedas e interfaz (5.2–5.6).
5. `CrearEscenaMenu`: `Menu` (5.7).
6. Build Settings = `Menu`, `Nivel1`.

`Validar()` abre ambas escenas y comprueba cámara, jugador, paredes, 5 monedas, HUD y paneles asignados,
EventSystem en cada escena, objeto `Menu` con botones Jugar/Salir y el orden de Build Settings.
Termina con `[VALIDAR] OK: la escena cumple la especificación.` (código de salida 0 en batch).

---

## 8. Ejecución

**Desde el editor:** menú **Herramientas → Construir Cazador de Monedas** → abrir
`Assets/Escenas/Menu.unity` → **Play** → **Jugar** (clic dentro de Game para dar foco al teclado).

**Desde línea de comandos** (con Unity **cerrado** y licencia activa):

```bash
# Construir
"C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe" -batchmode -nographics -quit \
  -projectPath "<ruta>/CazadorMonedas" -executeMethod ConstruirCazadorMonedas.Construir -logFile <log>

# Validar
"C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe" -batchmode -nographics \
  -projectPath "<ruta>/CazadorMonedas" -executeMethod ConstruirCazadorMonedas.Validar -logFile <log>
```

Los avisos `[Licensing::Client] Error: Code 404 ...` del log son inofensivos si termina con
`Exiting batchmode successfully now!`.

---

## 9. Criterios de aceptación (checklist)

Estructura y compilación
- [ ] Existen `Menu.unity`, `Nivel1.unity` y los 3 scripts; Build Settings = Menu (0), Nivel1 (1).
- [ ] La consola de Unity no tiene errores rojos.
- [ ] `Validar` termina con `OK`.

Comportamiento (empezar Play desde `Menu`)

| Prueba | Resultado esperado |
|---|---|
| Menú | Título y botones Jugar / Salir; Jugar carga Nivel1; Salir detiene Play (o cierra el juego). |
| Movimiento | Responde a flechas **y** WASD; no cae ni gira; diagonal no es más rápida. |
| Paredes | No atraviesa ninguna de las 4 paredes. |
| Monedas | Cada moneda desaparece y suma **una sola vez**. |
| Pausa | Esc congela el juego y muestra el panel; Continuar o Esc reanudan; Reiniciar empieza desde 0; Menú principal vuelve al menú. |
| Victoria | Contador `5 / 5`, mensaje de victoria, jugador detenido, panel de fin; Esc ya no pausa. |
| Fin | Reiniciar reaparece las 5 monedas con contador 0; Menú principal vuelve al menú. |
| HUD | Texto blanco, tamaño 24, arriba a la izquierda, legible sobre el fondo oscuro. |

---

## 10. Problemas frecuentes

| Problema | Qué revisar |
|---|---|
| No se mueve | Foco en Game, script en `Jugador`, Velocidad > 0, sin errores rojos, juego no pausado. |
| Error de Input (`InvalidOperationException`) | Active Input Handling = Both; reiniciar editor. |
| Los botones no responden | Debe haber un `EventSystem` en la escena; Canvas con `GraphicRaycaster`. |
| Todo queda congelado al cambiar de escena | Alguna ruta no restableció `Time.timeScale = 1`. |
| "Jugar" no carga Nivel1 | Nivel1 debe estar en Build Settings con el nombre exacto `Nivel1`. |
| Cae o gira | Gravity Scale = 0 y Freeze Rotation Z. |
| Atraviesa paredes | Box Collider 2D en jugador y paredes, Is Trigger apagado, jugador Dynamic y Simulated. |
| Monedas no suman | Tag = `Moneda`, Circle Collider 2D con Is Trigger, Rigidbody 2D en Jugador. |
| `linearVelocity` no existe | No es Unity 6, o se usa `Rigidbody` 3D. |
| No aparece la victoria | Debe haber exactamente 5 monedas etiquetadas y Meta = 5. |
| Sprites rosas/ausentes (URP) | Usar el material de sprite por defecto del pipeline; no cambiar shaders manualmente. |

---

## 11. Retos opcionales (solo si el usuario los pide)

1. Cambiar colores/posiciones manteniendo las monedas dentro del escenario.
2. Añadir 3 monedas más en lugares accesibles y poner **Meta = 8** en el Inspector de `Jugador`.
3. Crear un obstáculo: Square gris + Box Collider 2D con Is Trigger **desactivado** y **sin** Rigidbody 2D.
4. Mostrar el tiempo que se tardó en recoger las monedas en el panel de fin.
5. Añadir instrucciones (controles, objetivo, tecla Esc) en el menú.

---

## 12. Fuera de alcance (no hacer)

- Sonido, partículas, animaciones, TextMeshPro, Input System nuevo para el movimiento.
- Compilar/exportar el juego (Build Profiles) salvo petición explícita.
- Modificar valores de las tablas de la sección 5 o renombrar objetos, carpetas, tag o clases.
- Subir a control de versiones `Library/`, `Temp/`, `Logs/`, `obj/` (usar el `.gitignore` estándar de Unity).

Para conservar el proyecto se entrega **la carpeta completa del proyecto**, pudiendo excluir `Library` y `Temp`.
