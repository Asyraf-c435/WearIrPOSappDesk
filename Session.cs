using System;

namespace KasirWearIt
{
    public static class Session
    {
        public static int IdUser { get; set; }
        public static string NamaLengkap { get; set; } = string.Empty;
        public static string Username { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static int IdOutlet { get; set; }
        public static bool IsLoggedIn { get; set; } = false;
        
        public static void ClearSession()
        {
            IdUser = 0;
            NamaLengkap = string.Empty;
            Username = string.Empty;
            Role = string.Empty;
            IdOutlet = 0;
            IsLoggedIn = false;
        }
    }
}