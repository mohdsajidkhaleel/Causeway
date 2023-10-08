namespace CargoManagement.Models.Shared
{
    public static class BookingStatus
    {
        public const string New = "NB";
        public const string AssignedForPickUP = "AP";
        public const string PickedUp = "PU";
        public const string CollectedForShipment = "CS";
        public const string AssignForTransit = "AT";
        public const string InTransit = "IT";
        public const string AssignedForDelivery = "AD";
        public const string OutForDelivery = "OD";
        public const string ReturnedToConsignor = "CR";
        public const string DeliveredShipment = "DS";
        public const string CustomerPaymentCollected = "CP";
        public const string RemovedFromJourney = "RJ";
        public const string DeliverToHub = "DH";
        public const string PartialDelivered = "PD"; // will confirm and change the Name. This status used when booking have multiple Statuses but not yet delivered fully.
    }
    public static class PaymentStatus
    {
        public const string Self = "S";
        public const string Credit = "C";
        public const string ToPay = "T";
    }

    public static class JourneyStatus
    {
        public const string Scheduled = "S";
        public const string InTransit = "I";
        public const string Ended = "E";
        public const string Cancelled = "C";
        public const string Delivered = "D";
        public const string PartiallyDelivered = "PD";
    }
    public static class JourneyShipmentAction
    {
        public const string Delivery = "D";
        public const string Pickup = "P";
    }
    public static class JourneyShipmentStatus
    {
        public const string Delivered = "D";
        public const string Pickedup = "P";
        public const string Scheduled = "S";
        public const string Received = "R";
        public const string InTransit = "I";
        public const string Cancelled = "C";
    }
}
