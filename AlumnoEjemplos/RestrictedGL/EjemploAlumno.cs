using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploAlumno : TgcExample
    {
        string mediaFolder;
        Heightmap terreno;

        #region Descripciones
            /// <summary>
            /// Categoría a la que pertenece el ejemplo.
            /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
            /// </summary>
            public override string getCategory() {
                return "AlumnoEjemplos";
            }

            /// <summary>
            /// Completar nombre del grupo en formato Grupo NN
            /// </summary>
            public override string getName() {
                return "RestrictedGL";
            }

            /// <summary>
            /// Completar con la descripción del TP
            /// </summary>
            public override string getDescription() {
                return "SuperMegaTanque";
            }
        #endregion

        /// <summary>Código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.</summary>
        public override void init() {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            Device d3dDevice = GuiController.Instance.D3dDevice; //Device de DirectX para crear primitivas
            mediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir + "RestrictedGL\\";
            #region Tutorial
                /*
                //---USER VARS---
                //Crear una UserVar
                GuiController.Instance.UserVars.addVar("variablePrueba");

                //Cargar valor en UserVar
                GuiController.Instance.UserVars.setValue("variablePrueba", 5451);

                //---MODIFIERS---
                //Crear un modifier para un valor FLOAT
                GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);

                //Crear un modifier para un ComboBox con opciones
                string[] opciones = new string[] { "opcion1", "opcion2", "opcion3" };
                GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);

                //Crear un modifier para modificar un vértice
                GuiController.Instance.Modifiers.addVertex3f("valorVertice", new Vector3(-100, -100, -100), new Vector3(50, 50, 50), new Vector3(0, 0, 0));

                //---CONFIGURAR CÁMARA ROTACIONAL---
                //Es la camara que viene por default, asi que no hace falta hacerlo siempre
                GuiController.Instance.RotCamera.Enable = true;
                //Configurar centro al que se mira y distancia desde la que se mira
                GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);

                //---CONFIGURAR CAMARA PRIMERA PERSONA---
                //Camara en primera persona, tipo videojuego FPS
                //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
                //Por default la camara FPS viene desactivada
                //GuiController.Instance.FpsCamera.Enable = true;
                //Configurar posicion y hacia donde se mira
                //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
                //CÁMARA TERCERA PERSONA: GuiController.Instance.ThirdPersonCamera.Enable = true;

                //---LISTAS EN C#---
                //crear
                List<string> lista = new List<string>();

                //agregar elementos
                lista.Add("elemento1");
                lista.Add("elemento2");

                //obtener elementos
                string elemento1 = lista[0];

                //bucle foreach
                foreach (string elemento in lista) {
                    //Loggear por consola del Framework
                    GuiController.Instance.Logger.log(elemento);
                }

                //bucle for
                for (int i = 0; i < lista.Count; i++) {
                    string element = lista[i];
                }
                */
            #endregion

            //Cargar terreno:
            terreno.heightmap = mediaFolder + "Heightmap.jpg";
            terreno.textura = mediaFolder + "Mapa.jpg";
            terreno.scaleXZ = 100f;
            terreno.scaleY = 2.5750f;
            terreno.map = new TgcSimpleTerrain();
            terreno.map.loadHeightmap(terreno.heightmap, terreno.scaleXZ, terreno.scaleY, new Vector3(0, 0, 0));
            terreno.map.loadTexture(terreno.textura);

            //Configurar FPS Camara:
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 200f;
            GuiController.Instance.FpsCamera.JumpSpeed = 200f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-722.6171f, 495.0046f, -31.2611f), new Vector3(164.9481f, 35.3185f, -61.5394f));

            //Agregar modificadores de escala:
            GuiController.Instance.Modifiers.addFloat("scaleXZ", 0.1f, 100f, terreno.scaleXZ);
            GuiController.Instance.Modifiers.addFloat("scaleY", 0.1f, 10f, terreno.scaleY);
        }

        ///<summary>Se llama cada vez que hay que refrescar la pantalla</summary>
        ///<param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime) {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            #region Tutorial
                /*
                //Obtener valor de UserVar (hay que castear)
                int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");

                //Obtener valores de Modifiers
                float valorFloat = (float)GuiController.Instance.Modifiers["valorFloat"];
                string opcionElegida = (string)GuiController.Instance.Modifiers["valorIntervalo"];
                Vector3 valorVertice = (Vector3)GuiController.Instance.Modifiers["valorVertice"];

                //---INPUT---
                //conviene deshabilitar ambas camaras para que no haya interferencia

                //Capturar Input teclado
                if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F)) {
                    //Tecla F apretada
                }

                //Capturar Input Mouse
                if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT)) {
                    //Boton izq apretado
                }
                */
            #endregion

            float scaleXZModificado = (float)GuiController.Instance.Modifiers["scaleXZ"];
            float scaleYModificado = (float)GuiController.Instance.Modifiers["scaleY"];
            if (terreno.scaleXZ != scaleXZModificado || terreno.scaleY != scaleYModificado) {
                terreno.map.loadHeightmap(terreno.heightmap, scaleXZModificado, scaleYModificado, new Vector3(0, 0, 0));
            }
            terreno.map.render();
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            
        }

    }

    struct Heightmap
    {
        public TgcSimpleTerrain map;
        public string heightmap; //path al heightmap
        public string textura; //path a la textura
        public float scaleXZ;
        public float scaleY;
    }
}