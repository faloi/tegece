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
        bool iddle = true;
        Vector3 destination;
        const float THRESHOLD_POS = 50f;
        const float THRESHOLD_DIR = 0.5f;

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

            Vector3 origin = this.Position;
            iddle = this.isInPosition(Position, destination, THRESHOLD_POS);
            if (iddle) this.destination = tank.Position;
            GuiController.Instance.UserVars["aX"] = Position.X.ToString();
            GuiController.Instance.UserVars["aY"] = Position.Y.ToString();
            GuiController.Instance.UserVars["aZ"] = Position.Z.ToString();
            GuiController.Instance.UserVars["bX"] = destination.X.ToString();
            GuiController.Instance.UserVars["bY"] = destination.Y.ToString();
            GuiController.Instance.UserVars["bZ"] = destination.Z.ToString();

            if (!iddle) { //si todavía no se llegó al destino...
                Vector3 direcOrigen = this.forwardVector;
                Vector3 direcDestino = this.destination - this.Position;
                direcOrigen.Normalize();
                direcDestino.Normalize();
                if (isInPosition(direcOrigen, direcDestino, THRESHOLD_DIR)) {
                    this.move(dir); //va bien => sigue derecho
                } else {
                    this.rotate(Direction.Right); //girar para acomodarse
                }
            }

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