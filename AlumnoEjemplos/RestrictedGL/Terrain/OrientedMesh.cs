using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public struct OrientedMesh
    {
        //contiene un mesh y su posición en la matriz 2d del heightmap
        public TgcMesh mesh;
        public int[] gridPos;

        public OrientedMesh(TgcMesh mesh) {
            this.mesh = mesh;
            gridPos = new int[2];
        }
    }
}