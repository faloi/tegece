using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
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

    public abstract class Tank : ITransformObject {
        private const float SCALE = 3;
        private const float INTERVAL_BETWEEN_MISSILES = 2.5f;
        protected const float MAX_SPEED = 300;
        private const int MAX_BLOCKED_TIME = 2;

        public TgcBoundingSphere boundingSphere;
        public readonly MeshShader mesh;
        public Tank enemy { set; get; }
        protected Microsoft.DirectX.Direct3D.Effect effect;
        private readonly Terrain.Terrain terrain;
        protected Vector3 forwardVector;
        protected Vector3 initMissileRotation;

        public int score;
        protected float time = 0;
        protected float blockedTime = 0;
        
        private bool blocked;
        public bool isBlocked {
            get { return this.blocked; }
            set { this.blocked = value; this.effect.SetValue("isBlocked", value); }
        }

        public bool isPermanentBlocked;
        protected bool isMoving;
        protected bool isRotating;
        protected bool colliding;
        protected float speed;
        protected float totalSpeed; //valor absoluto de speed
        protected float rotationSpeed;
        protected float totalRotationSpeed; //valor absoluto de rotationSpeed
        protected Direction direction;
        protected Direction lastDirectionBeforeCrash;
        private Matrix translationMatrix;
        

        public Tank(Vector3 initialPosition, Terrain.Terrain terrain, string scenePath) {
            var loader = new TgcSceneLoader { MeshFactory = new MeshShaderFactory() };
            var scene = loader.loadSceneFromFile(scenePath);
            this.mesh = (MeshShader) scene.Meshes[0];
            this.loadShader();
            
            this.terrain = terrain;
            missilesShooted = new List<Missile>();

            this.boundingSphere = new TgcBoundingSphere(this.mesh.BoundingBox.calculateBoxCenter(), this.mesh.BoundingBox.calculateBoxRadius()*3);

            this.mesh.AutoTransformEnable =  this.mesh.AutoUpdateBoundingBox = false;
            this.translationMatrix = Matrix.Identity;
            this.Position = initialPosition;
            
            this.setTranslationMatrix(initialPosition);
            this.totalSpeed = 0f;
            this.totalRotationSpeed = 100f;
            this.forwardVector = new Vector3(0, 0, -1);
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

        public abstract void setInitMissileRotation();

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

        protected Matrix transformMatrix {
            get { return scaleMatrix*rotationMatrix*translationMatrix; }
        }

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
            
        public virtual void moveOrientedY(float movement) {
            mesh.moveOrientedY(movement);
            
        }

        public virtual void rotateY(float angle) {
            mesh.rotateY(angle);
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
                this.setInitMissileRotation();
                var newMissile = new Missile(realPosition, this.initMissileRotation);
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

        protected virtual void processSuccessfulShot() {
            this.score++;
            this.enemy.blockTank();
        }

        protected void processMovement() {
            if (this.isMoving) {
                moveOrientedY(Shared.ElapsedTime * speed);
                setTranslationMatrix(Position);
                this.colliding = this.terrain.treeFactory.isAnyCollidingWith(this.boundingSphere) || TgcCollisionUtils.testSphereSphere(this.boundingSphere, this.enemy.boundingSphere) || terrain.isOutOfBounds(this.mesh);
                if (this.colliding) {
                    if (this.lastDirectionBeforeCrash == this.direction)
                    {
                        this.stop();
                        this.moveOrientedY(Shared.ElapsedTime * (-speed));
                        this.setTranslationMatrix(Position);
                    }
                }else{
                    this.lastDirectionBeforeCrash = this.direction;
                }
                this.boundingSphere.setCenter(this.mesh.BoundingBox.calculateBoxCenter()); 
            }

            if (this.isRotating) {
                var rotAngle = Geometry.DegreeToRadian(rotationSpeed * Shared.ElapsedTime);
                this.rotateY(rotAngle);
                this.forwardVector.TransformNormal(Matrix.RotationY(rotAngle));
                if (terrain.isOutOfBounds(this.mesh)) {
                    this.rotateY(-rotAngle);
                    this.forwardVector.TransformNormal(Matrix.RotationY(-rotAngle));
                }
            }
        }

     
        public virtual void render() {
            var d3DInput = Gui.I.D3dInput;
            if(this.isPermanentBlocked && d3DInput.keyPressed(Key.R)) {
                this.isPermanentBlocked = false;
                this.enemy.isPermanentBlocked = false;
            }
            if(this.isBlocked) {
                this.blockedTime += Shared.ElapsedTime;
            }else if (this.isPermanentBlocked) {
                this.blockedTime = 0;
            }else{
                this.moveAndRotate();
                this.processShader();
            }
            if(this.blockedTime>= MAX_BLOCKED_TIME) {
                this.isBlocked = false;
                this.blockedTime = 0f;
            }

            this.mesh.BoundingBox.transform(transformMatrix);

            this.mesh.Transform = transformMatrix;
            

            this.mesh.render();
            
            if (Modifiers.showBoundingBox())
                this.boundingSphere.render();

            var missilesToRemove = new List<Missile>();
            foreach (var missile in this.missilesShooted) {
                if (missile.isCollidingWith(terrain)) {
                    this.terrain.deform(missile.Position.X, missile.Position.Z, 150, 10);
                    missilesToRemove.Add(missile);
                }
                else if (missile.isExplodedOnTank(this.enemy)) {
                    this.processSuccessfulShot();
                    missilesToRemove.Add(missile);
                }
                else if (this.terrain.isOutOfBounds(missile)) {
                    missilesToRemove.Add(missile);
                }
                else {
                    missile.render();
                }
            }

            missilesToRemove.ForEach(o => missilesShooted.Remove(o));
        }

        private void loadShader() {
            var d3dDevice = Gui.I.D3dDevice;
            string compilationErrors;
            this.effect = Microsoft.DirectX.Direct3D.Effect.FromFile(d3dDevice, this.pathShader(), null, null, ShaderFlags.None, null, out compilationErrors);
            this.mesh.effect = effect;
            
        }

        public void blockTank() {
            this.isBlocked = true;
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

        public virtual void dispose() {
            this.mesh.dispose();
            
        }
    }
}