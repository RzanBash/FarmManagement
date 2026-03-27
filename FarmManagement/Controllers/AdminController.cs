using Microsoft.AspNetCore.Mvc;
using FarmManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
namespace FarmManagement.Controllers
{


    public class AdminController : Controller
    {
        private readonly FarmBookingDBContext _context;

        // بناء الكلاس مع تهيئة الـ DBContext
        public AdminController(FarmBookingDBContext context)
        {
            _context = context;
        }


        #region إدارة تسجيل الدخول

        // عرض صفحة تسجيل الدخول
        public IActionResult Login()
        {
            return View();  // يعرض صفحة تسجيل الدخول
        }

        // عند إرسال النموذج، سيتم التحقق من صحة بيانات المستخدم
        [HttpPost] 
        public IActionResult Login(string username, string password)
        {
            // محاولة العثور على المستخدم في قاعدة البيانات مع كلمة المرور
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            // إذا لم يتم العثور على المستخدم أو كانت كلمة المرور غير صحيحة
            if (user == null)
            {
                // إضافة رسالة الخطأ إلى ViewData لعرضها في الـ View
                ViewData["ErrorMessage"] = "اسم المستخدم أو كلمة المرور غير صحيحة";
                return View();  // إعادة عرض صفحة تسجيل الدخول مع رسالة خطأ
            }

            // إذا تم التحقق بنجاح، سيتم توجيه المستخدم إلى الصفحة المطلوبة
            return RedirectToAction("Index", "Admin");  // توجيه إلى صفحة Index في AdminController
        }

        #endregion



        #region إدارة المنتجات

        // عرض جميع المنتجات في صفحة واحدة
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();  // جلب جميع المنتجات
            var cabins = await _context.Cabins.ToListAsync();      // جلب جميع الأكواخ
            var bookings = await _context.Bookings.ToListAsync(); // جلب جميع الحجوزات 

            var viewModel = new AdminViewModel
            {
                Products = products,  // تمرير المنتجات إلى الـ ViewModel
                Cabins = cabins,      // تمرير الأكواخ إلى الـ ViewModel
                Bookings = bookings   // تمرير الحجوزات إلى الـ ViewModel
            };

            return View(viewModel);  // عرض الـ View
        }

     

        // عند إرسال النموذج، سيتم حفظ المنتج
        // عرض صفحة إضافة منتج جديد
        public IActionResult CreateProduct()
        {
            return View();  // عرض صفحة إضافة المنتج
        }

