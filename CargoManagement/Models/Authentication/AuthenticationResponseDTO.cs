namespace CargoManagement.Models.Authentication
{
    public class AuthenticationResponseDTO
    {
        public string token { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Image { get; set; }
        public bool IsAdminUser { get; set; }
        public int? UserRoleId { get; set; }
    }
}
