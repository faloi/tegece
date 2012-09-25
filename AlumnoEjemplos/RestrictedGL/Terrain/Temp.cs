/*
    Device d3dDevice = GuiController.Instance.D3dDevice;
    this.center = center;

    //Dispose de VertexBuffer anterior, si habia
    if (vbTerrain != null && !vbTerrain.Disposed) {
        vbTerrain.Dispose();
    }

    //Cargar heightmap
    heightmapData = loadHeightmapValues(d3dDevice, heightmapPath);
    float width = (float)heightmapData.GetLength(0);
    float length = (float)heightmapData.GetLength(1);

    //Crear vertexBuffer
    totalVertices = 2 * 3 * (heightmapData.GetLength(0) - 1) * (heightmapData.GetLength(1) - 1); //por cada puntito tengo dos triángulos de 3 vértices
    vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

    //Cargar vertices
    int dataIdx = 0;
    CustomVertex.PositionTextured[] data = new CustomVertex.PositionTextured[totalVertices];

    //Convertir el center que me pasaron en el vértice de arriba a la izquierda
    center.X = center.X * scaleXZ - (width / 2) * scaleXZ;
    center.Y = center.Y * scaleY;
    center.Z = center.Z * scaleXZ - (length / 2) * scaleXZ;
    
     * xxx        cxx
     * xcx   =>   xxx   (c = center)
     * xxx        xxx
    

    for (int i = 0; i < width - 1; i++) {
        for (int j = 0; j < length - 1; j++) {
             Para todos los pixeles del heightmap, menos el último, armar los dos triángulos de su derecha
             * v1 - v3
             *  | / |
             * v2 - v4
             * (los pixeles terminan siendo los vértices de los triángulos)
            

            //Vertices
            Vector3 v1 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j] * scaleY, center.Z + j * scaleXZ);
            Vector3 v2 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);
            Vector3 v3 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j] * scaleY, center.Z + j * scaleXZ);
            Vector3 v4 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);

            //Coordendas de textura
            Vector2 t1 = new Vector2(i / width, j / length);
            Vector2 t2 = new Vector2(i / width, (j + 1) / length);
            Vector2 t3 = new Vector2((i + 1) / width, j / length);
            Vector2 t4 = new Vector2((i + 1) / width, (j + 1) / length);

            //Cargar triangulo 1
            data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
            data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
            data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

            //Cargar triangulo 2
            data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
            data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
            data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

            dataIdx += 6;
        }
    }

    vbTerrain.SetData(data, 0, LockFlags.None);
*/