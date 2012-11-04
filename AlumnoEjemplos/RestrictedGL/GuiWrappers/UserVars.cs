namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    static class UserVars
    {
        public static void add(string key) {
            Gui.I.UserVars.addVar(key);
        }

        public static void addMany(params string[] keys) {
            foreach (var key in keys)
                UserVars.add(key);
        }

        public static void set(string key, object value) {
            Gui.I.UserVars.setValue(key, value);
        }
    }
}
