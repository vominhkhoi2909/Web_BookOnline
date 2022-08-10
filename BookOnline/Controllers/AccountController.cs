using BookOnline.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookOnline.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookOnlineDatabaseContext _context;

        #region Tính Năng Khác.
        //Khởi tạo mặc định lấy dữ liệu từ db.
        public AccountController(BookOnlineDatabaseContext context)
        {
            _context = context;
        }

        //Mã hóa password.
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
        #endregion

        #region Đăng Ký
        //Điều hương trang giao diện.
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện đăng ký.
        public IActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                //Kiểm tra dữ liệu trùng.
                var checkEmail = _context.Accounts.Where(a => a.Account_Email == account.Account_Email).FirstOrDefault();
                var checkPhone = _context.Accounts.Where(a => a.Account_Phone == account.Account_Phone).FirstOrDefault();
                var checkName = _context.Accounts.Where(a => a.Account_Name == account.Account_Name).FirstOrDefault();

                if (checkName == null && checkEmail == null && checkPhone == null && account.Account_Password == account.Account_ConfirmPassword)
                {
                    try
                    {
                        //Lưu các trường dữ liệu cơ bản.
                        account.Account_Password = GetMD5(account.Account_Password);
                        account.Account_ConfirmPassword = GetMD5(account.Account_Password);
                        account.Account_Type = 0;
                        account.Account_Status = true;
                        _context.Accounts.Add(account);

                        //Cập nhật db.
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Account");
                    }
                    catch (Exception ex)
                    {
                        //Lưu msg.
                        ViewBag.Error = ex.ToString();
                        return View();
                    }
                }
                else if (checkName != null)
                {
                    //Lưu msg.
                    ViewBag.Error = "Tên tài khoản đã được sử dụng.";
                    return View();
                }
                else if (account.Account_Password != account.Account_ConfirmPassword)
                {
                    //Lưu msg.
                    ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp";
                    return View();
                }
                else if (checkEmail != null)
                {
                    //Lưu msg.
                    ViewBag.Error = "Email đã được sử dụng.";
                    return View();
                }
                else if (checkPhone != null)
                {
                    //Lưu msg.
                    ViewBag.Error = "Số điện thoại đã được sử dụng.";
                    return View();
                }
            }

            return View();
        }
        #endregion

        #region Đăng Nhập
        //Điều hương trang giao diện.
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện đăng nhập.
        public IActionResult Login(Account account)
        {
            if (ModelState.IsValid)
            {
                //Mã hóa password.
                var f_password = GetMD5(account.Account_Password);
                //Check thông tin đăng nhập có khớp vs db.
                var check = _context.Accounts.Where(a => (a.Account_Email == account.Account_Name && a.Account_Password == f_password) || (a.Account_Name == account.Account_Name && a.Account_Password == f_password)).FirstOrDefault();
               
                if (check != null)
                {
                    //Lưu lại thông tin đăng nhập.
                    ViewBag.Login = check.Account_Id;
                    ViewBag.Type = check.Account_Type;
                    TempData["Login"] = ViewBag.Login;
                    TempData["Type"] = ViewBag.Type;

                    if (check.Account_Type == 1)
                    {
                        //Chuyển tới trang quản lý nếu type thuộc 1.
                        return RedirectToAction("Index", "Categories");
                    }
                    else
                    {
                        //Chuyển tới trang quản lý nếu type khác 1.
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //Lưu msg.
                    ViewBag.Error = "Đăng nhập thất bại! Vui lòng kiểm tra lại thông tin tài khoản hoặc mật khẩu.";
                    return View();
                }
            }

            return View();
        }
        #endregion

        //Hàm thực hiện đăng xuất.
        public IActionResult Logout()
        {
            //Xóa thông tin đăng nhập đã lưu.
            ViewBag.Login = null;
            ViewBag.Type = null;
            TempData["Login"] = null;

            return RedirectToAction("Login", "Account");
        }

        #region Quản Lý Tài Khoản
        // GET: Account
        // Hiển thị trang danh sách các tài khoản.
        public IActionResult Index()
        {
            if (TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }
            //Check login và quyền của account để vào trang admin.
            if (ViewBag.Login != null && ViewBag.Type == 1)
            {
                //Hàm check msg bắt hoạt động của action thành công / lỗi.
                if (TempData["Success"] != null)
                {
                    ViewBag.Success = TempData["Success"];
                }
                if (TempData["Fail"] != null)
                {
                    ViewBag.Fail = TempData["Fail"];
                }

                //Lấy danh sách không bị xóa.
                return View(_context.Accounts.Where(a => a.Account_Status == true).ToList());
            }
            else if(ViewBag.Login != null)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }            
        }

        // GET: Account/Details/5
        // Xem chi tiết của tài khoản.
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin theo id.
            var account = _context.Accounts
                .FirstOrDefault(m => m.Account_Id == id);

            if (account == null)
            {
                return NotFound();
            }

            //Truyền dữ liệu đã lấy về view.
            return View(account);
        }

        // GET: Categories/Create
        //Điều hướng tới trang thêm mới.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện tình năng thêm mới.
        public IActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                //Kiểm tra dữ liệu trùng.
                var checkEmail = _context.Accounts.Where(a => a.Account_Email == account.Account_Email).FirstOrDefault();
                var checkPhone = _context.Accounts.Where(a => a.Account_Phone == account.Account_Phone).FirstOrDefault();
                var checkName = _context.Accounts.Where(a => a.Account_Name == account.Account_Name).FirstOrDefault();

                if (checkName == null && checkEmail == null && checkPhone == null && account.Account_Password == account.Account_ConfirmPassword)
                {
                    try
                    {
                        //Lưu các trường dữ liệu cơ bản.
                        account.Account_Password = GetMD5(account.Account_Password);
                        account.Account_ConfirmPassword = GetMD5(account.Account_Password);
                        account.Account_Type = 1;
                        account.Account_Status = true;
                        _context.Accounts.Add(account);

                        //Cập nhật db.
                        _context.SaveChanges();
                        //Lưu thống tin msg.
                        TempData["Success"] = "Thêm mới thể loại thành công.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        //Lưu msg.
                        ViewBag.CreateError = ex.ToString();
                        return View(account);
                    }
                }
                else if (checkName != null)
                {
                    //Lưu msg.
                    ViewBag.CreateError = "Tên tài khoản đã được sử dụng.";
                    return View(account);
                }
                else if (account.Account_Password != account.Account_ConfirmPassword)
                {
                    //Lưu msg.
                    ViewBag.CreateError = "Mật khẩu và xác nhận mật khẩu không khớp";
                    return View(account);
                }
                else if (checkEmail != null)
                {
                    //Lưu msg.
                    ViewBag.CreateError = "Email đã được sử dụng.";
                    return View(account);
                }
                else if (checkPhone != null)
                {
                    //Lưu msg.
                    ViewBag.CreateError = "Số điện thoại đã được sử dụng.";
                    return View(account);
                }
            }
            return View(account);
        }

        // GET: Account/Delete/5
        //Điều hướng tới trang giao diện chức năng.
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin dựa trên id.
            var account = _context.Accounts
                .FirstOrDefault(m => m.Account_Id == id);

            if (account == null)
            {
                return NotFound();
            }

            //Đưa dữ liệu lấy được vào view.
            return View(account);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện tính năng xóa tài khoản.
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Lấy thông tin dựa trên id.
                var account = _context.Accounts.Find(id);

                //Xóa tạm thời để ẩn khỏi ds.
                account.Account_Status = false;
                _context.Update(account);

                //Dùng để xóa vĩnh viễn.
                //_context.Accounts.Remove(account);

                //Cập nhật db.
                _context.SaveChanges();

                //Lưu thông tin msg.
                TempData["Success"] = "Xóa thông tin tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                //Lưu thông tin msg.
                TempData["Fail"] = "Lỗi không thể xóa tài khoản vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
