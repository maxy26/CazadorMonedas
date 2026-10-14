#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ConstruirCazadorMonedas
{
    const string CarpetaSprites = "Assets/Sprites";
    const string RutaEscena = "Assets/Escenas/Nivel1.unity";
    const string RutaMenu = "Assets/Escenas/Menu.unity";

    static readonly Color ColorFondo = new Color(0.08f, 0.13f, 0.25f);
    static readonly Color ColorBoton = new Color(0f, 0.5f, 1f);

    [MenuItem("Herramientas/Construir Cazador de Monedas")]
    public static void Construir()
    {
        ConfigurarProyecto();
        CrearCarpetas();
        Sprite cuadrado = CrearSprite("Cuadrado", false);
        Sprite circulo = CrearSprite("Circulo", true);
        CrearEscena(cuadrado, circulo);
        CrearEscenaMenu();
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(RutaMenu, true),
            new EditorBuildSettingsScene(RutaEscena, true)
        };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[CazadorMonedas] Escenas construidas en " + RutaMenu + " y " + RutaEscena);
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

    // ---------- Escena Nivel1 ----------
    static void CrearEscena(Sprite cuadrado, Sprite circulo)
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CrearCamara();

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
        var cazador = jugador.AddComponent<CazadorMonedas>(); // Velocidad = 4, Meta = 5 (valores por defecto)

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

        // Interfaz
        var canvas = CrearCanvas();
        var partida = new GameObject("Partida").AddComponent<ControlPartida>();

        // HUD (arriba a la izquierda, blanco, tamaño 24)
        var arribaIzq = new Vector2(0f, 1f);
        cazador.textoContador = CrearTexto("TextoContador", canvas, "Monedas: 0 / 5", 24,
            TextAnchor.UpperLeft, arribaIzq, new Vector2(20, -20), new Vector2(420, 40));
        cazador.textoVictoria = CrearTexto("TextoVictoria", canvas, "¡Ganaste! Recogiste todas las monedas.", 24,
            TextAnchor.UpperLeft, arribaIzq, new Vector2(20, -65), new Vector2(500, 40));
        cazador.textoVictoria.gameObject.SetActive(false);
        cazador.partida = partida;

        // Panel de pausa (pantalla completa oscurecida)
        var pausa = CrearPanel("PanelPausa", canvas, new Color(0f, 0f, 0f, 0.6f), Vector2.zero, true);
        CrearTexto("Titulo", pausa.transform, "Pausa", 48, TextAnchor.MiddleCenter,
            Centro, new Vector2(0, 120), new Vector2(400, 70));
        CrearBoton("BotonContinuar", pausa.transform, "Continuar", new Vector2(0, 30), partida.Continuar);
        CrearBoton("BotonReiniciar", pausa.transform, "Reiniciar", new Vector2(0, -40), partida.Reiniciar);
        CrearBoton("BotonMenu", pausa.transform, "Menú principal", new Vector2(0, -110), partida.IrAlMenu);
        pausa.SetActive(false);

        // Panel de fin (caja centrada; deja ver el HUD)
        var fin = CrearPanel("PanelFin", canvas, new Color(0.1f, 0.15f, 0.3f, 0.92f), new Vector2(420, 260), false);
        CrearTexto("Titulo", fin.transform, "Fin de la partida", 32, TextAnchor.MiddleCenter,
            Centro, new Vector2(0, 75), new Vector2(400, 50));
        CrearBoton("BotonReiniciar", fin.transform, "Reiniciar", new Vector2(0, -5), partida.Reiniciar);
        CrearBoton("BotonMenu", fin.transform, "Menú principal", new Vector2(0, -75), partida.IrAlMenu);
        fin.SetActive(false);

        partida.panelPausa = pausa;
        partida.panelFin = fin;

        EditorSceneManager.SaveScene(escena, RutaEscena);

        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.in2DMode = true;
    }

    // ---------- Escena Menu ----------
    static void CrearEscenaMenu()
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CrearCamara();
        var canvas = CrearCanvas();
        var menu = new GameObject("Menu").AddComponent<MenuPrincipal>();

        CrearTexto("Titulo", canvas, "Cazador de Monedas", 56, TextAnchor.MiddleCenter,
            Centro, new Vector2(0, 120), new Vector2(800, 90));
        CrearBoton("BotonJugar", canvas, "Jugar", new Vector2(0, 0), menu.Jugar);
        CrearBoton("BotonSalir", canvas, "Salir", new Vector2(0, -70), menu.Salir);

        EditorSceneManager.SaveScene(escena, RutaMenu);
    }

    // ---------- Objetos de escena ----------
    static void CrearCamara()
    {
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        camGO.AddComponent<AudioListener>();
        camGO.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ColorFondo;
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

    // ---------- Interfaz (UGUI) ----------
    static readonly Vector2 Centro = new Vector2(0.5f, 0.5f);

    static Transform CrearCanvas()
    {
        var go = new GameObject("Canvas", typeof(RectTransform));
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var escalador = go.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; // tamaño 24 = 24 px, como OnGUI
        go.AddComponent<GraphicRaycaster>();

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
        return go.transform;
    }

    static Font Fuente()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    static RectTransform CrearRect(string nombre, Transform padre, Vector2 ancla, Vector2 pos, Vector2 tamano)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        return rt;
    }

    static Text CrearTexto(string nombre, Transform padre, string contenido, int tamanoFuente,
        TextAnchor alineacion, Vector2 ancla, Vector2 pos, Vector2 tamano)
    {
        var t = CrearRect(nombre, padre, ancla, pos, tamano).gameObject.AddComponent<Text>();
        t.font = Fuente();
        t.text = contenido;
        t.fontSize = tamanoFuente;
        t.color = Color.white;
        t.alignment = alineacion;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    static GameObject CrearPanel(string nombre, Transform padre, Color color, Vector2 tamano, bool pantallaCompleta)
    {
        var rt = CrearRect(nombre, padre, Centro, Vector2.zero, tamano);
        if (pantallaCompleta)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }
        rt.gameObject.AddComponent<Image>().color = color;
        return rt.gameObject;
    }

    static Button CrearBoton(string nombre, Transform padre, string etiqueta, Vector2 pos, UnityAction accion)
    {
        var rt = CrearRect(nombre, padre, Centro, pos, new Vector2(260, 50));
        var img = rt.gameObject.AddComponent<Image>();
        img.color = ColorBoton;
        var boton = rt.gameObject.AddComponent<Button>();
        boton.targetGraphic = img;
        var texto = CrearTexto("Texto", rt, etiqueta, 24, TextAnchor.MiddleCenter, Centro, Vector2.zero, rt.sizeDelta);
        texto.rectTransform.anchorMin = Vector2.zero;
        texto.rectTransform.anchorMax = Vector2.one;
        texto.rectTransform.sizeDelta = Vector2.zero;
        UnityEventTools.AddPersistentListener(boton.onClick, accion);
        return boton;
    }

    // ---------- Validación (ejecutable por línea de comandos) ----------
    public static void Validar()
    {
        EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Single);
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
            var cm = j.GetComponent<CazadorMonedas>();
            Chk(cm != null, "Script en Jugador");
            if (cm != null)
                Chk(cm.textoContador != null && cm.textoVictoria != null && cm.partida != null, "Jugador con HUD y Partida asignados");
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

        var partida = GameObject.Find("Partida");
        var cp = partida != null ? partida.GetComponent<ControlPartida>() : null;
        Chk(cp != null && cp.panelPausa != null && cp.panelFin != null, "ControlPartida con paneles de pausa y fin");
        Chk(GameObject.Find("EventSystem") != null, "EventSystem en Nivel1");

        EditorSceneManager.OpenScene(RutaMenu, OpenSceneMode.Single);
        Chk(GameObject.Find("Menu") != null && GameObject.Find("Menu").GetComponent<MenuPrincipal>() != null, "Menu con MenuPrincipal");
        Chk(GameObject.Find("BotonJugar") != null && GameObject.Find("BotonSalir") != null, "Botones Jugar y Salir");
        Chk(GameObject.Find("EventSystem") != null, "EventSystem en Menu");

        var escenas = EditorBuildSettings.scenes;
        Chk(escenas.Length == 2 && escenas[0].path == RutaMenu && escenas[1].path == RutaEscena, "Build Settings: Menu y luego Nivel1");

        Debug.Log(errores == 0 ? "[VALIDAR] OK: la escena cumple la especificación." : "[VALIDAR] " + errores + " error(es).");
        if (Application.isBatchMode) EditorApplication.Exit(errores == 0 ? 0 : 1);
    }
}
#endif
