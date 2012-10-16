using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
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

        private readonly UserVars userVars;
        private readonly TgcMesh mesh;
        
        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;
        private ITerrainCollision terrain;

        public List<Missile> missilesShooted { get; set; }
        private const float SCALE = 3;

        public Tank(Vector3 initialPosition, ITerrainCollision terrain)
        {
            this.userVars = new UserVars();

            var scene = new TgcSceneLoader().loadSceneFromFile(Path.TankScene);
            this.mesh = scene.Meshes[0];

            this.terrain = terrain;
            this.missilesShooted = new List<Missile>();
            
            this.translationMatrix = Matrix.Identity;
            this.setTranslationMatrix(initialPosition);
            this.mesh.AutoTransformEnable = false;
        }

        private void setTranslationMatrix(Vector3 translation) {
            var matrix = this.translationMatrix;
            matrix.Translate(this.createHeightmapPointFromTankPosition(translation));
            
            this.translationMatrix = matrix;
        }

        private float calculateSpeed(Direction direction) {
            var speed = Modifiers.get<float>("tankVelocity");

            return direction == Direction.Backward || direction == Direction.Right
               ? speed
               : -speed;
        }

        private Vector3 realPosition { get { return this.createHeightmapPointFromTankPosition(this.Position); } }

        private void shoot() {
            var newMissile = new Missile(this.realPosition, this.Rotation);
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

        private Vector3 createHeightmapPointFromTankPosition(Vector3 position) {
            return new Vector3(position.X, this.terrain.getYValueFor(position.X, position.Z) * this.terrain.ScaleY, position.Z);
        }

        private Vector3[] heightmapContactPoints { 
            get {
                var corners = this.mesh.BoundingBox.computeCorners();

                var basePoints = new[] {0, 1, 4};
                var heightmapPoints = new Vector3[3];

                for (var i = 0; i < basePoints.Length; i++) {
                    var nextPoint = basePoints[i];
                    heightmapPoints[i] = this.createHeightmapPointFromTankPosition(corners[nextPoint]);
                }

                return heightmapPoints;
            }
        }

        private Plane positionPlane {
            get {
                var contactPoints = this.heightmapContactPoints;
                return Plane.FromPoints(contactPoints[0], contactPoints[1], contactPoints[2]);
            }
        }

        private Vector3 positionPlaneNormal {
            get {
                var plane = this.positionPlane;
                return new Vector3(-plane.A, -plane.B, -plane.C);
            }
        }

        private Vector3 rightMovementVector {
            get { return Vector3.Cross(this.positionPlaneNormal, this.forwardVector); }
        }

        private Vector3 forwardVector {
            get {
                var faces = this.mesh.BoundingBox.computeFaces();
                var forwardFacePlane = faces[2].Plane;
                return new Vector3(forwardFacePlane.A, forwardFacePlane.B, forwardFacePlane.C);
            }
        }

        private Vector3 adaptedForwardVector {
            get { return Vector3.Cross(this.rightMovementVector, this.positionPlaneNormal); }
        }

        private Matrix rotationMatrix {
            get {
                var rotation = Matrix.Identity;

                var right = this.rightMovementVector;
                rotation.M11 = right.X;
                rotation.M12 = right.Y;
                rotation.M13 = right.Z;

                var normal = this.positionPlaneNormal;
                rotation.M21 = normal.X;
                rotation.M22 = normal.Y;
                rotation.M23 = normal.Z;

                var forward = this.adaptedForwardVector;
                rotation.M31 = forward.X;
                rotation.M32 = forward.Y;
                rotation.M33 = forward.Z;

                rotation.M44 = 1;

                return rotation;
            }
        }

        private Matrix scaleMatrix {
            get {
                var matrix = Matrix.Identity;
                matrix.M11 = matrix.M22 = matrix.M33 = SCALE;

                return matrix;
            }
        }

        private void moveAndRotate() {
            var d3DInput = GuiController.Instance.D3dInput;

            this.isMoving = false;
            this.isRotating = false;

            if (d3DInput.keyDown(Key.UpArrow))
                this.move(Direction.Forward);
            if (d3DInput.keyDown(Key.DownArrow))
                this.move(Direction.Backward);
            
            if (d3DInput.keyDown(Key.RightArrow))
                this.rotate(Direction.Right);
            if (d3DInput.keyDown(Key.LeftArrow))
                this.rotate(Direction.Left);
            
            if (d3DInput.keyDown(Key.RightControl))
                this.shoot();

            var camera = GuiController.Instance.ThirdPersonCamera;
            
            if (this.isMoving) {
                this.moveOrientedY(Shared.ElapsedTime*this.linearSpeed);
                camera.Target = this.Position;
                this.setTranslationMatrix(this.Position);
            }

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(rotationSpeed * Shared.ElapsedTime);
                this.mesh.rotateY(rotAngle);
                camera.rotateY(rotAngle);
            }
        }

        public void render()
        {
            this.moveAndRotate();
            
            this.mesh.Transform = this.transformMatrix;
            this.mesh.render();

            var missilesToRemove = new List<Missile>();
            foreach (var missile in missilesShooted) {
                if (missile.isCollidingWith(terrain)) {
                    this.terrain.deform(missile.Position.X, missile.Position.Z, 150, 10);
                    missilesToRemove.Add(missile);
                } else {
                    missile.render();
                }
            }
            missilesToRemove.ForEach(o=> missilesShooted.Remove(o));
        }

        private Matrix transformMatrix {
            get { return this.scaleMatrix * this.rotationMatrix * this.translationMatrix; }
        }

        private Matrix translationMatrix { get; set; }

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

        public Vector3 Rotation { get { return this.mesh.Rotation; } set { throw new NotImplementedException(); } }
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
