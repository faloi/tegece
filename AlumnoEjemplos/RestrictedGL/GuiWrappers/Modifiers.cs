namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    public static class Modifiers
    {
        public static T get<T>(string key)
        {
            return (T) Gui.I.Modifiers.getValue(key);
        }        

        public static bool showBoundingBox() {
            return get<bool>("ShowBoundingBox");
        }
    }
}
