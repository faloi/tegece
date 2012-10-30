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

            var d3DInput = GuiController.Instance.D3dInput;
            if (d3DInput.keyDown(Key.U))
                rotate(Direction.Right);
            if (d3DInput.keyDown(Key.Y))
                rotate(Direction.Left);
            //this.move(dir);

            //Origen, y destino:
            Vector3 origin = this.Position;
            this.destination = tank.Position;

            //Direcciones origen y destino:
            Vector3 direcOrg = this.forwardVector; 
            Vector3 direcDst = this.destination - this.Position;

            //Normalizar direcciones y sus ángulos:
            direcOrg.Normalize(); 
            direcOrg.Normalize();
            double angOrg = /*Math.Atan(direcOrg.Z, direcOrg.X)*/2;
            double angDst = /*Math.Atan(direcDst.Z, direcDst.X)*/2;
            if (angOrg < 0) angOrg += 2 * Math.PI;
            if (angDst < 0) angOrg += 2 * Math.PI;
            double distIzq = angOrg > angDst ? Math.PI * 2 - angOrg + angDst : angDst - angOrg;
            double distDer = angOrg < angDst ? Math.PI * 2 - angDst + angOrg : angOrg - angDst;

            /*
            GuiController.Instance.UserVars["aX"] = "a";
            GuiController.Instance.UserVars["aY"] = Position.Y.ToString();
            GuiController.Instance.UserVars["aZ"] = Position.Z.ToString();
            GuiController.Instance.UserVars["bX"] = destination.X.ToString();
            GuiController.Instance.UserVars["bY"] = destination.Y.ToString();
            GuiController.Instance.UserVars["bZ"] = destination.Z.ToString();
            */

            base.processMovement();
        }

        private bool isInPosition(Vector3 positionA, Vector3 positionB, float threshold) {
            //Determina si A está en la posición B (teniendo en cuanta un rango de error)
            return (positionA.X > positionB.X - threshold && positionA.X < positionB.X + threshold &&
                    positionA.Z > positionB.Z - threshold && positionA.Z < positionB.Z + threshold);
        }

        protected override string pathShader() { return Path.TankEnemyShader; }
    }
}