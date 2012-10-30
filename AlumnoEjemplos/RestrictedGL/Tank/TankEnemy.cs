using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    public class TankEnemy : Tank {
        public Tank tank { get; set; }
        private Direction dir = Direction.Forward;
        Vector3 destination;
        const double THRESHOLD_POS = 200;
        const double THRESHOLD_DIR = 0.5;

        public TankEnemy(Vector3 initialPosition, ITerrainCollision terrain) : base(initialPosition, terrain) { }

        protected override void moveAndRotate() {
            //IA del enemigo
            //El chabon se mueve siempre para adelante, persiguiendo a la posición del tanque jugador
            //Para llegar a una determinada posición, primero debería rotar en la dirección adecuada
            //Y darle para adelante.
            isMoving = false;
            isRotating = false;

            /*
            var d3DInput = GuiController.Instance.D3dInput;
            if (d3DInput.keyDown(Key.U))
                rotate(Direction.Right);
            if (d3DInput.keyDown(Key.Y))
                rotate(Direction.Left);
            if (d3DInput.keyDown(Key.J)) move(dir);
            */

            //Origen, y destino:
            Vector3 origin = this.Position;
            this.destination = tank.Position;

            //Direcciones origen y destino:
            Vector3 direcOrg = this.forwardVector; 
            Vector3 direcDst = this.destination - this.Position;

            //Normalizar direcciones y sus ángulos:
            direcOrg.Normalize(); 
            direcDst.Normalize();
            double angOrg = Math.Atan2(direcOrg.Z, direcOrg.X);
            double angDst = Math.Atan2(direcDst.Z, direcDst.X);
            if (angOrg < 0) angOrg += 2 * Math.PI;
            if (angDst < 0) angOrg += 2 * Math.PI;
            double distLeft = angOrg > angDst ? Math.PI * 2 - angOrg + angDst : angDst - angOrg;
            double distRight = angOrg < angDst ? Math.PI * 2 - angDst + angOrg : angOrg - angDst;
            if (!isInPosition(direcOrg, direcDst, THRESHOLD_DIR)) {
                if (distLeft < distRight) {
                    this.rotate(Direction.Left);
                } else {
                    this.rotate(Direction.Right);
                }   
            }
            this.move(dir);
            
            /*
            GuiController.Instance.UserVars["aX"] = direcOrg.X.ToString();
            GuiController.Instance.UserVars["aY"] = "0";
            GuiController.Instance.UserVars["aZ"] = direcOrg.Z.ToString();
            GuiController.Instance.UserVars["bX"] = direcDst.X.ToString();
            GuiController.Instance.UserVars["bY"] = "0";
            GuiController.Instance.UserVars["bZ"] = direcDst.Z.ToString();
            */

            base.processMovement();
        }

        private bool isInPosition(Vector3 positionA, Vector3 positionB, double threshold) {
            //Determina si A está en la posición B (teniendo en cuanta un rango de error)
            return (positionA.X > positionB.X - threshold && positionA.X < positionB.X + threshold &&
                    positionA.Z > positionB.Z - threshold && positionA.Z < positionB.Z + threshold);
        }

        protected override string pathShader() { return Path.TankEnemyShader; }
    }
}