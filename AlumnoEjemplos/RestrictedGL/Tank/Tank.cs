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
using AlumnoEjemplos.RestrictedGL.Terrain;

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
        protected const float MAX_SPEED = 300;

        public TgcBoundingSphere boundingSphere;
        public readonly MeshShader mesh;
        public Tank enemy { set; get; }
        protected Microsoft.DirectX.Direct3D.Effect effect;
        private readonly Terrain.Terrain terrain;
        protected Vector3 forwardVector;

        protected float time = 0;
        protected bool isMoving;
        protected bool isRotating;
        protected bool colliding;
        protected float speed;
        protected float totalSpeed; //valor absoluto de speed
        protected float rotationSpeed;
        protected float totalRotationSpeed; //valor absoluto de rotationSpeed
        protected Direction direction;
        private Matrix translationMatrix;
        public Vector3 lastRotation;
        private TgcMesh turret;


        public Tank(Vector3 initialPosition, Terrain.Terrain terrain) {
            var loader = new TgcSceneLoader { MeshFactory = new MeshShaderFactory() };
            var scene = loader.loadSceneFromFile(Path.Tank);
            this.mesh = (MeshShader) scene.Meshes[0];
            this.loadShader();
            var turretScene = loader.loadSceneFromFile(Path.Turret);
            this.turret = turretScene.Meshes[0];

            this.terrain = terrain;
            missilesShooted = new List<Missile>();

            this.boundingSphere = new TgcBoundingSphere(this.mesh.BoundingBox.calculateBoxCenter(), this.mesh.BoundingBox.calculateBoxRadius()*3);

            this.mesh.AutoTransformEnable =  this.mesh.AutoUpdateBoundingBox = this.turret.AutoUpdateBoundingBox = this.turret.AutoTransformEnable = false;
            this.translationMatrix = Matrix.Identity;
            this.Position = initialPosition;
            
            this.setTranslationMatrix(initialPosition);
            this.totalSpeed = 0f;
            this.totalRotationSpeed = 100f;
            this.forwardVector = new Vector3(0, 0, -1);
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
            
        public void moveOrientedY(float movement) {
            mesh.moveOrientedY(movement);
            turret.moveOrientedY(movement);
        }

        public void rotateY(float angle) {
            mesh.rotateY(angle);
            turret.rotateY(angle);
            this.lastRotation = mesh.Rotation;
            //Gui.I.ThirdPersonCamera.rotateY(angle);
        }

        public void move(Vector3 v) {
            throw new NotImplementedException();
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
            return direction == Direction.Backward ? totalSpeed : -totalSpeed;
        }
        private float calculateRotationSpeed(Direction direction) {
            return direction == Direction.Right ? totalRotationSpeed : -totalRotationSpeed;
        }

        protected void shoot() {
            var flightTimeOfLastMissile = 0f;
            foreach (var missile in missilesShooted) {
                if (flightTimeOfLastMissile == 0f)
                    flightTimeOfLastMissile = missile.flightTime;
                else if (flightTimeOfLastMissile < missile.flightTime) {
                    flightTimeOfLastMissile = missile.flightTime;
                }
            }
            if (INTERVAL_BETWEEN_MISSILES<=flightTimeOfLastMissile || missilesShooted.Count==0) {
                var newMissile = new Missile(realPosition, Rotation);
                TgcStaticSound sound = new TgcStaticSound();
                sound.loadSound(Path.ExplosionSound);  
                sound.play(false);
                missilesShooted.Add(newMissile);
            }
        }

        protected void move(Direction direction) {
            speed = calculateSpeed(direction);
            isMoving = true;
        }

        protected void rotate(Direction direction) {
            this.rotationSpeed = Modifiers.get<float>("rotationVelocity");
            rotationSpeed = calculateRotationSpeed(direction);
            isRotating = true;
        }

        protected void acel(float n) {
            this.totalSpeed += n;
            if (totalSpeed > MAX_SPEED) totalSpeed = MAX_SPEED;
        }

        protected void doFriction() {
            if (isMoving)
                this.totalSpeed -= 0.5f;
            if (totalSpeed <= 0) this.totalSpeed = 0;
        }

        protected void stop() {
            isMoving = false;
            totalSpeed = 0;
        }

        private Vector3 createHeightmapPointFromTankPosition(Vector3 position) {
            return new Vector3(position.X, terrain.getYValueFor(position.X, position.Z)*terrain.ScaleY, position.Z);
        }

        protected virtual void moveAndRotate() {
            var d3DInput = Gui.I.D3dInput;

            if (this.totalSpeed == 0) isMoving = false;
            isRotating = false;

            if (d3DInput.keyDown(Key.UpArrow)) {
                if (!isMoving || (isMoving && direction == Direction.Forward)) {
                    direction = Direction.Forward;
                    acel(1);
                    move(Direction.Forward);
                } else {
                    acel(-1);
                }
            }
            if (d3DInput.keyDown(Key.DownArrow)) {
                if (!isMoving || (isMoving && direction == Direction.Backward)) {
                    direction = Direction.Backward;
                    acel(1);
                    move(Direction.Backward);
                } else {
                    acel(-1);
                }
            }

            if (d3DInput.keyDown(Key.RightArrow))
                rotate(Direction.Right);
            if (d3DInput.keyDown(Key.LeftArrow))
                rotate(Direction.Left);

            if (d3DInput.keyDown(Key.RightControl))
                shoot();

            doFriction();

            UserVars.set("totalSpeed", totalSpeed);
            UserVars.set("direction", direction.ToString());
            this.processMovement();
        }

        protected void processMovement() {
            //var camera = Gui.I.ThirdPersonCamera;

            if (isMoving) {
                moveOrientedY(Shared.ElapsedTime * speed);
                //camera.Target = Position;
                setTranslationMatrix(Position);
                this.colliding = this.terrain.treeFactory.isAnyCollidingWith(this.boundingSphere) || TgcCollisionUtils.testAABBAABB(this.mesh.BoundingBox, this.enemy.boundingBox);
                if (terrain.isOutOfBounds(this.mesh) || this.colliding) {
                    this.stop();
                    moveOrientedY(Shared.ElapsedTime * (-speed));
                    //camera.Target = Position;
                    setTranslationMatrix(Position);
                }
                this.boundingSphere.setCenter(this.mesh.BoundingBox.calculateBoxCenter()); 
            }

            if (isRotating) {
                var rotAngle = Geometry.DegreeToRadian(rotationSpeed * Shared.ElapsedTime);
                //camera.rotateY(rotAngle);
                rotateY(rotAngle);
                forwardVector.TransformNormal(Matrix.RotationY(rotAngle));
                if (terrain.isOutOfBounds(this.mesh)) {
                    //camera.rotateY(-rotAngle);
                    rotateY(-rotAngle);
                    forwardVector.TransformNormal(Matrix.RotationY(-rotAngle));
                }
            }
        }

        public virtual void render() {
            this.moveAndRotate();

            this.mesh.BoundingBox.transform(transformMatrix);
            this.turret.BoundingBox.transform(transformMatrix);
            this.mesh.Transform = transformMatrix;
            this.turret.Transform = transformMatrix;

            this.processShader();
            this.mesh.render();
            this.turret.render();

            if (Modifiers.showBoundingBox())
                this.boundingSphere.render();

            var missilesToRemove = new List<Missile>();
            foreach (var missile in this.missilesShooted) {
                if (missile.isCollidingWith(terrain)) {
                    this.terrain.deform(missile.Position.X, missile.Position.Z, 150, 10);
                    missilesToRemove.Add(missile);
                } else if (this.terrain.isOutOfBounds(missile)) 
                    missilesToRemove.Add(missile);
                else 
                    missile.render();
            }
            
            missilesToRemove.ForEach(o => missilesShooted.Remove(o));
        }

        private void loadShader() {
            var d3dDevice = Gui.I.D3dDevice;
            string compilationErrors;
            this.effect = Microsoft.DirectX.Direct3D.Effect.FromFile(d3dDevice, this.pathShader(), null, null, ShaderFlags.None, null, out compilationErrors);
            this.mesh.effect = effect;
            
        }

        protected virtual string pathShader() { return Path.TankShader; }
        protected virtual void processShader() {
            time += Shared.ElapsedTime;
            this.effect.SetValue("time", time);
            if (time > float.MaxValue - 3) time = 0;
            if (isMoving) {
                effect.Technique = "RenderSceneMovement";
            } else {
                effect.Technique = "RenderScene";
            }
        }

        public void dispose() {
            this.mesh.dispose();
            this.turret.dispose();
        }
    }
}