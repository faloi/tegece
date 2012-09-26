using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using AlumnoEjemplos.RestrictedGL.Tank;
using Microsoft.DirectX;
using TgcViewer.Example;
using TgcViewer;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploTanque : TgcExample {

        private List<IAlumnoRenderObject> objectsToRender;

        private void setUpCamera()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(
                new Vector3(0, 0, 0),
                Modifiers.get<float>("cameraOffsetHeight"),
                Modifiers.get<float>("cameraOffsetForward"));
        }

        private void addModifiers()
        {
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.Modifiers.addFloat("tankVelocity", 0f, 1000f, 100f);

            GuiController.Instance.Modifiers.addFloat("cameraOffsetHeight", 0, 300, 200);
            GuiController.Instance.Modifiers.addFloat("cameraOffsetForward", 0, 400, 300);
        }

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
            this.objectsToRender = new List<IAlumnoRenderObject> { new Tank.Tank(), new TankTerrain() };
            
            var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            this.objectsToRender.ForEach(o => o.init(alumnoMediaFolder));

            UserVars.addMany("posX", "posY", "posZ");

            this.addModifiers();
            this.setUpCamera();
        }

        public override void close() {
            this.objectsToRender.ForEach(o => o.dispose());
        }

        public override void render(float elapsedTime) {
            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.OffsetHeight = Modifiers.get<float>("cameraOffsetHeight");
            camera.OffsetForward = Modifiers.get<float>("cameraOffsetForward");

            this.objectsToRender.ForEach(o => o.render(elapsedTime));
        }
    }
}
