namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    class UserVars : GuiControllerWrapper
    {
        public static void add(string key)
        {
            Gui.UserVars.addVar(key);
        }

        public static void addMany(params string[] keys)
        {
            foreach (var key in keys)
                UserVars.add(key);
        }
    }
}
