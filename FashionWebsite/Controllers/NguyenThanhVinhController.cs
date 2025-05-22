using FashionWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Web.Helpers;
using BCrypt.Net;
using FashionWebsite.Helpers;
using System.Net.Mail;

namespace FashionWebsite.Controllers
{
    public class NguyenThanhVinhController : Controller
    {
        // GET: NguyenThanhVinh
        
        ShopQuanAoEntities db = new ShopQuanAoEntities();

        public ActionResult Index()
        {
            return View();
        }


        //-----------------------------VIEW SANPHAM---------------------------//
        public ActionResult Quan(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 1
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }
        public ActionResult Ao(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 2
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }


        public ActionResult Giay(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 3
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }

        public ActionResult Dep(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 4
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }

        public ActionResult PhuKien(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 5
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }


        //----------------------------Partial SANPHAM-----------------------------//

        public ActionResult PartialQuan(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 1
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }
        public ActionResult PartialAo(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 2
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }


        public ActionResult PartialGiay(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 3
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }

        public ActionResult PartialDep(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 4
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }

        public ActionResult PartialPhuKien(int? page)
        {
            int iSize = 8;
            int iPage = (page ?? 1);
            var kq = (from s in db.SanPhams.OrderByDescending(p => p.MaSanPham)
                      where s.MaDanhMuc == 5
                      select s).ToList();
            return View(kq.ToPagedList(iPage, iSize));
        }

        public ActionResult PartialLogin()
        {
            return PartialView();
        }

        //=====================================LOGIN==================================//
        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginViewModel();

            // Nếu có cookie, tự động điền thông tin đăng nhập
            if (Request.Cookies["TenDN"] != null && Request.Cookies["MatKhau"] != null)
            {
                model.TenDN = Request.Cookies["TenDN"].Value;
                model.MatKhau = Request.Cookies["MatKhau"].Value;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            string returnUrl = (string)Session["urlchitietphim"];

            if (string.IsNullOrEmpty(model.TenDN) || string.IsNullOrEmpty(model.MatKhau))
            {
                ViewBag.ThongBao = "Không thể để trống thông tin";
                return View(model);
            }

            // Tìm tài khoản
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.TenDN == model.TenDN);
            if (kh == null)
            {
                ViewBag.ThongBao = "Không tìm thấy tài khoản của bạn";
                return View(model);
            }

            // === BẮT ĐẦU: PHÒNG CHỐNG BRUTE FORCE ===
            string loginKey = "LoginAttempt_" + model.TenDN;
            var loginAttempt = Session[loginKey] as LoginAttempt ?? new LoginAttempt();

            if (loginAttempt.FailedCount >= 3)
            {
                string captchaResponse = Request.Form["g-recaptcha-response"];
                if (string.IsNullOrEmpty(captchaResponse))
                {
                    ViewBag.ThongBao = "Vui lòng xác nhận CAPTCHA.";
                    return View(model);
                }

                // Gửi CAPTCHA đến Google xác minh
                string secretKey = "6LcdIDorAAAAAJJIbKFonPl2-LS-GoGAREkGpxsG"; // Thay bằng khóa bí mật reCAPTCHA của bạn
                var client = new WebClient();
                var result = client.DownloadString(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}");

                dynamic captchaResult = JsonConvert.DeserializeObject(result);
                if (captchaResult.success != true)
                {
                    ViewBag.ThongBao = "Xác minh CAPTCHA thất bại. Vui lòng thử lại.";
                    return View(model);
                }
            }


            // Nếu đang bị khóa
            if (loginAttempt.LockedUntil.HasValue && loginAttempt.LockedUntil > DateTime.Now)
            {
                var minutesLeft = (loginAttempt.LockedUntil.Value - DateTime.Now).Minutes;
                ViewBag.ThongBao = $"Tài khoản đang bị khóa. Vui lòng thử lại sau {minutesLeft} phút.";
                return View(model);
            }
            // === KẾT THÚC PHẦN CHẶN BRUTE FORCE ===

            // Kiểm tra định dạng mật khẩu trong DB (đã băm chưa?)
            bool isBCryptHashed = kh.MatKhau != null && kh.MatKhau.StartsWith("$2");

            bool isPasswordCorrect = false;

            if (isBCryptHashed)
            {
                // Đã băm -> kiểm tra bằng BCrypt
                isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.MatKhau, kh.MatKhau);
            }
            else
            {
                // Mật khẩu chưa băm -> kiểm tra trực tiếp
                if (model.MatKhau == kh.MatKhau)
                {
                    isPasswordCorrect = true;

                    // 👉 Băm lại mật khẩu và lưu vào DB
                    kh.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                    db.SaveChanges();
                }
            }

