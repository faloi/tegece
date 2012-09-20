using System;
using System.Collections.Generic;
using System.Text;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploTanque : TgcExample
    {
        private TgcBox surface;
        private TgcMesh tank;
        private bool tankMoving;
        private float moveForward;

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

            Microsoft.DirectX.Direct3D.Device d3DDevice = GuiController.Instance.D3dDevice;

            //Agrego UserVars para la posicion del tanque
            UserVars.addMany(
                "posX",
                "posY",
                "posZ"
            );

            //Agrego Modificador de Velocidad de Movimiento
            GuiController.Instance.Modifiers.addFloat("tankVelocity", 0f, 1000f, 100f);

            //Agrego Modificador de Offset de 3rd Person Camera

            GuiController.Instance.Modifiers.addFloat("cameraOffsetHeight", 0, 300, 200);
            GuiController.Instance.Modifiers.addFloat("cameraOffsetForward", 0, 400, 300);
  
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;


            //Cargo la superficie y su textura
            TgcTexture surfaceTexture = TgcTexture.createTexture(d3DDevice, alumnoMediaFolder + "RestrictedGL\\#TankExample\\Textures\\tierra.jpg");
            surface = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(500, 0, 500), surfaceTexture);

            //Cargo el Tanque
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            tank = scene.Meshes[0];

            //Seteo la Camara en 3ra Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(tank.Position, Modifiers.get<float>("cameraOffsetHeight"), Modifiers.get<float>("cameraOffsetForward"));
        }

        public override void close() {
            surface.dispose();
            tank.dispose();
        }

        public override void render(float elapsedTime) {
            TgcD3dInput d3DInput = GuiController.Instance.D3dInput;
            tankMoving = false;

            //Actualizo Modifiers de Camara

            TgcThirdPersonCamera camera = GuiController.Instance.ThirdPersonCamera;
            camera.OffsetHeight = (float)GuiController.Instance.Modifiers["cameraOffsetHeight"];
            camera.OffsetForward = (float)GuiController.Instance.Modifiers["cameraOffsetForward"];

            //Adelante
            if (d3DInput.keyDown(Key.W))
            {
                moveForward = -Modifiers.get<float>("tankVelocity"); 
                tankMoving = true;
            }

            //Atras
            if (d3DInput.keyDown(Key.S)) {
                moveForward = Modifiers.get<float>("tankVelocity");
                tankMoving = true;
            }
            if(tankMoving) {

                //Muevo el tanque
                tank.moveOrientedY(elapsedTime*moveForward);

                //Actualizo UserVars del tanque
                GuiController.Instance.UserVars["posX"] = tank.Position.X.ToString();
                GuiController.Instance.UserVars["posY"] = tank.Position.Y.ToString();
                GuiController.Instance.UserVars["posZ"] = tank.Position.Z.ToString();
               
            }

            //Muevo la camara
            camera.OffsetForward = tank.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            //Renderizo Objetos de la Escena
            surface.render();
            tank.render();
        }
    }
}
