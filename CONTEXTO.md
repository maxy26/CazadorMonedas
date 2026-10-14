# CLAUDE.md — Proyecto "Cazador de Monedas" (Unity 6, 2D)

> Archivo de contexto para Claude Code. Objetivo: construir **exactamente** el juego
> descrito en la guía "Mi primer videojuego 2D en Unity", trabajando 100 % dentro de Unity
> (sin motores ni librerías externas). Léelo completo antes de tocar nada.

---

## 1. Resumen del juego

**Cazador de monedas**: el jugador (cuadrado azul) se mueve con **flechas o WASD** dentro de un
recinto cerrado por cuatro paredes grises, recoge **5 monedas amarillas** y gana.

- Fondo oscuro (azul marino), cámara ortográfica fija.
- HUD arriba a la izquierda: `Monedas: X / 5`.
- Al recoger la 5.ª moneda: aparece `¡Ganaste! Recogiste todas las monedas.` bajo el contador y el jugador **se detiene y ya no responde al teclado**.
- No hay reinicio dentro del juego: al detener y volver a pulsar Play, la escena guardada reaparece con las 5 monedas.

Visual esperado (Game view, 16:9):

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
| Editor | **Unity 6 LTS** (6000.x). Mantener el mismo editor todo el proyecto. |
| Plantilla | **2D (Built-In Render Pipeline)**. Si solo existe Universal 2D, también sirve. |
| Nombre del proyecto | `CazadorMonedas` |
| Lenguaje | C# |
| Entrada | **Input Manager clásico** (`Input.GetAxisRaw`) |
| Física | Física 2D (`Rigidbody2D`, `BoxCollider2D`, `CircleCollider2D`) |

**Reglas de versión (críticas):**
- Unity 6 usa `Rigidbody2D.linearVelocity` y `linearDamping`. **No** usar `velocity` / `drag` (son de Unity 2022/2023).
- Usar siempre componentes **2D**, nunca `Rigidbody` / `BoxCollider` 3D.
- Decimales con punto (`0.7`), nunca coma, en código y datos.

> Si el proyecto aún no existe: el usuario lo crea en Unity Hub (plantilla 2D, nombre `CazadorMonedas`)
> y abre a Claude Code en la carpeta raíz del proyecto (donde están `Assets/`, `Packages/`, `ProjectSettings/`).
> Crear el proyecto por línea de comandos (`-createProject`) genera uno 3D, por eso no se recomienda.

---

## 3. Cómo debe trabajar Claude Code (estrategia)

Claude Code **no puede hacer clic en la interfaz de Unity**, así que la guía (que es paso a paso con el ratón)
se traduce en **archivos que generan todo por código**:

1. Escribir `Assets/Scripts/CazadorMonedas.cs` (script de juego, sección 6).
2. Escribir `Assets/Editor/ConstruirCazadorMonedas.cs` (script de editor que **construye la escena completa**, sección 7).
3. Ejecutar el constructor (menú **Herramientas → Construir Cazador de Monedas**, o por línea de comandos, sección 8).
4. Validar con el método `Validar` y la lista de la sección 9.

Reglas generales:
- No inventar mecánicas, arte, sonido ni UI extra. Alcance = lo descrito aquí.
- No añadir paquetes. Todo con lo que trae la plantilla 2D.
- No cambiar valores numéricos de las tablas. Son los de la guía.
- Cambios con **Play apagado**.
- Después de escribir scripts, esperar a que Unity compile y revisar que la consola no tenga errores rojos.

---

## 4. Estructura final del proyecto

```
CazadorMonedas/
├─ Assets/
│  ├─ Escenas/
│  │  └─ Nivel1.unity              ← escena única (en Build Settings)
│  ├─ Scripts/
│  │  └─ CazadorMonedas.cs         ← solo se adjunta a Jugador
│  ├─ Editor/
│  │  └─ ConstruirCazadorMonedas.cs ← herramienta de generación (extra, opcional conservarla)
│  └─ Sprites/
│     ├─ Cuadrado.png              ← 64×64 blanco, PPU 64 (= 1 unidad)
│     └─ Circulo.png               ← 64×64 blanco circular, PPU 64 (= 1 unidad de diámetro)
├─ ProjectSettings/                ← Active Input Handling = Both; tag "Moneda"
└─ Packages/
```