            if (isPasswordCorrect)
            {
                ViewBag.ThongBao = "Bạn đã đăng nhập thành công";
                Session["user"] = kh;

                // Reset đăng nhập sai
                Session[loginKey] = null;

                // Nhớ đăng nhập bằng cookie
                if (model.RememberMe)
                {
                    Response.Cookies["TenDN"].Value = model.TenDN;
                    Response.Cookies["MatKhau"].Value = model.MatKhau;
                    Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(30);
                    Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(30);
                }
                else
                {
                    Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(-1);
                }

                // Chuyển hướng sau khi đăng nhập
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "NguyenThanhVinh");
                }
            }
            else
            {
                // === NẾU MẬT KHẨU SAI, TĂNG ĐẾM VÀ KHÓA NẾU CẦN ===
                loginAttempt.FailedCount++;

                if (loginAttempt.FailedCount >= 5)
                {
                    loginAttempt.LockedUntil = DateTime.Now.AddMinutes(5); // Khóa 5 phút
                    ViewBag.ThongBao = "Tài khoản bị khóa do nhập sai quá nhiều lần. Vui lòng thử lại sau 5 phút.";
                }
                else
                {
                    int left = 5 - loginAttempt.FailedCount;
                    ViewBag.ThongBao = $"Sai mật khẩu. Bạn còn {left} lần thử.";
                }

                Session[loginKey] = loginAttempt;
                return View(model);
            }

        }

        //============================Logput==================//
        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "NguyenThanhVinh");

        }
        //==================================Sign==================//
        [HttpGet]
        public ActionResult Sign()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sign(KhachHang kh)
        {
            if (string.IsNullOrEmpty(kh.HoTen) || string.IsNullOrEmpty(kh.HoTen) || string.IsNullOrEmpty(kh.MatKhau))
            {
                ViewBag.ThongBao = "Không được trống thông tin này";
            }
            else
            {
                KhachHang kh1 = db.KhachHangs.SingleOrDefault(n => n.TenDN == kh.TenDN);

                if (kh1 != null)
                {
                    ViewBag.ThongBao = "Tài khoản đã tồn tại";
                }
                else if (!PasswordPolicy.IsValidPassword(kh.MatKhau))
                {
                    ViewBag.Err3 = "<span style='color:red;'>Mật khẩu phải có ít nhất 12 ký tự, chứa ký tự đặc biệt, chữ hoa và số.</span>";
                }

                else
                {
                    string hashmk = BCrypt.Net.BCrypt.HashPassword(kh.MatKhau);
                    kh.MatKhau = hashmk;
                    db.KhachHangs.Add(kh);
                    db.SaveChanges();
                    ViewBag.ThongBao = "Bạn đã đăng ký thành công";

                    var mail = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential("2224802010912@student.tdmu.edu.vn", "dcjp xtja busz ehdp"),
                        EnableSsl = true
                    };

                    var message = new MailMessage();
                    message.From = new MailAddress("2224802010912@student.tdmu.edu.vn");
                    message.ReplyToList.Add("2224802010912@student.tdmu.edu.vn");
                    message.To.Add(new MailAddress(kh.Email));
                    message.Subject = "Đăng ký thành công";
                    message.Body = "Chào " + kh.HoTen + ",\r\n\r\n Chúc mừng bạn đã đăng ký thành công.";
                    mail.Send(message);

                    return RedirectToAction("Login", "NguyenThanhVinh");
                }
            }

            return View();
        }
        //====================================Change Password=======================//
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.Success = TempData["Success"];

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string TenDN, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(TenDN) ||
                string.IsNullOrWhiteSpace(OldPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = db.KhachHangs.SingleOrDefault(k => k.TenDN == TenDN);

            if (user == null)
            {
                ViewBag.ThongBao = "Tên đăng nhập không tồn tại.";
                return View();
            }

            // --- ✅ Bước 1: Kiểm tra mật khẩu đã băm chưa và xác minh ---
            bool isBCryptHashed = user.MatKhau.StartsWith("$2");

            if (!isBCryptHashed)
            {
                // Nếu chưa băm, so sánh trực tiếp
                if (user.MatKhau != OldPassword)
                {
                    ViewBag.ErrOld = "<span style='color:red;padding: 140px'>Mật khẩu cũ không đúng.</span>";
                    return View();
                }

                // Đúng thì băm lại mật khẩu và lưu vào DB (nâng cấp hệ thống)
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(OldPassword);
                db.SaveChanges();
            }
            else
            {
                // Nếu đã băm thì dùng BCrypt để kiểm tra
                if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.MatKhau))
                {
                    ViewBag.ErrOld = "<span style='color:red; padding: 140px'>Mật khẩu cũ không đúng.</span>";
                    return View();
                }
            }

            // --- ✅ Bước 2: Kiểm tra mật khẩu mới ---
            if (!PasswordPolicy.IsValidPassword(NewPassword, OldPassword))
            {
                ViewBag.ErrNew = "<span style='color:red;'>Mật khẩu mới không hợp lệ. Yêu cầu: 12+ ký tự, chữ hoa, số, ký tự đặc biệt, không trùng mật khẩu cũ.</span>";
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                ViewBag.ErrConfirm = "<span style='color:red; padding: 140px'>Mật khẩu xác nhận không khớp.</span>";
                return View();
            }

            // --- ✅ Lưu mật khẩu mới (đã băm) ---
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("ChangePassword");
        }
    }
}