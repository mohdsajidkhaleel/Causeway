using System.ComponentModel.DataAnnotations;
namespace CargoManagement.Models.Journey
{
    public class JourneyUpdateUnitPriceDTO
    {
        [Required]
        public int JourneyId { get; set; }
        [Required]
        public List<UpdateUnitPriceDetails> UnitPriceDetails { get; set; }
    }

    public class UpdateUnitPriceDetails
    {
        [Required]
        public int BookingItemId { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
    }

    public class JourneyStartExportDetailsDTO
    {
        public int? JourneyId { get; set; }
        public int? JourneyItemId { get; set; }
        public DateTime? JourneyDate { get; set; }
        public string? JourneyName { get; set; }
        public string? BookingId { get; set; }        
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public string? ContactNumber { get; set; }
        public int? Packages { get; set; }
        public string? ItemName { get; set; }
        public string? Notes { get; set; }
        public Decimal? CollectionCharges { get; set; }
    }

    public class PayLaterDetailsDTO
    {
        [Required]
        public int PayLaterCustomerId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        public int JourneyId { get; set; }
         [Required]
        public int JourneyItemId { get; set; }

    }

    public class GetJourneyforUnitPriceDTO
    {
        public int JourneyId { get; set; }
        public string? JourneyName { get; set; }
        public int? JourneyItemId { get; set; }
        public int? BookingId { get; set; }
        public string? BookingCode { get; set; }        
        public string? BookingItemNotes { get; set; }        
        public int? BookingItemId { get; set; }    
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public int TotalQuantity { get; set; }
        public int DispatchedQuantity { get; set; }
        public string Boxtype { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool? IsEditable { get; set; }

    }

    public class ManifestReportFilterDTO {
        public DateTime DateofDispatch { get; set; }
        public string JourneyName { get; set; }
        public int JourneyID { get; set; }
    }
    
}
