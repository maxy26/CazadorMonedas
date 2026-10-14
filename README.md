# Cazador de Monedas

Juego 2D sencillo hecho en **Unity 6**. Mueve el cuadrado azul por el recinto y recoge las
**5 monedas amarillas** para ganar.

## Controles

| Tecla | Acción |
|---|---|
| Flechas o WASD | Mover al jugador |
| Esc | Pausar / reanudar |
| Ratón | Pulsar los botones de los menús |

## Pantallas

- **Menú principal**: Jugar o Salir.
- **Juego**: contador `Monedas: X / 5` arriba a la izquierda.
- **Pausa** (Esc): Continuar, Reiniciar o volver al Menú principal.
- **Fin**: al recoger las 5 monedas aparece `¡Ganaste!` y las opciones Reiniciar o Menú principal.

## Requisitos

- [Unity Hub](https://unity.com/download) con el editor **Unity 6000.4.8f1**.
- Plantilla Universal 2D (URP). No necesita paquetes adicionales.

## Cómo abrirlo y jugar

1. Clona el repositorio:
   ```
   git clone https://github.com/maxy26/CazadorMonedas.git
   ```
2. En Unity Hub: **Add → Add project from disk** y elige la carpeta clonada.
   La primera vez tarda un poco porque Unity regenera la carpeta `Library`.
3. Abre la escena `Assets/Escenas/Menu.unity` y pulsa **Play**.
4. Haz clic dentro de la ventana **Game** para que reciba el teclado.

## Estructura

```
Assets/
├─ Escenas/
│  ├─ Menu.unity                 Menú principal (primera escena)
│  └─ Nivel1.unity               Nivel con el jugador, paredes y monedas
├─ Scripts/
│  ├─ CazadorMonedas.cs          Movimiento, recogida de monedas y HUD
│  ├─ ControlPartida.cs          Pausa, fin, reiniciar e ir al menú
│  └─ MenuPrincipal.cs           Botones Jugar y Salir
├─ Editor/
│  └─ ConstruirCazadorMonedas.cs Genera las escenas por código y las valida
└─ Sprites/                      Cuadrado y círculo blancos (64×64)
```

## Regenerar las escenas

Las escenas se crean por código. Si se rompen o quieres empezar limpio:

- **Desde el editor**: menú **Herramientas → Construir Cazador de Monedas**.
- **Por línea de comandos** (con Unity cerrado):
  ```
  "C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe" -batchmode -nographics -quit -projectPath "<ruta>\CazadorMonedas" -executeMethod ConstruirCazadorMonedas.Construir -logFile <log>
  ```
  Para comprobar que todo está bien, usa `ConstruirCazadorMonedas.Validar` (debe terminar con `[VALIDAR] OK`).

## Notas técnicas

- Entrada con el **Input Manager clásico** (`Active Input Handling = Both`).
- Física 2D (`Rigidbody2D`, colliders 2D); la interfaz usa **UGUI** (Canvas).
- La especificación completa del proyecto está en [`CONTEXTO.md`](CONTEXTO.md).
