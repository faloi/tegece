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
    
    class TankPlayer : Tank {

        public TgcMesh turret;
        private const float TURRET_ROTATION_FACTOR = 0.005f;
        private float turretAngle;
        private Vector3 offsetRotationCenter = new Vector3(0, 0, 10);

        public TankPlayer (Vector3 initialPosition, Terrain.Terrain terrain, string scenePath) : base(initialPosition, terrain, scenePath) {
            
            var loader = new TgcSceneLoader { MeshFactory = new MeshShaderFactory() };
            var turretScene = loader.loadSceneFromFile(Path.Turret);
            this.turret = turretScene.Meshes[0];         
            this.turretAngle = 0;

            this.turret.AutoUpdateBoundingBox = this.turret.AutoTransformEnable = false;

        }

        public override void moveOrientedY(float movement) {
           base.moveOrientedY(movement);
           turret.moveOrientedY(movement);
        }

        public override void rotateY(float angle) {
            turret.rotateY(angle);
            base.rotateY(angle);
            GuiController.Instance.ThirdPersonCamera.rotateY(angle);
        }

        protected override void moveAndRotate() {
            var d3DInput = Gui.I.D3dInput;
            if (d3DInput.keyDown(Key.E))
                this.turretAngle += TURRET_ROTATION_FACTOR;
            if (d3DInput.keyDown(Key.Q))
                this.turretAngle -= TURRET_ROTATION_FACTOR;
            base.moveAndRotate();
        }

        public void rotateTurret() {
           
            this.turret.Transform = Matrix.Translation(offsetRotationCenter) * Matrix.RotationY(turretAngle) *
                                   Matrix.Translation(offsetRotationCenter) * transformMatrix;
            this.turret.Rotation = new Vector3(0, this.turretAngle + this.mesh.Rotation.Y, 0);
            //ACA HAY QUE HACER ROTAR LA CAMARA CUANDO ROTAS LA TURRET
            //GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(this.turret.Rotation.Y));
        }

        public override void setInitMissileRotation(){
            this.initMissileRotation = this.turret.Rotation;
        }


        public override void render() {
            base.render();
            this.turret.BoundingBox.transform(transformMatrix);
            this.turret.Transform = transformMatrix;
            this.rotateTurret();
            this.turret.render();
        }

        public override void dispose() {
            this.turret.dispose();
            base.dispose();
        }
    }
}
