using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public static class Tree {
        private static TgcMesh baseTree;
        private static int count;
        private static readonly Vector3 scale = new Vector3(3, 4.5f, 3);

        private static void createBaseTree() {
            var scene = new TgcSceneLoader().loadSceneFromFile(Path.TankScene);
            baseTree = (TgcMesh) scene.Meshes[2];
        }

        public static TgcMesh create(Vector3 initialPosition) {
            if (baseTree == null)
                createBaseTree();

            count++;
            
            var newTree = baseTree.createMeshInstance(baseTree.Name + count);
            newTree.Position = initialPosition;
            newTree.Scale = scale;

            return newTree;
        }

        public static float baseSize {
            get {
                if (baseTree == null)
                    createBaseTree();

                return baseTree.BoundingBox.calculateSize().X * scale.X;
            }
        }
    }
}
