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
        private float timeDirChange = 0;
        private Direction dir = Direction.Forward;

        public TankEnemy(Vector3 initialPosition, ITerrainCollision terrain) : base(initialPosition, terrain) { }

        protected override void moveAndRotate() {
            //IA del enemigo
            timeDirChange += Shared.ElapsedTime;
            if (timeDirChange > 1) {
                timeDirChange = 0;
                if (dir == Direction.Forward) dir = Direction.Backward;
                else dir = Direction.Forward;
            }
            this.move(dir);
            //this.rotate(Direction.Left);

            double dotProductTest;
            Vector3 directionTank = new Vector3(0, tank.lastRotation.Y, 0);
            Vector3 directionEnemy = new Vector3(0, lastRotation.Y, 0);
            GuiController.Instance.UserVars["tankRotateY"] = directionTank.Y.ToString();
            GuiController.Instance.UserVars["enemyRotateY"] = directionEnemy.Y.ToString();
            directionTank.Normalize();
            directionEnemy.Normalize();
            directionTank.Y = (float) Math.Round(directionTank.Y);
            directionEnemy.Y = (float) Math.Round(directionEnemy.Y);
            dotProductTest = Math.Round(Vector3.Dot(directionTank, directionEnemy));

            GuiController.Instance.UserVars["rotateProduct"] = dotProductTest.ToString();

            base.processMovement();
        }

        protected override string pathShader() { return Path.TankEnemyShader; }
    }
}