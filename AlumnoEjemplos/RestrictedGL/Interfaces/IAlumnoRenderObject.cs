namespace AlumnoEjemplos.RestrictedGL.Interfaces {
    
    public interface IAlumnoRenderObject {
        void init(string alumnoMediaFolder);
        void render(float elapsedTime);
        void dispose();
    }

}