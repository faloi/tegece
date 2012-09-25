using System;
using System.Collections.Generic;
using System.Text;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RestrictedGL{
    class Tank{
        private TgcMesh tankMesh;
        private bool moving;
        private bool rotating;
        private float linearMovement;
        private float rotationMovement;
        public Vector3 Position { set; get; }
        
        public void init(string alumnoMediaFolder) {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            tankMesh = scene.Meshes[0];
        }

        public void render(float elapsedTime){
            TgcD3dInput d3DInput = GuiController.Instance.D3dInput;

            moving = false;
            rotating = false;
            //Adelante
            if (d3DInput.keyDown(Key.W))
            {
                linearMovement = -Modifiers.get<float>("tankVelocity");
                moving = true;
            }

            //Atras
            if (d3DInput.keyDown(Key.S))
            {
                linearMovement = Modifiers.get<float>("tankVelocity");
                moving = true;
            }
            //Derecha
            if (d3DInput.keyDown(Key.D))
            {
                rotationMovement = Modifiers.get<float>("tankVelocity");
                rotating = true;
            }
            //Izquierda
            if (d3DInput.keyDown(Key.A))
            {
                rotationMovement = -Modifiers.get<float>("tankVelocity");
                rotating = true;
            }
            if (moving)
            {
                //Muevo el tanque
                tankMesh.moveOrientedY(elapsedTime * linearMovement);
            }
            if (rotating)
            {
                float rotAngle = Geometry.DegreeToRadian(rotationMovement * elapsedTime);
                tankMesh.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }
            //Actualizo UserVars del tanque
            GuiController.Instance.UserVars["posX"] = tankMesh.Position.X.ToString();
            GuiController.Instance.UserVars["posY"] = tankMesh.Position.Y.ToString();
            GuiController.Instance.UserVars["posZ"] = tankMesh.Position.Z.ToString();

            //Muevo la camara
            TgcThirdPersonCamera camera = GuiController.Instance.ThirdPersonCamera;
            camera.Target = tankMesh.Position;
            camera.OffsetForward = tankMesh.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            //Renderizo el Mesh
            tankMesh.render();

            bool showBoundingBox = (bool) GuiController.Instance.Modifiers["showBoundingBox"];
            if(showBoundingBox)
                tankMesh.BoundingBox.render();
        }
        public void dispose() {
            tankMesh.dispose();
        }
    }
}
