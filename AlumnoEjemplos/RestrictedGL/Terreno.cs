using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RestrictedGL
{
    class Terreno
    {
        private const int MAX_HEIGHT_MAPS = 9;
        private const float SCALE_XZ = 10f;

        List<TgcSimpleTerrain> heightMaps;
        Vector3[] heightMapsUbication;
        
        private readonly string heigthMapPath = Shared.mediaFolder + "\\Terreno\\Heightmap.jpg";
        private readonly string texturaPath = Shared.mediaFolder + "\\Terreno\\Mapa.jpg";
        
        float scaleY;
        TgcSkyBox skyBox;

        public Terreno()
        {
            heightMapInic();
            skyBoxInic();

            //Crea modifiers para el terreno:
            GuiController.Instance.Modifiers.addFloat("scaleY", 0f, 1f, scaleY);
        }
       
        private void heightMapInic() {
            //Cargar vars:
            scaleY = 0.25f;

            //Instanciar HeightMaps:
            heightMaps = new List<TgcSimpleTerrain>();
            for (int i = 0; i < MAX_HEIGHT_MAPS; i++) {
                heightMaps.Add(new TgcSimpleTerrain());
            }

            //Determinar ubicación de cada uno:
            heightMapsUbication = new Vector3[MAX_HEIGHT_MAPS];
            const float size = 62.75f;
            var offsetX = -size;
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                heightMapsUbication[i].X = offsetX;
                if (offsetX == size) {
                    offsetX = -size;
                } else {
                    offsetX += size;
                }

                if (i <= 2) {
                    heightMapsUbication[i].Z = size;
                } else if (i <= 5) {
                    heightMapsUbication[i].Z = 0;
                } else {
                    heightMapsUbication[i].Z = -size;
                }
            }
            /*esto crearía una matriz así:
             * xxx
             * xxx
             * xxx
            *///donde cada x es un heightmap, y la del medio es el (0,0,0)
       
            heightMapLoad();
        }

        private void heightMapLoad() {
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                var heightMapActual = heightMaps[i];
                heightMapActual.loadHeightmap(heigthMapPath, SCALE_XZ, scaleY, heightMapsUbication[i]);
                heightMapActual.loadTexture(texturaPath);
            }
        }

        private void skyBoxInic() {
            //Crear SkyBox:
            skyBox = new TgcSkyBox
             {
                 Center = new Vector3(0, 0, 0), 
                 Size = new Vector3(9000, 9000, 9000)
             };

            //Configurar las texturas para cada una de las 6 caras:
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, Shared.mediaFolder + "\\Terreno\\SkyTop.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, Shared.mediaFolder + "\\Terreno\\SkyTop.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, Shared.mediaFolder + "\\Terreno\\SkyLeft.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, Shared.mediaFolder + "\\Terreno\\SkyRight.jpg");
            
            //Los back&front se invierten por usar sistema left-handed...
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, Shared.mediaFolder + "\\Terreno\\SkyFront.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, Shared.mediaFolder + "\\Terreno\\SkyBack.jpg");

            skyBox.updateValues();
        }

        private void comprobarCambios() {
            //Compruba cambios en modifiers y actualiza:
            var scaleYNew = Modifiers.get<float>("scaleY");
            
            if (scaleY == scaleYNew) return;
            
            scaleY = scaleYNew;
            heightMapLoad();
        }

        public void render() {
            comprobarCambios();
            
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) { //renderizar los N HeightMaps
                var heightMapActual = heightMaps[i];
                heightMapActual.render();
            }

            skyBox.render();
        }
    }
}