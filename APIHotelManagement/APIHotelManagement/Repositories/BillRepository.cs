using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Repositories
{
    public class BillRepository : IBill
    {
        private readonly HotelManagementDbContext _context;

        public BillRepository(HotelManagementDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<BillVM>> GetAllBills()
        {
            return _context.Bills
                .Select(b => new BillVM
                {
                    BillId = b.BillId,
                    TotalAmount = b.TotalAmount,
                    Tax = b.Tax,
                    PaymentStatus = b.PaymentStatus.ToString(),
                    BillingDate = b.BillingDate,
                    ReservationId = b.ReservationId,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();
        }

        public async Task<IEnumerable<BillVM>> GetBillsByPaymentStatus(PaymentStatus status)
        {
            return _context.Bills
                .Where(b => b.PaymentStatus == status)
                .Select(b => new BillVM
                {
                    BillId = b.BillId,
                    TotalAmount = b.TotalAmount,
                    Tax = b.Tax,
                    PaymentStatus = b.PaymentStatus.ToString(),
                    BillingDate = b.BillingDate,
                    ReservationId = b.ReservationId,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();
        }

        
        public async Task<BillVM> GetBillById(int billId)
        {
            var bill = _context.Bills.Find(billId);
            if (bill == null) return null;

            return new BillVM
            {
                BillId = bill.BillId,
                TotalAmount = bill.TotalAmount,
                Tax = bill.Tax,
                PaymentStatus = bill.PaymentStatus.ToString(),
                BillingDate = bill.BillingDate,
                ReservationId = bill.ReservationId,
                CreatedAt = bill.CreatedAt,
                UpdatedAt = bill.UpdatedAt
            };
        }

        public async Task<BillDetailVM> IssueBill(int reservationId)
        {
            // Check if the bill already exists for this reservation
            var existingBill = _context.Bills
                .Include(b => b.Reservation)
                    .ThenInclude(r => r.Guest)
                .Include(b => b.Reservation)
                    .ThenInclude(r => r.Room)
                        .ThenInclude(room => room.RoomType)
                .Include(b => b.Reservation)
                    .ThenInclude(r => r.ReservationServices)
                        .ThenInclude(rs => rs.Service)
                .FirstOrDefault(b => b.ReservationId == reservationId);

            // If Bill exists, prepare and return its detail
            if (existingBill != null)
            {
                var reservation = existingBill.Reservation;
                decimal updatedTotalServiceCost = 0;
                List<string> serviceDetails = new List<string>();

                foreach (var rs in reservation.ReservationServices)
                {
                    if (rs.Service != null)
                    {
                        decimal serviceTotal = rs.Service.Price * rs.Quantity;
                        updatedTotalServiceCost += serviceTotal;
                        serviceDetails.Add($"{rs.Service.ServiceName} - {rs.Quantity} x {rs.Service.Price} = {serviceTotal}");
                    }
                }

                // Calculate the number of days for stay
                int numberOfDays = (reservation.CheckOut - reservation.CheckIn).Days;
                numberOfDays = numberOfDays > 0 ? numberOfDays : 1; // Ensure at least 1 day

                decimal upadtedTotalRoomCharge = reservation.Room.RoomType.RoomRate * numberOfDays;

                decimal upadtedFinalAmount = upadtedTotalRoomCharge + updatedTotalServiceCost;

                UpdateBillTotalAmount(existingBill.BillId, upadtedFinalAmount);

                PaymentRepository payObj = new PaymentRepository(_context);
                payObj.UpdatePaymentAmount(existingBill.BillId, upadtedFinalAmount);

                return new BillDetailVM
                {
                    BillId = existingBill.BillId,
                    GuestName = reservation.Guest.GuestName,
                    IdProofNumber = reservation.Guest.IdProofNumber,
                    ContactNumber = reservation.Guest.ContactNumber,
                    RoomTypeName = reservation.Room.RoomType.RoomTypeName,
                    RoomRate = reservation.Room.RoomType.RoomRate,
                    CheckIn = reservation.CheckIn,
                    CheckOut = reservation.CheckOut,
                    Services = serviceDetails,
                    TotalRoomCharges = upadtedTotalRoomCharge,
                    TotalServiceCharges = updatedTotalServiceCost,
                    FinalAmount = upadtedFinalAmount,
                    BillingDate = (DateTime)existingBill.BillingDate,
                    PaymentStatus = existingBill.PaymentStatus.ToString()
                };
            }

            // If no bill exists, generate new
            var reservationData = _context.Reservations
                .Include(r => r.Room).ThenInclude(room => room.RoomType)
                .Include(r => r.Guest)
                .Include(r => r.ReservationServices).ThenInclude(rs => rs.Service)
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservationData == null || reservationData.Room == null || reservationData.Room.RoomType == null)
                throw new Exception("Reservation, Room, or RoomType not found.");

            // Calculate the number of days for stay
            int stayDays = (reservationData.CheckOut - reservationData.CheckIn).Days;
            stayDays = stayDays > 0 ? stayDays : 1; // Ensure at least 1 day

            decimal roomRate = reservationData.Room.RoomType.RoomRate;
            decimal totalRoomCharges = roomRate * stayDays;
            decimal totalServiceCostNew = 0;
            List<string> newServiceDetails = new List<string>();

            foreach (var rs in reservationData.ReservationServices)
            {
                if (rs.Service != null)
                {
                    decimal serviceTotal = rs.Service.Price * rs.Quantity;
                    totalServiceCostNew += serviceTotal;
                    newServiceDetails.Add($"{rs.Service.ServiceName} - {rs.Quantity} x {rs.Service.Price} = {serviceTotal}");
                }
            }

            decimal finalAmount = totalRoomCharges + totalServiceCostNew;

            // Create new bill
            var newBill = new Bill
            {
                ReservationId = reservationId,
                TotalAmount = finalAmount,
                PaymentStatus = PaymentStatus.Pending,
                BillingDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bills.Add(newBill);
            await _context.SaveChangesAsync();

            // Create Payment Entry
            var payment = new Payment
            {
                ReservationId = reservationId,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Return the detailed bill response
            return new BillDetailVM
            {
                BillId = newBill.BillId,
                GuestName = reservationData.Guest.GuestName,
                IdProofNumber = reservationData.Guest.IdProofNumber,
                ContactNumber = reservationData.Guest.ContactNumber,
                RoomTypeName = reservationData.Room.RoomType.RoomTypeName,
                RoomRate = roomRate,
                CheckIn = reservationData.CheckIn,
                CheckOut = reservationData.CheckOut,
                Services = newServiceDetails,
                TotalRoomCharges = totalRoomCharges,
                TotalServiceCharges = totalServiceCostNew,
                FinalAmount = finalAmount,
                BillingDate = DateTime.UtcNow,
                PaymentStatus = newBill.PaymentStatus.ToString()
            };
        }

        // update bill amount (internal call)
        private void UpdateBillTotalAmount(int billId, decimal updatedFinalAmount)
        {
            var bill = _context.Bills.Find(billId);

            if (bill == null)
            {
                throw new Exception("Bill not found.");
            }

            // Update the bill's total amount
            bill.TotalAmount = updatedFinalAmount;
            bill.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }


        public async Task<bool> UpdateBill(int billId, BillVM billVM)
        {
            var bill = await _context.Bills.FindAsync(billId);
            if (bill == null) return false;

            bill.TotalAmount = billVM.TotalAmount;
            bill.Tax = billVM.Tax;
            bill.PaymentStatus = Enum.Parse<PaymentStatus>(billVM.PaymentStatus, true);
            bill.BillingDate = billVM.BillingDate;
            bill.UpdatedAt = DateTime.UtcNow;

            PaymentRepository payObj = new PaymentRepository(_context);
            payObj.UpdatePaymentStatus(billId, bill.PaymentStatus.ToString());

            return _context.SaveChanges() > 0;
        }

       
        public async Task<bool> DeleteBill(int billId)
        {
            var bill = await _context.Bills.FindAsync(billId);
            if (bill == null) return false;

            _context.Bills.Remove(bill);

            PaymentRepository payObj = new PaymentRepository(_context);
            payObj.DeletePayment(billId);

            return _context.SaveChanges() > 0;
        }
    }
}
