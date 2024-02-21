namespace DotNetSsh
{
    public class ProfileCreationOptions
    {
        public string Profile { get; set; }
        public string Project { get; set; }
        public string Auth { get; set; }
        public AuthType AuthType { get; set; }
        public bool Verbose { get; set; }
    }
}