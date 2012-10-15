using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RestrictedGL.Terrain {
    public class SkyBox : TgcSkyBox {
        public SkyBox(Vector3 center, Vector3 size) {
            this.Center = center;
            this.Size = size;

            this.setFaceTexture(SkyFaces.Up, Path.SkyTop);
            this.setFaceTexture(SkyFaces.Down, Path.SkyBottom);
            this.setFaceTexture(SkyFaces.Left, Path.SkyLeft);
            this.setFaceTexture(SkyFaces.Right, Path.SkyRight);

            this.setFaceTexture(SkyFaces.Front, Path.SkyFront);
            this.setFaceTexture(SkyFaces.Back, Path.SkyBack);

            this.updateValues();
        }
    }
}