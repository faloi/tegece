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

    public class Tank : IAlumnoRenderObject {

        private TgcMesh mesh;
        
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;

        public Vector3 position { get; set; }
        
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
                mesh.moveOrientedY(elapsedTime * this.linearSpeed);

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(elapsedTime * this.rotationSpeed);
                mesh.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            this.mesh.render();
        }

        private void updatePositionVars() {
            UserVars.set("posX", this.mesh.Position.X);
            UserVars.set("posY", this.mesh.Position.Y);
            UserVars.set("posZ", this.mesh.Position.Z);
        }

        public void init(string alumnoMediaFolder) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            
            this.mesh = scene.Meshes[0];
        }

        public void render(float elapsedTime) {
            this.moveAndRotate(elapsedTime);
            this.updatePositionVars();

            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.Target = mesh.Position;
            camera.OffsetForward = mesh.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
            if (showBoundingBox)
                mesh.BoundingBox.render();
        }

        public void dispose() {
            this.mesh.dispose();
        }
    }
}
