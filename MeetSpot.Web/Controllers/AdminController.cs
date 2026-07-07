using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class AdminController : BaseController
{
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly RoomApiService _roomApiService;
    private readonly UserApiService _userApiService;
    private readonly BookingApiService _bookingApiService;
    private readonly ReviewApiService _reviewApiService;
    private readonly HeroSlideApiService _heroSlideApiService;

    public AdminController(
        AnalyticsApiService analyticsApiService,
        RoomApiService roomApiService,
        UserApiService userApiService,
        BookingApiService bookingApiService,
        ReviewApiService reviewApiService,
        HeroSlideApiService heroSlideApiService)
    {
        _analyticsApiService = analyticsApiService;
        _roomApiService = roomApiService;
        _userApiService = userApiService;
        _bookingApiService = bookingApiService;
        _reviewApiService = reviewApiService;
        _heroSlideApiService = heroSlideApiService;
    }

    private IActionResult CheckAdmin()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");
        if (ViewBag.UserRole != "Admin")
            return RedirectToAction("Index", "Home");
        return null!;
    }

    public async Task<IActionResult> Dashboard()
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var token = Request.Cookies["AccessToken"]!;
        var stats = await _analyticsApiService.GetPublicStatsAsync();

        ViewBag.Stats = stats;
        return View();
    }

    public async Task<IActionResult> Rooms()
    {
        var check = CheckAdmin();
        if (check != null)
            return check;

        var rooms = await _roomApiService.GetAllRoomsAsync();
        return View(rooms);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _roomApiService.DeleteRoomAsync(token, id);
        return RedirectToAction("Rooms");
    }

    public async Task<IActionResult> Bookings()
    {
        var check = CheckAdmin();
        if (check != null)
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var bookings = await _bookingApiService.GetAllBookingsAsync(token);
        return View(bookings);
    }

    public async Task<IActionResult> Users()
    {
        var check = CheckAdmin();
        if (check != null)
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var users = await _userApiService.GetAllUsersAsync(token);
        return View(users);
    }

    public async Task<IActionResult> Reviews()
    {
        var check = CheckAdmin();
        if (check != null)
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var reviews = await _reviewApiService.GetAllReviewsAsync(token);
        return View(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.ApproveReviewAsync(token, id);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.DeleteReviewAsync(token, id);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> SetFeatured(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.SetFeaturedAsync(token, id, true);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> UnfeatureReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.SetFeaturedAsync(token, id, false);
        return RedirectToAction("Reviews");
    }

    [HttpGet]
    public async Task<IActionResult> EditRoom(int id)
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var room = await _roomApiService.GetRoomByIdAsync(id);
        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> EditRoom(int id, string name, string description, int floor, int capacity,decimal pricePerHour, string type)
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var token = Request.Cookies["AccessToken"]!;
        var dto = new { name, description, floor, capacity, pricePerHour, type };
        await _roomApiService.UpdateRoomAsync(token, id, dto);

        TempData["SuccessMessage"] = "Room updated successfully!";
        return RedirectToAction("Rooms");
    }

    public async Task<IActionResult> HeroSlides()
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var token = Request.Cookies["AccessToken"]!;
        var slides = await _heroSlideApiService.GetAllSlidesAsync(token);
        return View(slides);
    }

    [HttpPost]
    public async Task<IActionResult> AddSlide(IFormFile? imageFile, string? imageUrl, int displayOrder, string? caption)
    {
        var token = Request.Cookies["AccessToken"]!;
        string? finalImageUrl = null;

        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Only jpg, png, webp allowed";
                return RedirectToAction("HeroSlides");
            }

            var fileName = $"hero_{Guid.NewGuid()}{extension}";
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "hero");
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            finalImageUrl = $"/images/hero/{fileName}";
        }
        else if (!string.IsNullOrEmpty(imageUrl))
        {
            finalImageUrl = imageUrl;
        }
        
        if (!string.IsNullOrEmpty(finalImageUrl))
        {
            await _heroSlideApiService.CreateSlideAsync(token, finalImageUrl, displayOrder, caption);
            TempData["Success"] = "Slide added successfully!";
        }
        else
        {
            TempData["Error"] = "Please provide an image file or URL";
        }

        return RedirectToAction("HeroSlides");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSlide(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _heroSlideApiService.DeleteSlideAsync(token, id);
        return RedirectToAction("HeroSlides");
    }

    public async Task<IActionResult> Messages()
    {
        var check = CheckAdmin();
        if (check != null) 
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var contactApiService = HttpContext.RequestServices.GetRequiredService<ContactApiService>();
        var messages = await contactApiService.GetAllMessagesAsync(token);
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> EditSlide(int id)
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var token = Request.Cookies["AccessToken"]!;
        var slides = await _heroSlideApiService.GetAllSlidesAsync(token);
        var slide = slides.FirstOrDefault(s => s.Id == id);
        if (slide == null) return NotFound();

        return View(slide);
    }

    [HttpPost]
    public async Task<IActionResult> EditSlide(int id, IFormFile imageFile, int displayOrder, string? caption, bool isActive)
    {
        var token = Request.Cookies["AccessToken"]!;
        string? finalImageUrl = null;

        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();

            if (allowedExtensions.Contains(extension))
            {
                var fileName = $"hero_{Guid.NewGuid()}{extension}";
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "hero");
                Directory.CreateDirectory(uploadsPath);
                var filePath = Path.Combine(uploadsPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                finalImageUrl = $"/images/hero/{fileName}";
            }
        }

        if (!string.IsNullOrEmpty(finalImageUrl))
        {
            await _heroSlideApiService.UpdateSlideAsync(token, id, finalImageUrl, displayOrder, caption, isActive);
            TempData["Success"] = "Slide updated!";
        }
        else
            TempData["Error"] = "Please upload an image file!";
        
        return RedirectToAction("HeroSlides");
    }
}
