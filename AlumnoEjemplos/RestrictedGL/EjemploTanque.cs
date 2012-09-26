using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Tank;
using TgcViewer.Example;
using TgcViewer;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploTanque : TgcExample {

        private TankTerrain terrain;
        private Tank.Tank tank;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "RestrictedGL.Tanque";
        }

        public override string getDescription()
        {
            return "Ejemplo del tanque sobre una superficie plana";
        }

        public override void init() {
            this.tank = new Tank.Tank();
            this.terrain = new TankTerrain();

            UserVars.addMany("posX", "posY", "posZ");

            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.Modifiers.addFloat("tankVelocity", 0f, 1000f, 100f);

            GuiController.Instance.Modifiers.addFloat("cameraOffsetHeight", 0, 300, 200);
            GuiController.Instance.Modifiers.addFloat("cameraOffsetForward", 0, 400, 300);
  
            var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

            this.terrain.init(alumnoMediaFolder);
            this.tank.init(alumnoMediaFolder);

            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(tank.position, Modifiers.get<float>("cameraOffsetHeight"), Modifiers.get<float>("cameraOffsetForward"));
        }

        public override void close() {
            this.terrain.dispose();
            this.tank.dispose();
        }

        public override void render(float elapsedTime) {
            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.OffsetHeight = Modifiers.get<float>("cameraOffsetHeight");
            camera.OffsetForward = Modifiers.get<float>("cameraOffsetForward");
          
            this.terrain.render();
            this.tank.render(elapsedTime);
        }
    }
}
