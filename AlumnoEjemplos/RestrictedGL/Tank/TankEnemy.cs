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
        public ITransformObject tank { get; set; }
        private float time = 0;
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

            base.processMovement();
        }

        protected override string pathShader() { return Path.TankEnemyShader; }
        protected override void processShader() {
            time += Shared.ElapsedTime;
            this.effect.SetValue("time", time);
            if (time > float.MaxValue - 3) time = 0;
        }
    }
}