namespace CargoManagement.Models.Shared
{
    public class DropDownDTO
    {
        public int Value { get; set; }
        public string Text { get; set; }

    }
    public class BoxTypeDropDownDTO
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public decimal UnitPrice { get; set; }

    }

    public class BookingDropDownDTO
    {
        public int BookingItemID { get; set; }
        public string Text { get; set; }
        public decimal Id { get; set; }

    }

}