Las carpetas `Escenas` y `Scripts` son las de la guía. `Editor` y `Sprites` son necesarias solo porque
se genera todo por código (equivalen a los sprites integrados *Square* y *Circle* del menú 2D Object).

---

## 5. Especificación de la escena `Nivel1`

### 5.1 Ajustes de proyecto
- **Edit → Project Settings → Player → Other Settings → Configuration → Active Input Handling = `Both`**
  (valor `2` en `ProjectSettings.asset` → `activeInputHandler: 2`). Si no existe, `Input Manager (Old)` (`0`).
  Si se deja solo "Input System Package (New)" el script lanza error al leer el teclado.
  Cambiarlo puede requerir **reiniciar el editor**.
- **Tag** nuevo: `Moneda` (M mayúscula).

### 5.2 Main Camera

| Propiedad | Valor |
|---|---|
| Position | (0, 0, -10) |
| Rotation | (0, 0, 0) |
| Projection | **Orthographic** |
| Size | **5** |
| Clear Flags / Background Type | Solid Color |
| Background | azul marino oscuro (≈ `#142140`, RGB 0.08, 0.13, 0.25) |
| Aspecto del Game view | 16:9 (ajuste del editor, no del archivo) |

### 5.3 Jugador (cuadrado azul)

| Propiedad | Valor |
|---|---|
| Nombre | `Jugador` |
| Sprite | Cuadrado (1×1 unidad) |
| Position / Rotation / Scale | (0, 0, 0) / (0, 0, 0) / **(0.7, 0.7, 1)** |
| Sprite Renderer → Color | azul (≈ `#0080FF`) |
| Order in Layer | 0 |
| **Rigidbody 2D** | Body Type **Dynamic**, Simulated ✔, **Gravity Scale 0**, **Linear Damping 0**, Constraints → **Freeze Rotation Z ✔** |
| **Box Collider 2D** | **Is Trigger desactivado**, Size/Offset por defecto (1×1 / 0,0) |
| Script | `CazadorMonedas` (Velocidad = 4, Meta = 5) — **solo aquí** |

### 5.4 Paredes (4 objetos, color gris, sin Rigidbody 2D)

Cada una: Sprite Cuadrado gris (≈ `#8C94A6`), **Box Collider 2D con Is Trigger desactivado**, Rotation (0,0,0).
La dimensión se logra con **Scale**, **no** cambiando el Size del collider.

| Objeto | Position (X, Y, Z) | Scale (X, Y, Z) |
|---|---|---|
| ParedSuperior | 0, 4, 0 | 14, 0.4, 1 |
| ParedInferior | 0, -4, 0 | 14, 0.4, 1 |
| ParedIzquierda | -7, 0, 0 | 0.4, 8, 1 |
| ParedDerecha | 7, 0, 0 | 0.4, 8, 1 |

### 5.5 Monedas (5 objetos, círculo amarillo)

Cada una: Sprite Circulo amarillo (≈ `#FFD933`), Rotation (0,0,0), **Scale (0.45, 0.45, 1)**,
**Circle Collider 2D con Is Trigger ACTIVADO**, **Tag = `Moneda`**, **sin Rigidbody 2D**
(el del Jugador permite detectar el contacto).

| Objeto | Position (X, Y, Z) | Tag |
|---|---|---|
| Moneda1 | -4, 2, 0 | Moneda |
| Moneda2 | 0, 2, 0 | Moneda |
| Moneda3 | 4, 2, 0 | Moneda |
| Moneda4 | -3, -2, 0 | Moneda |
| Moneda5 | 3, -2, 0 | Moneda |

> Pared = collider sólido (bloquea). Moneda = trigger (detecta sin bloquear).

---

## 6. Script de juego — `Assets/Scripts/CazadorMonedas.cs`

Archivo y clase **deben llamarse igual** (`CazadorMonedas`). Copiar tal cual (UTF-8, para que `¡` se vea bien).

```csharp
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
```

Notas de diseño (no cambiar):
- `normalized` evita ir más rápido en diagonal.
- `Update` lee teclado; `FixedUpdate` aplica velocidad al cuerpo físico.
- `OnGUI` (IMGUI) es intencional para este prototipo: **no** usar Canvas/TextMeshPro.
- `Velocidad` y `Meta` son campos públicos: el valor que manda es el del **Inspector**, no el del código.

---

## 7. Constructor de escena — `Assets/Editor/ConstruirCazadorMonedas.cs`

