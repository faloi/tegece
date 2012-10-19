using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    
    public enum Direction {
        Forward,
        Backward,
        Left,
        Right
    }

    public class Tank : ITransformObject {
        private const float SCALE = 3;
        private const float INTERVAL_BETWEEN_MISSILES = 2.5f;

        protected readonly MeshShader mesh;
        protected Microsoft.DirectX.Direct3D.Effect effect;
        private readonly ITerrainCollision terrain;
        private readonly UserVars userVars;
        private Vector3 forwardVector;

        private bool isMoving;
        private bool isRotating;
        private float linearSpeed;
        private float rotationSpeed;
        private Matrix translationMatrix;

        public Tank(Vector3 initialPosition, ITerrainCollision terrain) {
            this.userVars = new UserVars();

            var loader = new TgcSceneLoader { MeshFactory = new MeshShaderFactory() };
            var scene = loader.loadSceneFromFile(Path.TankScene);
            this.mesh = (MeshShader) scene.Meshes[0];
            this.loadShader();

            this.terrain = terrain;
            missilesShooted = new List<Missile>();

            this.mesh.AutoTransformEnable =  this.mesh.AutoUpdateBoundingBox = false;
            this.translationMatrix = Matrix.Identity;
            
            this.setTranslationMatrix(initialPosition);

            this.forwardVector = new Vector3(0, 0, 1);
        }

        public TgcBoundingBox boundingBox {
            get { return mesh.BoundingBox; }
        }

        private List<Missile> missilesShooted { get; set; }

        private Vector3 realPosition {
            get { return createHeightmapPointFromTankPosition(Position); }
        }

        private Vector3[] heightmapContactPoints {
            get {
                var corners = mesh.BoundingBox.computeCorners();

                var basePoints = new[] {0, 1, 4};
                var heightmapPoints = new Vector3[3];

                for (var i = 0; i < basePoints.Length; i++) {
                    var nextPoint = basePoints[i];
                    heightmapPoints[i] = createHeightmapPointFromTankPosition(corners[nextPoint]);
                }

                return heightmapPoints;
            }
        }

        private Plane positionPlane {
            get {
                var contactPoints = heightmapContactPoints;
                return Plane.FromPoints(contactPoints[0], contactPoints[1], contactPoints[2]);
            }
        }

        private Vector3 positionPlaneNormal {
            get {
                var plane = positionPlane;
                return new Vector3(plane.A, plane.B, plane.C);
            }
        }

        private Vector3 rightMovementVector {
            get { return Vector3.Cross(positionPlaneNormal, forwardVector); }
        }

        private Vector3 adaptedForwardVector {
            get { return Vector3.Cross(rightMovementVector, positionPlaneNormal); }
        }

        private Matrix rotationMatrix {
            get {
                var rotation = Matrix.Identity;

                var right = rightMovementVector;
                rotation.M11 = right.X;
                rotation.M12 = right.Y;
                rotation.M13 = right.Z;

                var normal = positionPlaneNormal;
                rotation.M21 = normal.X;
                rotation.M22 = normal.Y;
                rotation.M23 = normal.Z;

                var forward = adaptedForwardVector;
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

        private Matrix transformMatrix {
            get { return scaleMatrix*rotationMatrix*translationMatrix; }
        }

        public bool AlphaBlendEnable { get; set; }

        #region Implementation of ITransformObject

        public Matrix Transform { get; set; }
        public bool AutoTransformEnable { get; set; }

        public Vector3 Position {
            get { return mesh.Position; }
            set { mesh.Position = value; }
        }

        public Vector3 Rotation {
            get { return mesh.Rotation; }
            set { throw new NotImplementedException(); }
        }

        public Vector3 Scale { get; set; }

        public void move(Vector3 v) {
            userVars
                .set("posX", v.X)
                .set("posY", v.Y)
                .set("posZ", v.Z);
        }

        public void moveOrientedY(float movement) {
            mesh.moveOrientedY(movement);
            move(mesh.Position);
        }

        public void rotateY(float angle) {
            mesh.rotateY(angle);
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

        private void setTranslationMatrix(Vector3 translation) {
            this.translationMatrix.Translate(createHeightmapPointFromTankPosition(translation));
        }

        private float calculateSpeed(Direction direction) {
            var speed = Modifiers.get<float>("tankVelocity");

            return direction == Direction.Backward || direction == Direction.Right
                       ? speed
                       : -speed;
        }

        protected void shoot() {
            var flightTimeOfLastMissile=0f;
            foreach (var missile in missilesShooted) {
                if(flightTimeOfLastMissile==0f)
                    flightTimeOfLastMissile=missile.flightTime;
                else if(flightTimeOfLastMissile<missile.flightTime) {
                    flightTimeOfLastMissile = missile.flightTime;
                }
            }
            if(INTERVAL_BETWEEN_MISSILES<=flightTimeOfLastMissile || missilesShooted.Count==0){
                var newMissile = new Missile(realPosition, Rotation);
                TgcStaticSound sound = new TgcStaticSound();
                sound.loadSound(Path.ExplosionSound);  
                sound.play(false);
                missilesShooted.Add(newMissile);
            }
        }

        protected void move(Direction direction) {
            linearSpeed = calculateSpeed(direction);
            isMoving = true;
        }

        protected void rotate(Direction direction) {
            rotationSpeed = calculateSpeed(direction);
            isRotating = true;
        }

        private Vector3 createHeightmapPointFromTankPosition(Vector3 position) {
            return new Vector3(position.X, terrain.getYValueFor(position.X, position.Z)*terrain.ScaleY, position.Z);
        }

        protected virtual void moveAndRotate() {
            var d3DInput = GuiController.Instance.D3dInput;

            isMoving = false;
            isRotating = false;

            if (d3DInput.keyDown(Key.UpArrow))
                move(Direction.Forward);
            if (d3DInput.keyDown(Key.DownArrow))
                move(Direction.Backward);

            if (d3DInput.keyDown(Key.RightArrow))
                rotate(Direction.Right);
            if (d3DInput.keyDown(Key.LeftArrow))
                rotate(Direction.Left);

            if (d3DInput.keyDown(Key.RightControl))
                shoot();

            this.processMovement();
        }

        protected void processMovement() {
            var camera = GuiController.Instance.ThirdPersonCamera;
            if (isMoving) {
                moveOrientedY(Shared.ElapsedTime * linearSpeed);
                camera.Target = Position;
                setTranslationMatrix(Position);
            }

            if (isRotating) {
                var rotAngle = Geometry.DegreeToRadian(rotationSpeed * Shared.ElapsedTime);
                camera.rotateY(rotAngle);
                rotateY(rotAngle);
                forwardVector.TransformNormal(Matrix.RotationY(rotAngle));
            }
        }

        public virtual void render() {
            this.moveAndRotate();

            this.mesh.BoundingBox.transform(transformMatrix);
            this.mesh.Transform = transformMatrix;

            this.processShader();
            this.mesh.render();

            if (Modifiers.showBoundingBox())
                this.mesh.BoundingBox.render();

            var missilesToRemove = new List<Missile>();
            foreach (var missile in this.missilesShooted) {
                if (missile.isCollidingWith(terrain)) {
                    this.terrain.deform(missile.Position.X, missile.Position.Z, 150, 10);
                    missilesToRemove.Add(missile);
                }
                
                else if (this.terrain.isOutOfBounds(missile)) 
                    missilesToRemove.Add(missile);
                
                else 
                    missile.render();
            }
            
            missilesToRemove.ForEach(o => missilesShooted.Remove(o));
        }

        private void loadShader() {
            var d3dDevice = GuiController.Instance.D3dDevice;
            string compilationErrors;
            this.effect = Microsoft.DirectX.Direct3D.Effect.FromFile(d3dDevice, this.pathShader(), null, null, ShaderFlags.None, null, out compilationErrors);
            this.mesh.effect = effect;
        }

        protected virtual string pathShader() { return Path.TankShader; }
        protected virtual void processShader() { }

        public void dispose() {
            this.mesh.dispose();
        }
    }
}