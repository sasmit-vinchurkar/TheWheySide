using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWheySide.Models;
using TheWheySide.ViewModel;

namespace TheWheySide.Controllers
{
    public class RoomController : Controller
    {
        private readonly TheWheySideDBEntities theWheySideDBEntities;
        public RoomController()
        {
            theWheySideDBEntities = new TheWheySideDBEntities();
        }
        // GET: Room
        public ActionResult Index()
        {
            RoomViewModel roomViewModel = new RoomViewModel();

            roomViewModel.ListOfBookingSatus = (from obj in theWheySideDBEntities.BookingStatus
                                                select new SelectListItem()
                                                {
                                                    Text = obj.BookingStatus,
                                                    Value = obj.BookingStatusId.ToString()
                                                }).ToList();

            roomViewModel.ListOfRoomType = (from obj in theWheySideDBEntities.RoomTypes
                                            select new SelectListItem()
                                            {
                                                Text = obj.RoomTypeName,
                                                Value = obj.RoomTypeId.ToString()
                                            }).ToList();    
            return View(roomViewModel);
        }

        [HttpPost]
        public ActionResult Index(RoomViewModel roomViewModel)
        {
            string ImageUniqueName = Guid.NewGuid().ToString();
            string ActualImageName = ImageUniqueName + Path.GetExtension(roomViewModel.Image.FileName);

            roomViewModel.Image.SaveAs(Server.MapPath("~/RoomImages/" + ActualImageName));
            //DBEntities
            Room room = new Room()
            {
                RoomNumber = roomViewModel.RoomNumber,
                RoomDescription = roomViewModel.RoomDescription,
                RoomPrice = roomViewModel.RoomPrice,
                BookingStatusId = roomViewModel.BookingStatusId,
                RoomTypeId = roomViewModel.RoomTypeId,
                IsActive = true,
                RoomImage = ActualImageName,
                RoomCapacity = roomViewModel.RoomCapacity,
              
            };
            theWheySideDBEntities.Rooms.Add(room);
            theWheySideDBEntities.SaveChanges();
            return Json(data: new {message="Room Sucessfully Added.", success = true }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetAllRooms()
        {
            IEnumerable<RoomDetailsViewModel> listroomDetailsViewModels =
                (from objRoom in theWheySideDBEntities.Rooms
                 join objBooking in theWheySideDBEntities.BookingStatus on objRoom.BookingStatusId equals objBooking.BookingStatusId
                 join objRoomType in theWheySideDBEntities.RoomTypes on objRoom.RoomTypeId equals objRoomType.RoomTypeId
                 select new RoomDetailsViewModel()
                 {
                     RoomNumber = objRoom.RoomNumber,
                     RoomDescription = objRoom.RoomDescription,
                     RoomCapacity = objRoom.RoomCapacity,
                     RoomPrice = objRoom.RoomPrice,
                     BookingStatus = objBooking.BookingStatus,
                     RoomType = objRoomType.RoomTypeName,
                     RoomImage = objRoom.RoomImage,
                     RoomId = objRoom.RoomId

                 }).ToList();

            return PartialViewResult("_RoomDetailsPartial", listroomDetailsViewModels);
        }
    }
}