Genera sprites, tag, ajustes, escena `Nivel1` completa y la añade a Build Settings.
Es **idempotente**: se puede ejecutar varias veces (regenera la escena).

```csharp
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConstruirCazadorMonedas
{
    const string CarpetaSprites = "Assets/Sprites";
    const string RutaEscena = "Assets/Escenas/Nivel1.unity";

    [MenuItem("Herramientas/Construir Cazador de Monedas")]
    public static void Construir()
    {
        ConfigurarProyecto();
        CrearCarpetas();
        Sprite cuadrado = CrearSprite("Cuadrado", false);
        Sprite circulo = CrearSprite("Circulo", true);
        CrearEscena(cuadrado, circulo);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[CazadorMonedas] Escena construida en " + RutaEscena);
    }

    // ---------- Proyecto ----------
    static void ConfigurarProyecto()
    {
        EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;

        // Active Input Handling = Both (2)
        var ps = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
        if (ps != null && ps.Length > 0)
        {
            var so = new SerializedObject(ps[0]);
            var p = so.FindProperty("activeInputHandler");
            if (p != null && p.intValue == 1) // 1 = solo Input System nuevo
            {
                p.intValue = 2;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.LogWarning("[CazadorMonedas] Active Input Handling cambiado a Both. " +
                                 "Reinicia el editor si Unity lo solicita.");
            }
        }
        AsegurarTag("Moneda");
    }

    static void AsegurarTag(string tag)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        var tm = new SerializedObject(assets[0]);
        var tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedPropertiesWithoutUndo();
    }

    static void CrearCarpetas()
    {
        foreach (var c in new[] { "Assets/Escenas", "Assets/Scripts", "Assets/Sprites", "Assets/Editor" })
            if (!AssetDatabase.IsValidFolder(c))
                AssetDatabase.CreateFolder("Assets", Path.GetFileName(c));
    }

    // ---------- Sprites (equivalen a Square y Circle de 2D Object) ----------
    static Sprite CrearSprite(string nombre, bool circulo)
    {
        string ruta = CarpetaSprites + "/" + nombre + ".png";
        if (!File.Exists(ruta))
        {
            const int n = 64;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            float r = n / 2f;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = x + 0.5f - r, dy = y + 0.5f - r;
                    bool dentro = !circulo || (dx * dx + dy * dy <= r * r);
                    tex.SetPixel(x, y, dentro ? Color.white : new Color(1, 1, 1, 0));
                }
            tex.Apply();
            File.WriteAllBytes(ruta, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceSynchronousImport);
        }
        var imp = (TextureImporter)AssetImporter.GetAtPath(ruta);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = 64;   // 64 px = 1 unidad → sprite de 1×1
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    // ---------- Escena ----------
    static void CrearEscena(Sprite cuadrado, Sprite circulo)
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Cámara
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        camGO.AddComponent<AudioListener>();
        camGO.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.13f, 0.25f);

        var azul = new Color(0f, 0.5f, 1f);
        var gris = new Color(0.55f, 0.58f, 0.65f);
        var amarillo = new Color(1f, 0.85f, 0.2f);

        // Jugador
        var jugador = CrearSpriteObj("Jugador", cuadrado, new Vector3(0, 0, 0), new Vector3(0.7f, 0.7f, 1f), azul);
        var rb = jugador.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        var bcJ = jugador.AddComponent<BoxCollider2D>();
        bcJ.size = Vector2.one;
        bcJ.isTrigger = false;
        jugador.AddComponent<CazadorMonedas>(); // Velocidad = 4, Meta = 5 (valores por defecto)

        // Paredes (tamaño por Scale, collider 1×1)
        CrearPared("ParedSuperior", cuadrado, gris, new Vector3(0, 4, 0), new Vector3(14f, 0.4f, 1f));
        CrearPared("ParedInferior", cuadrado, gris, new Vector3(0, -4, 0), new Vector3(14f, 0.4f, 1f));
        CrearPared("ParedIzquierda", cuadrado, gris, new Vector3(-7, 0, 0), new Vector3(0.4f, 8f, 1f));
        CrearPared("ParedDerecha", cuadrado, gris, new Vector3(7, 0, 0), new Vector3(0.4f, 8f, 1f));

        // Monedas
        CrearMoneda("Moneda1", circulo, amarillo, new Vector3(-4, 2, 0));
        CrearMoneda("Moneda2", circulo, amarillo, new Vector3(0, 2, 0));
        CrearMoneda("Moneda3", circulo, amarillo, new Vector3(4, 2, 0));
        CrearMoneda("Moneda4", circulo, amarillo, new Vector3(-3, -2, 0));
        CrearMoneda("Moneda5", circulo, amarillo, new Vector3(3, -2, 0));

        EditorSceneManager.SaveScene(escena, RutaEscena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(RutaEscena, true) };

        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.in2DMode = true;
    }

    static GameObject CrearSpriteObj(string nombre, Sprite sprite, Vector3 pos, Vector3 escala, Color color)
    {
        var go = new GameObject(nombre);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = escala;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = 0;
        return go;
    }

    static void CrearPared(string nombre, Sprite sprite, Color color, Vector3 pos, Vector3 escala)
    {
        var go = CrearSpriteObj(nombre, sprite, pos, escala, color);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = Vector2.one;
        bc.isTrigger = false;      // pared sólida; sin Rigidbody2D
    }

    static void CrearMoneda(string nombre, Sprite sprite, Color color, Vector3 pos)
    {
        var go = CrearSpriteObj(nombre, sprite, pos, new Vector3(0.45f, 0.45f, 1f), color);
        go.tag = "Moneda";
        var cc = go.AddComponent<CircleCollider2D>();
        cc.radius = 0.5f;
        cc.isTrigger = true;       // trigger; sin Rigidbody2D
    }

    // ---------- Validación (ejecutable por línea de comandos) ----------
    public static void Validar()
    {
        var escena = EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Single);
        int errores = 0;
        void Chk(bool ok, string msg) { if (!ok) { errores++; Debug.LogError("[VALIDAR] FALLA: " + msg); } }

        var cam = Camera.main;
        Chk(cam != null && cam.orthographic && Mathf.Approximately(cam.orthographicSize, 5f), "Cámara ortográfica size 5");

        var j = GameObject.Find("Jugador");
        Chk(j != null, "Existe Jugador");
        if (j != null)
        {
            var rb = j.GetComponent<Rigidbody2D>();
            Chk(rb != null && rb.bodyType == RigidbodyType2D.Dynamic && rb.gravityScale == 0f && rb.linearDamping == 0f, "Rigidbody2D Dynamic, gravedad 0, damping 0");
            Chk(rb != null && (rb.constraints & RigidbodyConstraints2D.FreezeRotation) != 0, "Freeze Rotation Z");
            var bc = j.GetComponent<BoxCollider2D>();
            Chk(bc != null && !bc.isTrigger, "BoxCollider2D no trigger");
            Chk(j.GetComponent<CazadorMonedas>() != null, "Script en Jugador");
        }

        foreach (var n in new[] { "ParedSuperior", "ParedInferior", "ParedIzquierda", "ParedDerecha" })
        {
            var p = GameObject.Find(n);
            Chk(p != null, n + " existe");
            if (p != null)
            {
                var bc = p.GetComponent<BoxCollider2D>();
                Chk(bc != null && !bc.isTrigger, n + " BoxCollider2D sólido");
                Chk(p.GetComponent<Rigidbody2D>() == null, n + " sin Rigidbody2D");
            }
        }

        var monedas = GameObject.FindGameObjectsWithTag("Moneda");
        Chk(monedas.Length == 5, "Exactamente 5 monedas con tag Moneda (hay " + monedas.Length + ")");
        foreach (var m in monedas)
        {
            var cc = m.GetComponent<CircleCollider2D>();
            Chk(cc != null && cc.isTrigger, m.name + " CircleCollider2D trigger");
            Chk(m.GetComponent<Rigidbody2D>() == null, m.name + " sin Rigidbody2D");
        }

        Debug.Log(errores == 0 ? "[VALIDAR] OK: la escena cumple la especificación." : "[VALIDAR] " + errores + " error(es).");
        if (Application.isBatchMode) EditorApplication.Exit(errores == 0 ? 0 : 1);
    }
}
#endif
```

