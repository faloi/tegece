using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

namespace AlumnoEjemplos.RestrictedGL
{
    struct Shared //datos comunes a todo
    {
        public static string mediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir + "RestrictedGL\\";
    }

    public class EjemploAlumno : TgcExample
    {
        Terreno terreno;

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

            //Inicializar terreno:
            terreno = new Terreno();

            //Configurar FPS Camara:
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 2000f;
            GuiController.Instance.FpsCamera.JumpSpeed = 2000f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 950, 0), new Vector3(1500f, 700f, -1500f));

            //Agregar modificadores:
            GuiController.Instance.Modifiers.addFloat("centerX", 0f, 10000f, 0);
            GuiController.Instance.Modifiers.addFloat("centerY", 0f, 10000f, 0);
            GuiController.Instance.Modifiers.addFloat("centerZ", 0f, 10000f, 0);
            GuiController.Instance.Modifiers.addFloat("sizeX", 0f, 30000f, 10000f);
            GuiController.Instance.Modifiers.addFloat("sizeY", 0f, 30000f, 10000f);
            GuiController.Instance.Modifiers.addFloat("sizeZ", 0f, 30000f, 10000f);
            GuiController.Instance.Modifiers.addFloat("velcam", 0f, 10000f, 2000f);

            //Agregar uservars de cámara:
            GuiController.Instance.UserVars.addVar("posX");
            GuiController.Instance.UserVars.addVar("posY");
            GuiController.Instance.UserVars.addVar("posZ");
            GuiController.Instance.UserVars.addVar("viewX");
            GuiController.Instance.UserVars.addVar("viewY");
            GuiController.Instance.UserVars.addVar("viewZ");
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

            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.R)) {
                //R = Cámara en el origen, mas o menos
                GuiController.Instance.FpsCamera.setCamera(new Vector3(0,950,0), new Vector3(1500f, 700f, -1500f));
            }

            /*//Actualizar SkyBox:
            Vector3 centerNew = new Vector3(0,0,0);
            Vector3 sizeNew = new Vector3(0,0,0);
            centerNew.X = (float)GuiController.Instance.Modifiers["centerX"];
            centerNew.Y = (float)GuiController.Instance.Modifiers["centerY"];
            centerNew.Z = (float)GuiController.Instance.Modifiers["centerZ"];
            sizeNew.X = (float)GuiController.Instance.Modifiers["sizeX"];
            sizeNew.Y = (float)GuiController.Instance.Modifiers["sizeY"];
            sizeNew.Z = (float)GuiController.Instance.Modifiers["sizeZ"];
            terreno.skyBoxUpdate(centerNew, sizeNew);*/
            
            //Actualizar velocidad cámara:
            GuiController.Instance.FpsCamera.MovementSpeed = (float)GuiController.Instance.Modifiers["velcam"];
            GuiController.Instance.FpsCamera.JumpSpeed = (float)GuiController.Instance.Modifiers["velcam"];
            
            //Actualizar uservars:
            GuiController.Instance.UserVars["posX"] = GuiController.Instance.FpsCamera.Position.X.ToString();
            GuiController.Instance.UserVars["posY"] = GuiController.Instance.FpsCamera.Position.Y.ToString();
            GuiController.Instance.UserVars["posZ"] = GuiController.Instance.FpsCamera.Position.Z.ToString();
            GuiController.Instance.UserVars["viewX"] = GuiController.Instance.FpsCamera.LookAt.X.ToString();
            GuiController.Instance.UserVars["viewY"] = GuiController.Instance.FpsCamera.LookAt.Y.ToString();
            GuiController.Instance.UserVars["viewZ"] = GuiController.Instance.FpsCamera.LookAt.Z.ToString();

            //Renderizar:
            terreno.heightMapRender();
            //terreno.skyBoxRender();
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            
        }

    }
}