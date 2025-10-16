namespace Helpers {
    public static class Log {
        public static void Debug(string message) {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Unity.Logging.Log.Debug(message);
#endif
        }

        public static void Info(string message) {
            Unity.Logging.Log.Info(message);
        }
        
        public static void Warning(string message) {
            Unity.Logging.Log.Warning(message);
        }
        
        public static void Error(string message) {
            Unity.Logging.Log.Error(message);
        }
    }
}