---

## 8. Ejecución

**Desde el editor (recomendado):** abrir el proyecto → menú **Herramientas → Construir Cazador de Monedas**
→ abrir `Assets/Escenas/Nivel1.unity` → **Play**.

**Desde línea de comandos** (con Unity cerrado y licencia activa; ajustar la ruta del editor):

```bash
# Construir
"<ruta>/Unity" -batchmode -nographics -quit -projectPath "<ruta>/CazadorMonedas" \
  -executeMethod ConstruirCazadorMonedas.Construir -logFile -

# Validar estructura de la escena
"<ruta>/Unity" -batchmode -nographics -projectPath "<ruta>/CazadorMonedas" \
  -executeMethod ConstruirCazadorMonedas.Validar -logFile -
```

Rutas típicas: Windows `C:\Program Files\Unity\Hub\Editor\<versión>\Editor\Unity.exe`,
macOS `/Applications/Unity/Hub/Editor/<versión>/Unity.app/Contents/MacOS/Unity`.
Si el cambio de *Active Input Handling* pide reinicio, ejecutar el comando de construcción **dos veces**
(la segunda ya con el ajuste cargado) y abrir el editor de nuevo.

---

## 9. Criterios de aceptación (checklist)

Estructura y compilación
- [ ] Existe `Assets/Escenas/Nivel1.unity`, `Assets/Scripts/CazadorMonedas.cs` y la escena está en Build Settings.
- [ ] La consola de Unity no tiene errores rojos.
- [ ] `Validar` termina con `OK`.

