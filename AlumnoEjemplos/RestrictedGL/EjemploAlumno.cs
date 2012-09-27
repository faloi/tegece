using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL
{
    struct Shared
    {
        public const string NombreGrupo = "RestrictedGL";
        public static readonly string MediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir + NombreGrupo + "\\";
    }

    public class EjemploAlumno : TgcExample
    {
        private Terrain.Terrain terrain;

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
                return Shared.NombreGrupo;
            }

            /// <summary>
            /// Completar con la descripción del TP
            /// </summary>
            public override string getDescription() {
                return "SuperMegaTanque";
            }
        #endregion

        private void updateUserVars() {
            UserVars.set("posX", GuiController.Instance.FpsCamera.Position.X);
            UserVars.set("posY", GuiController.Instance.FpsCamera.Position.Y);
            UserVars.set("posZ", GuiController.Instance.FpsCamera.Position.Z);
            UserVars.set("viewX", GuiController.Instance.FpsCamera.LookAt.X);
            UserVars.set("viewY", GuiController.Instance.FpsCamera.LookAt.Y);
            UserVars.set("viewZ", GuiController.Instance.FpsCamera.LookAt.Z);
        }

        /// <summary>Código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.</summary>
        public override void init() {
            this.terrain = new Terrain.Terrain();

            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 100f;
            GuiController.Instance.FpsCamera.JumpSpeed = 100f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-100, 100, 0), new Vector3(490f, 128f, -10f));

            GuiController.Instance.Modifiers.addFloat("Cam Velocity", 0f, 1000f, 100f);

            UserVars.addMany(
                "posX", 
                "posY",
                "posZ",
                "viewX",
                "viewY",
                "viewZ"
            );
        }

        ///<summary>Se llama cada vez que hay que refrescar la pantalla</summary>
        ///<param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime) {
            if (GuiController.Instance.D3dInput.keyDown(Key.R)) {
                //R = Cámara en el origen, más o menos
                GuiController.Instance.FpsCamera.setCamera(new Vector3(-100, 100, 0), new Vector3(490f, 128f, -10f));
            }
            
            GuiController.Instance.FpsCamera.MovementSpeed = 
            GuiController.Instance.FpsCamera.JumpSpeed =
                Modifiers.get<float>("Cam Velocity");
            
            this.updateUserVars();
            this.terrain.render();
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            this.terrain.dispose();
        }

    }
}