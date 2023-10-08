namespace CargoManagement.Models.Journey
{
    public class JourneyDeliveryNoteDTO
    {        
        public string? ConsigneeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int JourneyId { get; set; }
        public int TotalPackages { get; set; }        
        public string? DeliveryIncharge { get; set; }
        public string? Mobile { get; set; }
        public string? Items { get; set; }
        public int JourneyItemId { get; set; }
        public string? BookingId { get; set; }
        public string? ConsignorsName { get; set; }
        public string? BoxType { get; set; }
        public string? Description { get; set; }
        public string? JobNo { get; set; }
        public string? DoNo { get; set; }
        public string? Remarks { get; set; }
        public int? NoOfPackages { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        //public List<JourneyDeliveryItemListDTO> ItemList { get; set; }
    }
    public class JourneyDeliveryItemListDTO
    {
        public int JourneyItemId { get; set; }       
        public string BookingId { get; set; }
        public string? ConsignorsName { get; set; }
        public string BoxType { get; set; }
        public string Description { get; set; }
        public int NoOfPackages { get; set; }
        public decimal? Amount { get; set; }
    }

    public class InputDeliverNoteDTO
    {
        public int JourneyItemId { get; set; }
        public int journeyId { get; set; }
    }   

}