        // عند إرسال النموذج، سيتم حفظ المنتج
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)  // التحقق من صحة البيانات المدخلة
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // تأكد من أن المجلد موجود
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/" + uniqueFileName;  // حفظ مسار الصورة
                }

                _context.Products.Add(product);  // إضافة المنتج إلى قاعدة البيانات
                await _context.SaveChangesAsync();  // حفظ التغييرات
                return RedirectToAction("Index");  // إعادة توجيه إلى صفحة قائمة المنتجات
            }
            return View(product);  // إذا كانت هناك أخطاء، عرض الصفحة مرة أخرى
        }

        public IActionResult DetailsProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
 
        // عرض صفحة تعديل المنتج
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);  // جلب المنتج بناءً على الـ id

            if (product == null)
            {
                return NotFound();  // إذا لم يتم العثور على المنتج
            }

            return View(product);  // عرض الصفحة مع البيانات
        }

        // عند إرسال النموذج، سيتم حفظ التعديلات


        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)  // التحقق من صحة البيانات المدخلة
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);

                if (existingProduct == null)
                {
                    return NotFound();  // إذا لم يتم العثور على المنتج
                }

                // تحديث البيانات هنا
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;

                // تحديث صورة المنتج إذا كانت هناك صورة جديدة
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // تأكد من أن المجلد موجود
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }

                    existingProduct.ImageUrl = "/images/" + uniqueFileName;  // حفظ مسار الصورة الجديدة
                }

                await _context.SaveChangesAsync();  // حفظ التغييرات في قاعدة البيانات

                return RedirectToAction("Index");  // إعادة توجيه إلى صفحة عرض المنتجات
            }

            return View(product);  // إذا كانت هناك أخطاء، عرض الصفحة مرة أخرى
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Deletion successful!";  // Assign success message in TempData

            return RedirectToAction("Index");
        }


        #endregion

        #region إدارة الأكواخ

        // إضافة كوخ جديد
        public IActionResult CreateCabin()
        {
            return View();  // عرض صفحة إضافة كوخ جديد
        }

        [HttpPost]
        public async Task<IActionResult> CreateCabin(Cabin cabin, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)  // التحقق من صحة البيانات المدخلة
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // تأكد من أن المجلد موجود، وإذا لم يكن، أنشئه
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }

                    cabin.ImageUrl = "/images/" + uniqueFileName;  // حفظ مسار الصورة
                }

                _context.Cabins.Add(cabin);  // إضافة الكوخ إلى قاعدة البيانات
                await _context.SaveChangesAsync();  // حفظ التغييرات
                return RedirectToAction("Index");  // إعادة توجيه إلى صفحة عرض الأكواخ
            }
            return View(cabin);  // إذا كانت هناك أخطاء، عرض الصفحة مرة أخرى
        }


        public IActionResult DetailsCabin(int id)
        {
            var cabin = _context.Cabins.FirstOrDefault(c => c.Id == id);
            if (cabin == null)
            {
                return NotFound();
            }
            return View(cabin);
        }

        // عرض صفحة تعديل كوخ
        public IActionResult EditCabin(int id)
        {
            var cabin = _context.Cabins.FirstOrDefault(c => c.Id == id);  // جلب الكوخ بناءً على الـ id
            if (cabin == null)
            {
                return NotFound();  // إذا لم يتم العثور على الكوخ
            }
            return View(cabin);  // عرض الصفحة مع البيانات
        }

        // عند إرسال النموذج، سيتم حفظ التعديلات
        [HttpPost]
        public async Task<IActionResult> EditCabin(Cabin cabin, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)  // التحقق من صحة البيانات المدخلة
            {
                var existingCabin = await _context.Cabins.FindAsync(cabin.Id);

                if (existingCabin == null)
                {
                    return NotFound();  // إذا لم يتم العثور على الكوخ
                }

                // تحديث البيانات هنا
                existingCabin.Name = cabin.Name;
                existingCabin.Description = cabin.Description;
                existingCabin.PricePerNight = cabin.PricePerNight;
                existingCabin.Capacity = cabin.Capacity;
                existingCabin.IsAvailable = cabin.IsAvailable;

                // تحديث الصورة إذا كانت هناك صورة جديدة
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // تأكد من أن المجلد موجود
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }

                    existingCabin.ImageUrl = "/images/" + uniqueFileName;  // حفظ مسار الصورة الجديدة
                }

                await _context.SaveChangesAsync();  // حفظ التغييرات

                return RedirectToAction("Index");  // إعادة توجيه إلى صفحة عرض الأكواخ
            }

            return View(cabin);  // في حالة وجود أخطاء في النموذج، إعادة عرض الصفحة
        }
        // حذف الكوخ
        [HttpPost]
        public async Task<IActionResult> DeleteCabin(int id)
        {
            var cabin = await _context.Cabins.FindAsync(id);
            if (cabin == null)
            {
                return NotFound(); // إذا لم يتم العثور على الكوخ
            }

            _context.Cabins.Remove(cabin);
            await _context.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات
            TempData["SuccessMessage"] = "Deletion successful!";  // Assign success message in TempData

            return RedirectToAction("Index"); // العودة إلى صفحة الأكواخ
        }
        #endregion
        // عرض صفحة إضافة حجز
        public IActionResult CreateBooking()
        {
            ViewBag.Cabins = _context.Cabins.ToList();  // تمرير الأكواخ إلى الـ View
            return View();
        }
        // معالجة إنشاء الحجز
        [HttpPost]
        public async Task<IActionResult> CreateBooking(Booking booking)
        {
            // إضافة الحجز إلى قاعدة البيانات مباشرة
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();  // حفظ التغييرات في قاعدة البيانات

            // إعادة التوجيه إلى صفحة عرض الحجوزات بعد حفظ الحجز
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DetailsBooking(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Cabin)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }


        // عرض صفحة تعديل الحجز
        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);  // جلب الحجز بناءً على الـ ID
            if (booking == null)
            {
                return NotFound();  // إذا لم يتم العثور على الحجز
            }

            ViewBag.Cabins = _context.Cabins.ToList();  // تمرير الأكواخ إلى الـ View
            return View(booking);  // عرض الحجز في صفحة التعديل
        }

        // معالجة تعديل الحجز
        [HttpPost]
        public async Task<IActionResult> EditBooking(int id, Booking booking)
        {
            if (id != booking.Id)  // التحقق من أن الـ ID الذي تم تمريره يطابق الـ ID في الـ Booking
            {
                return NotFound();  // في حالة عدم التطابق
            }

            // تعديل الحجز مباشرة في قاعدة البيانات
            _context.Bookings.Update(booking);  // تحديث الحجز في قاعدة البيانات
            await _context.SaveChangesAsync();  // حفظ التغييرات في قاعدة البيانات

            // إعادة التوجيه إلى صفحة عرض الحجوزات بعد التعديل
            return RedirectToAction(nameof(Index));
        }


        // حذف الحجز
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(); // إذا لم يتم العثور على الحجز
            }

            _context.Bookings.Remove(booking);  // حذف الحجز من قاعدة البيانات
            await _context.SaveChangesAsync();  // حفظ التغييرات في قاعدة البيانات
            TempData["SuccessMessage"] = "Deletion successful!";  // Assign success message in TempData

            return RedirectToAction("Index");  // إعادة التوجيه إلى صفحة الحجوزات
        }
    }
}