using System;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    
    enum Direction {
        Forward,
        Backward,
        Left,
        Right
    }

    public class Tank : IAlumnoRenderObject, ITransformObject {

        private TgcMesh mesh;
        
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;
        
        private float calculateSpeed(Direction direction) {
            var speed = Modifiers.get<float>("tankVelocity");

            return direction == Direction.Backward || direction == Direction.Right
               ? speed
               : -speed;
        }

        private void move(Direction direction) {
            this.linearSpeed = calculateSpeed(direction);
            this.isMoving = true;
        }

        private void rotate(Direction direction) {
            this.rotationSpeed = calculateSpeed(direction);
            this.isRotating = true;
        }

        private void moveAndRotate(float elapsedTime) {
            var d3DInput = GuiController.Instance.D3dInput;

            this.isMoving = false;
            this.isRotating = false;

            if (d3DInput.keyDown(Key.W))
                this.move(Direction.Forward);
            if (d3DInput.keyDown(Key.S))
                this.move(Direction.Backward);
            if (d3DInput.keyDown(Key.D))
                this.rotate(Direction.Right);
            if (d3DInput.keyDown(Key.A))
                this.rotate(Direction.Left);

            if (this.isMoving)
                this.moveOrientedY(elapsedTime * this.linearSpeed);

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(elapsedTime * this.rotationSpeed);
                this.rotateY(rotAngle);                
            }

            this.mesh.render();
        }

        public void init(string alumnoMediaFolder) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            
            this.mesh = scene.Meshes[0];
        }

        public void render(float elapsedTime) {
            this.moveAndRotate(elapsedTime);

            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.Target = this.mesh.Position;
            camera.OffsetForward = this.mesh.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
            if (showBoundingBox)
                mesh.BoundingBox.render();
        }

        public void dispose() {
            this.mesh.dispose();
        }

        #region Implementation of ITransformObject

        public Matrix Transform { get; set; }
        public bool AutoTransformEnable { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        
        public void move(Vector3 v) {
            UserVars.set("posX", v.X);
            UserVars.set("posY", v.Y);
            UserVars.set("posZ", v.Z);
        }

        public void moveOrientedY(float movement) {
            this.mesh.moveOrientedY(movement);
            this.move(this.mesh.Position);
        }

        public void rotateY(float angle) {
            this.mesh.rotateY(angle);
            GuiController.Instance.ThirdPersonCamera.rotateY(angle);
        }

        public void move(float x, float y, float z) {
            throw new NotImplementedException();
        }
        
        public void getPosition(Vector3 pos) {
            throw new NotImplementedException();
        }

        public void rotateX(float angle) {
            throw new NotImplementedException();
        }

        public void rotateZ(float angle) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
