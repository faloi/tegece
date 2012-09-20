using System;
using System.Collections.Generic;
using System.Text;
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
        private float velocityFoward = 100f;

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
            GuiController.Instance.ThirdPersonCamera.setCamera(tank.Position, 200, 300);
        }

        public override void close() {
            surface.dispose();
            tank.dispose();
        }

        public override void render(float elapsedTime) {
            TgcD3dInput d3DInput = GuiController.Instance.D3dInput;
            tankMoving = false;
            //Adelante
            if (d3DInput.keyDown(Key.W))
            {
                moveForward = -velocityFoward;
                tankMoving = true;
            }

            //Atras
            if (d3DInput.keyDown(Key.S))
            {
                moveForward = velocityFoward;
                tankMoving = true;
            }
            if(tankMoving) {
                tank.moveOrientedY(elapsedTime*moveForward);
            }
            surface.render();
            tank.render();
        }
    }
}
