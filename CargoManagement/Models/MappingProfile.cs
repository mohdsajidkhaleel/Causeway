using AutoMapper;
using CargoManagement.Models.Authentication;
using CargoManagement.Models.Booking;
using CargoManagement.Models.BookinItems;
using CargoManagement.Models.BoxType;
using CargoManagement.Models.CustomerAddress;
using CargoManagement.Models.Customers;
using CargoManagement.Models.CustomerTransactions;
using CargoManagement.Models.Expense;
using CargoManagement.Models.ExpenseType;
using CargoManagement.Models.Hubs;
using CargoManagement.Models.Journey;
using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Models.User;
using CargoManagement.Models.Usertype;
using CargoManagement.Repository;

namespace CargoManagement.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<UserCreationDTO, CargoManagement.Repository.User>();
            CreateMap<UserUpdateDTO, CargoManagement.Repository.User>();
            CreateMap<CargoManagement.Repository.User, UserResponseDTO>()
                .ForMember(x => x.HubName, opt => opt.MapFrom(src => src.Hub.Name))
                .ForMember(x => x.UserRoleId, opt => opt.MapFrom(src => src.UserRole.Id))
                .ForMember(x => x.UserRoleName, opt => opt.MapFrom(src => src.UserRole.UserRoleName))
                .ForMember(x => x.UserTypeName, opt => opt.MapFrom(src => src.UserType.Name));
            CreateMap<CargoManagement.Repository.User, MyProfileDTO>();


            //User Type
            CreateMap<CargoManagement.Repository.Usertype, UserTypeResponse>();

            //Hub
            CreateMap<HubCreationDTO, Hub>();
            CreateMap<HubUpdationDTO, Hub>();
            CreateMap<Hub, HubResponseDTO>();
            CreateMap<Hub, HubDetailsDTO>()
                .ForMember(dto => dto.State, opt => opt.MapFrom(repo => repo.State.Name))
                .ForMember(dto => dto.District, opt => opt.MapFrom(repo => repo.District.Name))
                .ForMember(dto => dto.Location, opt => opt.MapFrom(repo => repo.Location.Name))
                .ForMember(dto => dto.HubType, opt => opt.MapFrom(repo => repo.HubType.Name));

            //Customer Address
            CreateMap<CustomerAddressCreationDTO, Customeraddress>();
            CreateMap<Customeraddress, CustomerAdderssResponseDTO>();
            CreateMap<Customeraddress, CustomerAddressDetailsDTO>()
                .ForMember(dto => dto.State, opt => opt.MapFrom(src => src.State.Name))
                .ForMember(dto => dto.District, opt => opt.MapFrom(src => src.District.Name))
                .ForMember(dto => dto.State, opt => opt.MapFrom(src => src.State.Name))
                .ForMember(dto => dto.Location, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dto => dto.Pincode, opt => opt.MapFrom(src => src.Location.Pincode));
            CreateMap<CustomerAddressUpdationDTO, Customeraddress>();

            //Customer 
            CreateMap<CustomerCreationDTO, Customer>()
                .ForMember(x => x.Customeraddresses, opt => opt.MapFrom(src => src.Address));
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customeraddresses));
            CreateMap<Customer, DropdownCustomerResponseDTO>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customeraddresses));
            CreateMap<Customer, CustomerDetailsDTO>()
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customeraddresses));
            CreateMap<CustomerUpdationDTO, Customer>();

            //CustomerTransactions
            CreateMap<Customertransaction, CustomerTransactionsDTO>();

            //BookingItems  
            CreateMap<BookingItemsCreationDTO, CargoManagement.Repository.Bookingitem>();
            CreateMap<CargoManagement.Repository.Bookingitem, ViewBookingStatusResponseDTO>()
                .ForMember(dto => dto.BoxTypeName, opt => opt.MapFrom(item => item.BoxType.Name));
            CreateMap<CargoManagement.Repository.Bookingitem, BookingItemsResponseDTO>()
                .ForMember(dto => dto.RemainingQty, opt => opt.MapFrom(item => item.Quantity - (item.PlannedQty + item.InTransitQty + item.ReceivedQty + item.DeliveredQty)));
            CreateMap<CargoManagement.Repository.Bookingitem, BookingItemsListDTO>()
                .ForMember(dto => dto.BoxTypeName, opt => opt.MapFrom(item => item.BoxType.Name))
                .ForMember(dto => dto.RemainingQty, opt => opt.MapFrom(item => item.Quantity - (item.PlannedQty + item.InTransitQty + item.ReceivedQty + item.DeliveredQty)));

            //Booking
            CreateMap<BookingCreationDTO, CargoManagement.Repository.Booking>()
                .ForMember(entity => entity.Bookingitems, opt => opt.MapFrom(dto => dto.BookingItems));
            CreateMap<CargoManagement.Repository.Booking, BookingResponseDTO>()
                .ForMember(dto => dto.BookingItems, opt => opt.MapFrom(entity => entity.Bookingitems));
            CreateMap<CargoManagement.Repository.Booking, BookingListDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                 .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.CustomerAddress.Address
                + ", " + src.CustomerAddress.Location.Name
                + ", " + src.CustomerAddress.District.Name
                + "-" + src.CustomerAddress.Location.Pincode
                ))
                .ForMember(dest => dest.BookingItemDescription, opt => opt.MapFrom(src => src.Bookingitems.Select(x => x.Description).FirstOrDefault()))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Customer.Mobile))
                .ForMember(dest => dest.ReceipientName, opt => opt.MapFrom(src => src.ReceipientCustomer.Name))
                .ForMember(dest => dest.ReceipientAddress, opt => opt.MapFrom(src => src.ReceipientCustomerAddress.Address
                + ", " + src.ReceipientCustomerAddress.Location.Name
                + ", " + src.ReceipientCustomerAddress.District.Name
                + "-" + src.ReceipientCustomerAddress.Location.Pincode
                ))
                .ForMember(dest => dest.CurrentHubName, opt => opt.MapFrom(src => src.CurrentHub != null ? src.CurrentHub.Name : ""))
                .ForMember(dest => dest.IsPaymentCompleted, opt => opt.MapFrom(src => src.PaidDate == null ? false : true))
                .ForMember(dest => dest.TotalBoxCount, opt => opt.MapFrom(src => src.Bookingitems.Sum(items => items.Quantity)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Journey, opt => opt.MapFrom(src => src.Journey.Name))
                .ForMember(dest => dest.NetAmnount, opt => opt.MapFrom(src => src.Bookingitems.Select(x=> x.UnitPrice).FirstOrDefault()));
            //.ForMember(dest => dest.PaymentModeName, opt => opt.MapFrom(src => mapPaymentType( src.PaymentMode)));
            CreateMap<CargoManagement.Repository.Booking, BookingDetailsDTO>()
                .ForMember(dto => dto.BookingItems, opt => opt.MapFrom(entity => entity.Bookingitems))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Customer.Mobile))
                .ForMember(dest => dest.ReceipientName, opt => opt.MapFrom(src => src.ReceipientCustomer.Name))
                .ForMember(dest => dest.ReceipientAddress, opt => opt.MapFrom(src => src.ReceipientCustomer.Customeraddresses.FirstOrDefault()))
                .ForMember(dest => dest.OriginHubName, opt => opt.MapFrom(src => src.OriginHub != null ? src.OriginHub.Name : ""))
                .ForMember(dest => dest.IsPaymentCompleted, opt => opt.MapFrom(src => src.PaidDate == null ? false : true))
                .ForMember(dest => dest.TotalBoxCount, opt => opt.MapFrom(src => src.Bookingitems.Sum(items => items.Quantity)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.PaymentModeName, opt => opt.MapFrom(src => mapPaymentType(src.PaymentMode)))
                .ForMember(dest => dest.PaymentMode, opt => opt.MapFrom(src => src.PaymentMode))
                .ForMember(dest => dest.Journey, opt => opt.MapFrom(src => src.Journey.Name))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Recepient, opt => opt.MapFrom(src => src.ReceipientCustomer));


            //Bookingtransaction
            CreateMap<Bookingtransaction, BookingTransactionsDTO>()
                 .ForMember(dto => dto.StatusDescription, opt => opt.MapFrom(entity => generateHistoryStatus(entity)));


            //Journey Details
            CreateMap<JourneyItemsCreationDTO, CargoManagement.Repository.Journeyitem>();
            CreateMap<CargoManagement.Repository.Journeyitem, JourneyItemsResponseDTO>()
                 .ForMember(dest => dest.BookingCode, opt => opt.MapFrom(src => src.BookingItem.Booking.BookingId))
                 .ForMember(dest => dest.JourneyName, opt => opt.MapFrom(src => src.Journey.Name))
                 .ForMember(dest => dest.DateOfJourney, opt => opt.MapFrom(src => src.Journey.DateOfJourney))
                 .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingItem.Booking.Id))
                 .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.BookingItem.Booking.PaidDate))
                 .ForMember(dest => dest.IsPaymentSuccessfull, opt => opt.MapFrom(src => IsPaymentSuccessfull(src)))
                 .ForMember(dest => dest.BoxTypeId, opt => opt.MapFrom(src => src.BookingItem.BoxTypeId))
                 .ForMember(dest => dest.DestinationHubName, opt => opt.MapFrom(src => src.DestinationHub.Name))
                 .ForMember(dest => dest.BookingItem, opt => opt.MapFrom(src => src.BookingItem))
                 .ForMember(dest => dest.AmountToPay, opt => opt.MapFrom(src => src.PaymentMode == "T" ? Convert.ToString((src.Quantity * src.BookingItem.UnitPrice) + src.BookingItem.Booking.FreightCharges) : "0"));

            //Journey
            CreateMap<JourneyCreationDTO, CargoManagement.Repository.Journey>()
                .ForMember(dest => dest.Journeyitems, opt => opt.MapFrom(src => src.Items));
            CreateMap<CargoManagement.Repository.Journey, JourneyResponseDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Journeyitems));
            CreateMap<CargoManagement.Repository.Journey, JourneyListDTO>()
               //.ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name))
               .ForMember(dest => dest.DestinationHubName, opt => opt.MapFrom(src => src.DestinationHub.Name))
               .ForMember(dest => dest.OriginHubName, opt => opt.MapFrom(src => src.OriginHub.Name))
               .ForMember(dest => dest.DeliveryCount, opt => opt.MapFrom(src => src.Journeyitems.Where(items => items.Action == JourneyShipmentAction.Delivery).Count()))
               .ForMember(dest => dest.PickupCount, opt => opt.MapFrom(src => src.Journeyitems.Where(items => items.Action == JourneyShipmentAction.Pickup).Count()))
               .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status == "S" ? "Scheduled" : (src.Status == "I" ? "In Transit" : (src.Status == "E" ? "Ended" : (src.Status == "C" ? "Cancelled" : "Undefined")))));

            //Expense
            CreateMap<CargoManagement.Repository.Journeyexpense, GetExpenseResponseDTO>()
                .ForMember(dest => dest.ExpenseTypeName, opt => opt.MapFrom(src => src.ExpenseType.Name));

            //Box types
            CreateMap<CargoManagement.Repository.Boxtype, BoxTypeResponseDTO>();

            //Box types
            CreateMap<Bookingfile, BookingFileDTO>();

            //Expense types
            CreateMap<Expensetype, ExpenseTypeCreationDTO>();
            CreateMap<ExpenseTypeCreationDTO, Expensetype>();

            //Journey Expense types
            CreateMap<Journeyexpense, JourneyExpenseCreationDTO>();
            CreateMap<JourneyExpenseCreationDTO, Journeyexpense>();


            //Bookingitemsdistribution
            CreateMap<Bookingitemsdistribution, BookingItemDistributionListDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingItem.Id))
               .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingItem.Booking.BookingId))
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.BookingItem.Booking.Customer.Name))
               .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.BookingItem.Booking.CustomerAddress.Address))
               .ForMember(dest => dest.ReceipientName, opt => opt.MapFrom(src => src.BookingItem.Booking.ReceipientCustomer.Name))
               .ForMember(dest => dest.ReceipientAddress, opt => opt.MapFrom(src => src.BookingItem.Booking.ReceipientCustomerAddress.Address))
               .ForMember(dest => dest.TotalBoxCount, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.BoxType, opt => opt.MapFrom(src => src.BookingItem.BoxType.Name))
               .ForMember(dest => dest.BoxDescription, opt => opt.MapFrom(src => src.BookingItem.Description))
               .ForMember(dest => dest.ItemDistributionId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.BookingItem.Booking.Customer.Mobile));

        }

        private string mapJourneyStatus(string status)
        {
            switch (status)
            {
                case "S": return "Scheduled"; break;
                case "I": return "In Transit"; break;
                case "E": return "Ended"; break;
                default: return "Undefined"; break;
            }
        }
        private string mapPaymentType(string status)
        {
            switch (status)
            {
                case "S": return "Self"; break;
                case "C": return "Credit"; break;
                case "T": return "ToPay"; break;
                default: return "Undefined"; break;
            }
        }        
        private string generateHistoryStatus(Bookingtransaction entry)
        {
            if ((Convert.ToBoolean(entry.Status.IsHub)) == true)
            {
                return entry.Status.Name + "(" + entry.CurrentHub.Name + ")";
            }
            else if ((Convert.ToBoolean(entry.Status.IsJourney)) == true)
            {
                return entry.Status.Name + (entry.NextHub != null ? "(to :" + entry.NextHub.Name + ")" : "(Customer)") + " in journey-" + entry.Journey.Name;
            }

            return entry.Status.Name;
        }
        private bool? IsPaymentSuccessfull(Journeyitem src)
        {
            if (src.BookingItem.Booking.PaymentMode == "S")
            {
                return true;
            }
            else if(src.BookingItem.Booking.PaymentMode == "T" && src.BookingItem.Booking.PaidDate != null)
            {
                return true;
            }
                return null;
        }


    }
}
