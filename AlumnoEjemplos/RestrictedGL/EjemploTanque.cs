using System;
using System.Collections.Generic;
using System.Text;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer.Example;
using TgcViewer;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploTanque : TgcExample {

        private TankTerrain terrain;
        private Tank tank;

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
            //Instancio Elementos del Contexto
            tank = new Tank();
            terrain = new TankTerrain();

            //Agrego UserVars para la posicion del tanque
            UserVars.addMany(
                "posX",
                "posY",
                "posZ"
            );
            //Agrego Modificador para Mostrar BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);

            //Agrego Modificador de Velocidad de Movimiento
            GuiController.Instance.Modifiers.addFloat("tankVelocity", 0f, 1000f, 100f);

            //Agrego Modificador de Offset de 3rd Person Camera

            GuiController.Instance.Modifiers.addFloat("cameraOffsetHeight", 0, 300, 200);
            GuiController.Instance.Modifiers.addFloat("cameraOffsetForward", 0, 400, 300);
  
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;


            //Cargo la superficie y su textura
            terrain.init(alumnoMediaFolder);
            //Cargo el Tanque
            tank.init(alumnoMediaFolder);

            //Seteo la Camara en 3ra Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(tank.Position, Modifiers.get<float>("cameraOffsetHeight"), Modifiers.get<float>("cameraOffsetForward"));
        }

        public override void close() {
            terrain.dispose();
            tank.dispose();
        }

        public override void render(float elapsedTime) {
                     
            //Actualizo Modifiers de Camara

            TgcThirdPersonCamera camera = GuiController.Instance.ThirdPersonCamera;
            camera.OffsetHeight = (float)GuiController.Instance.Modifiers["cameraOffsetHeight"];
            camera.OffsetForward = (float)GuiController.Instance.Modifiers["cameraOffsetForward"];

          
            //Renderizo Objetos de la Escena
            terrain.render();
            tank.render(elapsedTime);
        }
    }
}