Comportamiento (probar con Play; hacer clic dentro de Game para dar foco al teclado)

| Prueba | Resultado esperado |
|---|---|
| Movimiento | Responde a flechas **y** WASD; no cae ni gira; diagonal no es más rápida. |
| Paredes | No atraviesa ninguna de las 4 paredes. |
| Monedas | Cada moneda desaparece y suma **una sola vez**. |
| Victoria | Contador `5 / 5`, mensaje de victoria y el jugador queda detenido. |
| Nueva partida | Detener Play y volver a iniciar: reaparecen las 5 monedas. |
| HUD | Texto blanco, tamaño 24, arriba a la izquierda, legible sobre el fondo oscuro. |

---

## 10. Problemas frecuentes

| Problema | Qué revisar |
|---|---|
| No se mueve | Foco en Game, script en `Jugador`, Velocidad > 0, sin errores rojos. |
| Error de Input (`InvalidOperationException`) | Active Input Handling = Both o Old; reiniciar editor. |
| Cae o gira | Gravity Scale = 0 y Freeze Rotation Z. |
| Atraviesa paredes | Box Collider 2D en jugador y paredes, Is Trigger apagado, jugador Dynamic y Simulated. |
| Monedas no suman | Tag = `Moneda`, Circle Collider 2D con Is Trigger, Rigidbody 2D en Jugador. |
| `linearVelocity` no existe | No es Unity 6, o se usa `Rigidbody` 3D. |
| `NullReferenceException` | Rigidbody2D y script deben estar en el mismo objeto `Jugador`. |
| No aparece la victoria | Debe haber exactamente 5 monedas etiquetadas y Meta = 5. |
| Sprites rosas/ausentes (URP) | Usar el material de sprite por defecto del pipeline; no cambiar shaders manualmente. |

---

## 11. Retos opcionales (solo si el usuario los pide)

1. Cambiar colores/posiciones manteniendo las monedas dentro del escenario.
2. Añadir 3 monedas más en lugares accesibles y poner **Meta = 8** en el Inspector de `Jugador`.
3. Crear un obstáculo: Square gris + Box Collider 2D con Is Trigger **desactivado** y **sin** Rigidbody 2D; debe poder rodearse.

Preguntas de comprensión que puede responder:
- *Pared vs moneda*: la pared es collider sólido (bloquea); la moneda es trigger (detecta sin bloquear).
- *Si Meta > nº de monedas*: nunca se gana (el contador no llega a la meta).
- *¿Por qué el script va en Jugador?*: porque ahí está el Rigidbody2D que se mueve y que detecta los triggers.
- *¿Qué campo cambia la velocidad?*: `Velocidad` en el componente Cazador Monedas del Inspector.

---

## 12. Fuera de alcance (no hacer)

- Sonido, partículas, animaciones, menús, botón de reinicio, Canvas/TextMeshPro, Input System nuevo.
- Compilar/exportar el juego (Build Profiles) salvo petición explícita.
- Modificar valores de las tablas de la sección 5 o renombrar objetos, carpetas, tag o clase.
- Subir a control de versiones `Library/`, `Temp/`, `Logs/`, `obj/` (usar el `.gitignore` estándar de Unity).

Para conservar el proyecto se entrega **la carpeta completa del proyecto** (escena y script por separado
no incluyen todos los ajustes), pudiendo excluir `Library` y `Temp`.
