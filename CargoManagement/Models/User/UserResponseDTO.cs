namespace CargoManagement.Models.User
{
    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? AlternativeMobile { get; set; }
        public string? Image { get; set; }
        public int? UserTypeId { get; set; }
        public int? HubId { get; set; }
        public string? UserTypeName { get; set; }
        public string? UserRoleName { get; set; }
        public int? UserRoleId { get; set; }
        public string? HubName { get; set; }

    }
}
