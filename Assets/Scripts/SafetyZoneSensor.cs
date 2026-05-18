using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics; 
using Bhaptics.SDK2;

public class SafetyZoneSensor : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Material safeMaterial;    // Material Verde
    public Material dangerMaterial;  // Material Rojo
    private MeshRenderer myRenderer;
    
    private HashSet<Collider> objetosDentro = new HashSet<Collider>();

    [Header("Configuración bHaptics")]
    public string eventoManoDerecha = "right_hand_warning";
    public string eventoManoIzquierda = "left_hand_warning";

    void Start() 
    {
        myRenderer = GetComponent<MeshRenderer>();
        myRenderer.material = safeMaterial; 
    }

    // Usamos el Update para vigilar la zona en tiempo real (por si las manos desaparecen de golpe)
    void Update()
    {
        // Solo gastamos recursos si hay cosas en la lista
        if (objetosDentro.Count > 0)
        {
            ActualizarEstado();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            // Añadimos el objeto a nuestra lista de asistencia
            objetosDentro.Add(other);
            ActualizarEstado(); 

            // --- LÓGICA BHAPTICS ---
            bool esManoDerecha = false;
            bool esManoIzquierda = false;
            Transform ancestroActual = other.transform;

            while (ancestroActual != null)
            {
                string nombre = ancestroActual.name.ToLower();
                if (nombre.Contains("right")) { esManoDerecha = true; break; }
                else if (nombre.Contains("left")) { esManoIzquierda = true; break; }
                ancestroActual = ancestroActual.parent;
            }

            if (esManoDerecha)
            {
                BhapticsLibrary.Play(eventoManoDerecha);
            }
            else if (esManoIzquierda)
            {
                BhapticsLibrary.Play(eventoManoIzquierda);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            // Tachamos el objeto de la lista cuando sale físicamente
            if (objetosDentro.Remove(other))
            {
                ActualizarEstado();

                // 1. Averiguamos de qué lado era la parte que acaba de salir
                bool esManoDerecha = false;
                bool esManoIzquierda = false;
                Transform ancestroActual = other.transform;

                while (ancestroActual != null)
                {
                    string nombre = ancestroActual.name.ToLower();
                    if (nombre.Contains("right")) { esManoDerecha = true; break; }
                    else if (nombre.Contains("left")) { esManoIzquierda = true; break; }
                    ancestroActual = ancestroActual.parent;
                }

                // 2. Magia independiente: Verificamos si la mano ha salido por completo
                if (esManoDerecha && !QuedaManoDentro("right"))
                {
                    // Ha salido un trozo derecho y ya no queda NADA derecho dentro
                    BhapticsLibrary.StopByEventId(eventoManoDerecha);
                }
                else if (esManoIzquierda && !QuedaManoDentro("left"))
                {
                    // Ha salido un trozo izquierdo y ya no queda NADA izquierdo dentro
                    BhapticsLibrary.StopByEventId(eventoManoIzquierda);
                }
            }
        }
    }

    // El cerebro del sistema: Evalúa la lista y decide el color
    private void ActualizarEstado()
    {
        // MAGIA PURA: Borramos de la lista cualquier objeto que haya sido destruido o desactivado por perder tracking
        objetosDentro.RemoveWhere(colisionador => colisionador == null || !colisionador.gameObject.activeInHierarchy);

        // Si después de limpiar la lista sigue habiendo algo dentro... es que hay peligro real
        if (objetosDentro.Count > 0)
        {
            myRenderer.material = dangerMaterial;
        }
        else
        {
            // Lista completamente vacía
            myRenderer.material = safeMaterial;
            
            // Failsafe de seguridad: apagamos ambas
            BhapticsLibrary.StopByEventId(eventoManoDerecha);
            BhapticsLibrary.StopByEventId(eventoManoIzquierda);
        }
    }

    // Escanea la lista de asistencia buscando si QUEDA ALGUNA parte de ese lado concreto
    private bool QuedaManoDentro(string lado)
    {
        foreach (Collider col in objetosDentro)
        {
            if (col != null)
            {
                Transform ancestroActual = col.transform;
                while (ancestroActual != null)
                {
                    if (ancestroActual.name.ToLower().Contains(lado)) return true;
                    ancestroActual = ancestroActual.parent;
                }
            }
        }
        return false; // No queda nada de ese lado
    }
}