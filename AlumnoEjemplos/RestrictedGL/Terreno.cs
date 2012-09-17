using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

using TgcViewer.Utils.Terrain;
using System.Collections;

namespace AlumnoEjemplos.RestrictedGL
{
    class Terreno
    {
        ArrayList heightMaps;
        Vector3[] heightMapsUbic;
        string map; //path al heightmap
        string textura; //path a la textura
        float scaleXZ;
        float scaleY;
        TgcSkyBox skyBox;

        public Terreno() {
            heightMapInic();
            skyBoxInic();
        }

        public void heightMapInic() {
            //Cargar vars:
            map = Shared.mediaFolder + "\\Terreno\\Heightmap.jpg";
            textura = Shared.mediaFolder + "\\Terreno\\Mapa.jpg";
            scaleXZ = 100f;
            scaleY = 3.5650f;

            //Instanciar HeightMaps:
            heightMaps = new ArrayList();
            for (int i = 0; i < 9; i++) {
                heightMaps.Add(new TgcSimpleTerrain());
            }

            //Determinar ubicación de cada uno:
            heightMapsUbic = new Vector3[9];
            float size = 61;
            float offsetX = -size;
            for (int i = 0; i < 9; i++) {
                heightMapsUbic[i].X = offsetX;
                if (offsetX == size) {
                    offsetX = -size;
                } else {
                    offsetX += size;
                }

                if (i <= 2) {
                    heightMapsUbic[i].Z = size;
                } else if (i <= 5) {
                    heightMapsUbic[i].Z = 0;
                } else {
                    heightMapsUbic[i].Z = -size;
                }
            }
            /*esto crearía una matriz así:
             * xxx
             * xxx
             * xxx
            *///donde cada x es un heightmap, y la del medio es el (0,0,0)
       
            for (int i = 0; i < 9; i++) {
                TgcSimpleTerrain heightMapActual = (TgcSimpleTerrain) heightMaps[i];
                heightMapActual.loadHeightmap(map, scaleXZ, scaleY, heightMapsUbic[i]);
                heightMapActual.loadTexture(textura);
            }
        }

        public void skyBoxInic() {
            //Crear SkyBox:
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2500, 0);
            skyBox.Size = new Vector3(30000, 60000, 15000);

            //Configurar las texturas para cada una de las 6 caras:
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, Shared.mediaFolder + "\\Terreno\\phobos_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, Shared.mediaFolder + "\\Terreno\\phobos_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, Shared.mediaFolder + "\\Terreno\\phobos_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, Shared.mediaFolder + "\\Terreno\\phobos_rt.jpg");
            //Los back&front se invierten por usar sistema left-handed...
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, Shared.mediaFolder + "\\Terreno\\phobos_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, Shared.mediaFolder + "\\Terreno\\phobos_ft.jpg");

            skyBox.updateValues();
        }

        public void skyBoxUpdate(Vector3 skyCenterNew, Vector3 skySizeNew) {
            //Actualiza los valores de centro y tamaño del SkyBox:
            if (!skyBox.Center.Equals(skyCenterNew) || !skyBox.Center.Equals(skySizeNew)) {
                skyBox.Center = skyCenterNew;
                skyBox.Size = skySizeNew;
                skyBox.updateValues();
            }
        }

        public void heightMapRender() {
            for (int i = 0; i < 9; i++) {
                TgcSimpleTerrain heightMapActual = (TgcSimpleTerrain)heightMaps[i];
                heightMapActual.render();
            }
        }

        public void skyBoxRender() { skyBox.render(); }
    }
}