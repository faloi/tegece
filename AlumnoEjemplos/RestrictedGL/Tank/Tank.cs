using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    
    internal enum Direction {
        Forward,
        Backward,
        Left,
        Right
    }

    public class Tank : ITransformObject {

        public TgcBoundingBox BoundingBox
        {
            get { return this.mesh.BoundingBox; }
        }

        private readonly UserVars userVars;
        private readonly TgcMesh mesh;
        
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;

        public List<Missile> missilesShooted { get; set; }
        private readonly Vector3 scale = new Vector3(3, 3, 3);

        private float calculateSpeed(Direction direction) {
            var speed = Modifiers.get<float>("tankVelocity");

            return direction == Direction.Backward || direction == Direction.Right
               ? speed
               : -speed;
        }

        private void shoot() {
            var newMissile = new Missile(this.mesh.Position,this.mesh.Rotation);
            this.missilesShooted.Add(newMissile);
        }

        private void move(Direction direction) {
            this.linearSpeed = calculateSpeed(direction);
            this.isMoving = true;
        }

        private void rotate(Direction direction) {
            this.rotationSpeed = calculateSpeed(direction);
            this.isRotating = true;
        }

        public Tank(Vector3 initialPosition) {
            this.userVars = new UserVars();
            
            var scene = new TgcSceneLoader().loadSceneFromFile(Shared.MediaFolder + "#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            
            this.mesh = scene.Meshes[0];
            this.mesh.move(initialPosition);
            this.mesh.Scale = this.scale;

            this.missilesShooted = new List<Missile>();
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
            if (d3DInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                this.shoot();

            var camera = GuiController.Instance.ThirdPersonCamera;
            
            if (this.isMoving) {
                this.moveOrientedY(elapsedTime*this.linearSpeed);
                camera.Target = this.mesh.Position;
            }

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(rotationSpeed * elapsedTime);
                this.mesh.rotateY(rotAngle);
                camera.rotateY(rotAngle);
            }
        }

        public void render(float elapsedTime)
        {
            this.moveAndRotate(elapsedTime);
            this.mesh.render();

            foreach (var missile in missilesShooted) {
                missile.render(elapsedTime);
            }

        }

        public void dispose() {
            this.mesh.dispose();
        }

        public bool AlphaBlendEnable { get; set; }

        #region Implementation of ITransformObject

        public Matrix Transform { get; set; }
        public bool AutoTransformEnable { get; set; }
        
        public Vector3 Position {
            get { return this.mesh.Position; }
            set { this.mesh.Position = value; }
        }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        
        public void move(Vector3 v) {
            this.userVars
                .set("posX", v.X)
                .set("posY", v.Y)
                .set("posZ", v.Z);
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
