using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
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

        private readonly TgcMesh mesh;
        
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;
        private float currentAngle;

        private readonly List<Missile> missilesShooted;
        private readonly Vector3 scale = new Vector3(3, 3, 3);

        private float calculateSpeed(Direction direction) {
            var speed = Modifiers.get<float>("tankVelocity");

            return direction == Direction.Backward || direction == Direction.Right
               ? speed
               : -speed;
        }

        private void shoot() {
            var newMissile = new Missile(this.mesh.Position,this.currentAngle);
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
            var scene = new TgcSceneLoader().loadSceneFromFile(Shared.MediaFolder + "#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            
            this.mesh = scene.Meshes[0];
            this.mesh.move(initialPosition);
            this.mesh.Scale = this.scale;

            this.missilesShooted = new List<Missile>();
        }

        public void moveAndRotate(float elapsedTime) {
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
            if (d3DInput.keyDown(Key.Space))
                this.shoot();

            if (this.isMoving)
                this.moveOrientedY(elapsedTime * this.linearSpeed);

            if (this.isRotating) {
                currentAngle += elapsedTime*this.rotationSpeed;
 
                var rotAngle = Geometry.DegreeToRadian(currentAngle);
                this.rotateY(rotAngle);                
            }
        }

        public void render(float elapsedTime)
        {
            this.mesh.render();

            foreach (var missile in missilesShooted) {
                missile.render(elapsedTime);
            }

            //var camera = GuiController.Instance.ThirdPersonCamera;
            //camera.Target = this.mesh.Position;
            //camera.OffsetForward = this.mesh.Position.Z + Modifiers.get<float>("cameraOffsetForward");

            //var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
            //if (showBoundingBox)
            //    mesh.BoundingBox.render();
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
