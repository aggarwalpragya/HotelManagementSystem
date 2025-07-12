using APIHotelManagement.Controllers;
using APIHotelManagement.DTOs;
using APIHotelManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIHotelManagement.Tests
{
    [TestClass]
    public class ReceptionistControllerTests
    {
        private Mock<ILogger<ReceptionistController>> _mockLogger;
        private Mock<IGuest> _mockGuestRepo;
        private Mock<IRoom> _mockRoomRepo;
        private Mock<IReservation> _mockReservationRepo;
        private Mock<IReservationService> _mockReservationServiceRepo;
        private Mock<IPayment> _mockPaymentRepo;
        private Mock<IBill> _mockBillRepo;
        private Mock<IRoomType> _mockRoomTypeRepo;
        private Mock<IService> _mockServiceRepo;
        private ReceptionistController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ReceptionistController>>();
            _mockGuestRepo = new Mock<IGuest>();
            _mockRoomRepo = new Mock<IRoom>();
            _mockReservationRepo = new Mock<IReservation>();
            _mockReservationServiceRepo = new Mock<IReservationService>();
            _mockPaymentRepo = new Mock<IPayment>();
            _mockBillRepo = new Mock<IBill>();
            _mockRoomTypeRepo = new Mock<IRoomType>();
            _mockServiceRepo = new Mock<IService>();

            _controller = new ReceptionistController(
                _mockLogger.Object,
                _mockGuestRepo.Object,
                _mockRoomRepo.Object,
                _mockReservationRepo.Object,
                _mockReservationServiceRepo.Object,
                _mockPaymentRepo.Object,
                _mockBillRepo.Object,
                _mockRoomTypeRepo.Object,
                _mockServiceRepo.Object
            );
        }

        [TestMethod]
        public async Task GetAllGuests_ShouldReturnOk_WithGuestList()
        {
            var mockGuests = new List<GuestVM> { new GuestVM() };
            _mockGuestRepo.Setup(repo => repo.GetAllGuests()).ReturnsAsync(mockGuests);

            var result = await _controller.GetAllGuests();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(((OkObjectResult)result).Value, typeof(List<GuestVM>));
        }

        [TestMethod]
        public async Task GetGuestById_ShouldReturnNotFound_WhenGuestDoesNotExist()
        {
            _mockGuestRepo.Setup(repo => repo.GetGuestById(It.IsAny<int>())).ReturnsAsync((GuestVM)null);
            var result = await _controller.GetGuestById(1);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetGuestById_ShouldReturnOk_WhenGuestExists()
        {
            var mockGuest = new GuestVM { GuestId = 1 };
            _mockGuestRepo.Setup(repo => repo.GetGuestById(1)).ReturnsAsync(mockGuest);
            var result = await _controller.GetGuestById(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task AddGuest_ShouldReturnCorrectResponse_BasedOnRepositoryResponse()
        {
            var guestVM = new GuestVM();
            _mockGuestRepo.Setup(repo => repo.AddGuest(guestVM)).ReturnsAsync(true);
            var successResult = await _controller.AddGuest(guestVM);
            Assert.IsInstanceOfType(successResult, typeof(OkObjectResult));

            _mockGuestRepo.Setup(repo => repo.AddGuest(guestVM)).ReturnsAsync(false);
            var failureResult = await _controller.AddGuest(guestVM);
            Assert.IsInstanceOfType(failureResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SearchAvailableRooms_ShouldReturnBadRequest_WhenInvalidInput()
        {
            var result = await _controller.SearchAvailableRooms(null, DateTime.Now, DateTime.Now.AddDays(1));
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SearchAvailableRooms_ShouldReturnCorrectResponse_BasedOnRoomAvailability()
        {
            string roomTypeName = "Single";
            DateTime checkIn = DateTime.Now;
            DateTime checkOut = DateTime.Now.AddDays(1);

            _mockRoomRepo.Setup(repo => repo.SearchAvailableRooms(roomTypeName, checkIn, checkOut)).ReturnsAsync(new List<RoomVM>());
            var noRoomsResult = await _controller.SearchAvailableRooms(roomTypeName, checkIn, checkOut);
            Assert.IsInstanceOfType(noRoomsResult, typeof(NotFoundObjectResult));

            _mockRoomRepo.Setup(repo => repo.SearchAvailableRooms(roomTypeName, checkIn, checkOut)).ReturnsAsync(new List<RoomVM> { new RoomVM() });
            var availableRoomsResult = await _controller.SearchAvailableRooms(roomTypeName, checkIn, checkOut);
            Assert.IsInstanceOfType(availableRoomsResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task MakeReservation_ShouldReturnCorrectResponse_BasedOnRepositoryResponse()
        {
            var reservationVM = new MakeReservationVM();
            _mockReservationRepo.Setup(repo => repo.MakeReservation(reservationVM)).ReturnsAsync(true);
            var successResult = await _controller.MakeReservation(reservationVM);
            Assert.IsInstanceOfType(successResult, typeof(OkObjectResult));

            _mockReservationRepo.Setup(repo => repo.MakeReservation(reservationVM)).ReturnsAsync(false);
            var failureResult = await _controller.MakeReservation(reservationVM);
            Assert.IsInstanceOfType(failureResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdatePaymentStatus_ShouldReturnCorrectResponse_BasedOnRepositoryResponse()
        {
            _mockPaymentRepo.Setup(repo => repo.UpdatePaymentStatus(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var successResult = await _controller.UpdatePaymentStatus(1, "Paid");
            Assert.IsInstanceOfType(successResult, typeof(OkObjectResult));

            _mockPaymentRepo.Setup(repo => repo.UpdatePaymentStatus(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
            var failureResult = await _controller.UpdatePaymentStatus(1, "Paid");
            Assert.IsInstanceOfType(failureResult, typeof(BadRequestObjectResult));
        }
    }
}
