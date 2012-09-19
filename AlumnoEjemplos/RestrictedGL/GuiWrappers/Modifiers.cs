namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    public class Modifiers : GuiControllerWrapper
    {
        public static T get<T>(string key)
        {
            return (T) Gui.Modifiers.getValue(key);
        }        
    }
}
