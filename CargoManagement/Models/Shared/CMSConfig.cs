namespace CargoManagement.Models.Shared
{
    public class CMSConfig
    {
        public string FileDownloadUrl { get; set; }
        public string ProfileFolder { get; set; }
        public string ProfileFolderAlias { get; set; }
        public string BookingDocsFolder { get; set; }
        public string BookingDocsFolderAlias { get; set; }
        public string TokenKey { get; set; }
        public string TokenValidity { get; set; }
        public string TokenIssuer { get; set; }
        public string LastBookingId { get; set; }

    }
}
