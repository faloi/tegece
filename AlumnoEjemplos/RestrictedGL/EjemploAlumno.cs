using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Example;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;

namespace AlumnoEjemplos.RestrictedGL
{
    struct Shared
    {
        public const string NombreGrupo = "RestrictedGL";
        public static readonly string MediaFolder = Gui.I.AlumnoEjemplosMediaDir + NombreGrupo + "\\";
        public static float ElapsedTime = 0;
    }

    public class EjemploAlumno : TgcExample
    {
        private Terrain.Terrain terrain;
        private Tank.Tank tank;
        private Tank.TankEnemy tankEnemy;

        private const float SCALE_Y = 2f;
        private const float SCALE_XZ = 50f;
        
        public EjemploAlumno() {

        }

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
                return "Tenés que matar al tanque enemigo: Flechitas = Moverse; Ctrl Derecho = Disparar";
            }
        #endregion

        private void updateUserVars() {
            UserVars.set("posX", Gui.I.FpsCamera.Position.X);
            UserVars.set("posX", Gui.I.FpsCamera.Position.X);
            UserVars.set("posY", Gui.I.FpsCamera.Position.Y);
            UserVars.set("posZ", Gui.I.FpsCamera.Position.Z);
            UserVars.set("viewX", Gui.I.FpsCamera.LookAt.X);
            UserVars.set("viewY", Gui.I.FpsCamera.LookAt.Y);
            UserVars.set("viewZ", Gui.I.FpsCamera.LookAt.Z);
        }

        /// <summary>Código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.</summary>
        public override void init() {
            var d3dDevice = Gui.I.D3dDevice;

            this.terrain = new Terrain.Terrain(SCALE_XZ, SCALE_Y);

            var tankY = this.terrain.heightmapData[64, 64] * SCALE_Y;
            this.tank = new Tank.Tank(new Vector3(0, tankY + 15, 0), this.terrain);

            var rand = new Randomizer((int)-this.terrain.heightmapSizeScaled/2 + 500, (int)this.terrain.heightmapSizeScaled/2 - 500);
            this.tankEnemy = new Tank.TankEnemy(new Vector3(rand.getNext(), tankY + 15, rand.getNext()), this.terrain);
            this.tankEnemy.tank = this.tank;

            Gui.I.FpsCamera.Enable = true;
            Gui.I.FpsCamera.MovementSpeed = 100f;
            Gui.I.FpsCamera.JumpSpeed = 100f;
            Gui.I.FpsCamera.setCamera(this.tank.Position + new Vector3(0, 300, 400), this.tank.Position);

            Gui.I.Modifiers.addFloat("Cam Velocity", 0f, 1000f, 500f);
            Gui.I.Modifiers.addFloat("rotationVelocity", 0f, 1000f, 100f);
            Gui.I.Modifiers.addBoolean("ShowBoundingBox", "Show bounding box", false);

            UserVars.addMany(
                "posX", 
                "posY",
                "posZ",
                "viewX",
                "viewY",
                "viewZ",
                "destX",
                "destY",
                "destZ",
                "totalSpeed",
                "direction"
            );

            //Aumentar distancia del far plane
            d3dDevice.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                (float)d3dDevice.CreationParameters.FocusWindow.Width / d3dDevice.CreationParameters.FocusWindow.Height, 1f, 30000f);
        }

        ///<summary>Se llama cada vez que hay que refrescar la pantalla</summary>
        ///<param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime) {
            Shared.ElapsedTime = elapsedTime;

            if (Gui.I.D3dInput.keyDown(Key.R)) { //(test)
                //R = Cámara en el origen, más o menos
                Gui.I.FpsCamera.setCamera(new Vector3(-100, 200, 0), new Vector3(490f, 128f, -10f));
            }
            if (Gui.I.D3dInput.keyDown(Key.T)) { //(test)
                //T = Rotar al enemigo
                terrain.deform(Gui.I.FpsCamera.Position.X, Gui.I.FpsCamera.Position.Z, 150, 1);
            }
            
            Gui.I.FpsCamera.MovementSpeed = 
            Gui.I.FpsCamera.JumpSpeed =
                Modifiers.get<float>("Cam Velocity");
            
            this.updateUserVars();

            this.tank.render();
            this.tankEnemy.render();
            this.terrain.render();
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            this.terrain.dispose();
        }

    }
}