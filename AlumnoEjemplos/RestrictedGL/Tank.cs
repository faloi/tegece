using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RestrictedGL {
    
    enum Direction {
        Forward,
        Backward,
        Left,
        Right
    }

    public class Tank {

        private TgcMesh tankMesh;
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;
        public Vector3 position { set; get; }
        
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
                tankMesh.moveOrientedY(elapsedTime * this.linearSpeed);

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(elapsedTime * this.rotationSpeed);
                tankMesh.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            tankMesh.render();
        }

        public void init(string alumnoMediaFolder) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            tankMesh = scene.Meshes[0];
        }

        public void render(float elapsedTime) {
            this.moveAndRotate(elapsedTime);

            UserVars.set("posX", tankMesh.Position.X);
            UserVars.set("posY", tankMesh.Position.Y);
            UserVars.set("posZ", tankMesh.Position.Z);

            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.Target = tankMesh.Position;
            camera.OffsetForward = tankMesh.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
            if (showBoundingBox)
                tankMesh.BoundingBox.render();
        }

        public void dispose() {
            tankMesh.dispose();
        }
    }
}
