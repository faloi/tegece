using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.Utils;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    public class TankEnemy : Tank {
        private Direction dir = Direction.Forward;
        Vector3 destination;
        bool iddle = true;
        bool avoiding = false;
        float timeAvoiding = 0;
        const double THRESHOLD_POS = 200;
        const double THRESHOLD_DIR = 0.5;

        public TankEnemy(Vector3 initialPosition, Terrain.Terrain terrain) : base(initialPosition, terrain, Path.TankEnemy) { }

        protected override void moveAndRotate() {
            //IA del enemigo:
            //El chabon se mueve siempre para adelante, persiguiendo a la posición del tanque jugador
            //Para llegar a una determinada posición, primero debería rotar en la dirección adecuada
            //Y darle para adelante.

            if (this.totalSpeed == 0) isMoving = false;
            isRotating = false;

            //Origen, y destino
            Vector3 origin = this.Position;
            if (iddle) { //(hasta no llegar ahí no cambia de destino)
                this.destination = this.enemy.Position;
                iddle = false;
            }

            //Direcciones origen y destino
            Vector3 direcOrg = this.forwardVector; 
            Vector3 direcDst = this.destination - this.Position;

            //Normalizar direcciones y sus ángulos
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
            this.acel(1);
            this.move(dir);
            this.doFriction();

            //Si este random se cumple, dispara
            if (new Utils.Randomizer(1, 500).getNext() > 495) {
                this.shoot();
                iddle = true; //para que tenga chances de esquivar el bache...
            }

            if (colliding) {
                if (!avoiding) {
                    recalc(); //REECALCULANDO...
                    timeAvoiding = 0;
                } else {
                    timeAvoiding += Shared.ElapsedTime;
                    if (timeAvoiding > 3) {
                        recalc();
                        timeAvoiding = 0;
                    }
                }
            }

            //Si llego al destino, lo marca como inactivo para buscar uno nuevo...
            if (isInPosition(Position, destination, THRESHOLD_POS)) {
                iddle = true;
                if (avoiding) avoiding = false;
            }

            Gui.I.UserVars["destX"] = destination.X.ToString();
            Gui.I.UserVars["destY"] = destination.Y.ToString();
            Gui.I.UserVars["destZ"] = destination.Z.ToString();
            Gui.I.UserVars["enemyColliding"] = colliding.ToString();
            Gui.I.UserVars["enemyAvoiding"] = avoiding.ToString();
            Gui.I.UserVars["enemyTA"] = timeAvoiding.ToString();

            base.processMovement();
        }

        private void recalc() {
            Randomizer rand = new Randomizer(-200, 200);
            destination.X = Position.X + rand.getNext();
            destination.Z = Position.Z + rand.getNext();
            iddle = true;
            avoiding = true;
        }

        protected override void processSuccessfulShot(){
            base.processSuccessfulShot();
            GuiController.Instance.Logger.log("TankEnemy " + this.score + " - " + this.enemy.score + " TankPlayer");
            if (this.score == 5){
                GuiController.Instance.Logger.log("TankEnemy WINS.");
                GuiController.Instance.Logger.log("Press R to RESTART.");

                this.isBlocked = false;
                this.isPermanentBlocked=true;
                this.score = 0;

                this.enemy.isBlocked = false;
                this.enemy.isPermanentBlocked = true;
                this.enemy.score = 0;
            }
        }

        public override void setInitMissileRotation(){
            this.initMissileRotation = this.mesh.Rotation;
        }

        private bool isInPosition(Vector3 positionA, Vector3 positionB, double threshold) {
            //Determina si A está en la posición B (teniendo en cuanta un rango de error)
            return (positionA.X > positionB.X - threshold && positionA.X < positionB.X + threshold &&
                    positionA.Z > positionB.Z - threshold && positionA.Z < positionB.Z + threshold);
        }

        protected override string pathShader() { return Path.TankEnemyShader; }
    }
}