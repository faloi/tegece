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
        private Tank.TankPlayer tank;
        private Tank.TankEnemy tankEnemy;

        private string currentCamera;

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
            this.tank = new Tank.TankPlayer(new Vector3(0, tankY + 15, 0), this.terrain, Path.Tank);

            var rand = new Randomizer((int)-this.terrain.heightmapSizeScaled/2 + 500, (int)this.terrain.heightmapSizeScaled/2 - 500);
            this.tankEnemy = new Tank.TankEnemy(new Vector3(rand.getNext(), tankY + 15, rand.getNext()), this.terrain);

            this.tank.enemy = this.tankEnemy;
            this.tankEnemy.enemy = this.tank;

            Gui.I.Modifiers.addFloat("Cam Velocity", 0f, 1000f, 500f);
            Gui.I.Modifiers.addFloat("rotationVelocity", 0f, 1000f, 100f);
            Gui.I.Modifiers.addBoolean("ShowBoundingBox", "Show bounding box", false);
            object[] values = {"Third Person","First Person(Free)" };
            Gui.I.Modifiers.addInterval("Camera", values, 0);
                        

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
                "direction",
                "enemyColliding",
                "enemyAvoiding",
                "enemyTA"
            );

            //Aumentar distancia del far plane
            d3dDevice.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                (float)d3dDevice.CreationParameters.FocusWindow.Width / d3dDevice.CreationParameters.FocusWindow.Height, 1f, 30000f);
            this.setUpCamera();
        }

        protected void setUpCamera() {
            if (Modifiers.get<string>("Camera") == "Third Person"){
                Gui.I.FpsCamera.Enable = false;
                var camera = Gui.I.ThirdPersonCamera;
                camera.Enable = true;
            }else{
                Gui.I.ThirdPersonCamera.Enable = false;
                var camera = Gui.I.FpsCamera;
                camera.Enable = true;
                camera.MovementSpeed = camera.JumpSpeed = Modifiers.get<float>("Cam Velocity");
            }
            this.currentCamera = Modifiers.get<string>("Camera");

        }

        ///<summary>Se llama cada vez que hay que refrescar la pantalla</summary>
        ///<param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime) {
            Shared.ElapsedTime = elapsedTime;
           
            this.updateUserVars();

            this.tank.render();
            this.tankEnemy.render();
            this.terrain.render();

            if (Modifiers.get<string>("Camera") != this.currentCamera)
                this.setUpCamera();
            if (Modifiers.get<string>("Camera") == "Third Person"){
                var camera = Gui.I.ThirdPersonCamera;
                camera.setCamera(tank.Position,900f, 1300f);
            }
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            this.terrain.dispose();
        }

    }
}