using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookOnline.Models;
using System.IO;

namespace BookOnline.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookOnlineDatabaseContext _context;

        #region Tính Năng Khác.
        //Khởi tạo mặc định lấy dữ liệu từ db.
        public BooksController(BookOnlineDatabaseContext context)
        {
            _context = context;
        }

        // GET: Books
        // Hiển thị trang danh sách các tựa sách.
        public async Task<IActionResult> Index()
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
                return View(await _context.Books.Where(b => b.Book_Status == true).ToListAsync());
            }
            else if (ViewBag.Login != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }            
        }

        // GET: Books/Details/5
        // Xem chi tiết của sách.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin theo id.
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Book_Id == id);

            //Dựa trên sách lấy thông tin tên thể loại.
            var cat = _context.Categories.Where(c => c.Category_Id == book.Book_Category).FirstOrDefault();
            ViewBag.Cat_Name = cat.Category_Name;

            if (book == null)
            {
                return NotFound();
            }

            //Truyền dữ liệu đã lấy về view.
            return View(book);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Book_Id == id);
        }
        #endregion

        #region Thêm Mới
        // GET: Books/Create
        //Điều hướng tới trang thêm mới.
        public IActionResult Create()
        {
            //Lấy thông tin thể loại để hiển thị combobox.
            ViewData["CatId"] = new SelectList(_context.Categories.Where(c => c.Category_Status == true), "Category_Id", "Category_Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện tình năng thêm mới.
        public async Task<IActionResult> Create([Bind("Book_Id,Book_Name,Book_Author,Book_Content,Book_ShortContent,Book_Category,Book_Img,Book_Date,Book_NXB,Book_View,Book_Status")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Check dữ liệu đã tồn tại chưa.
                    var check = _context.Books.Where(b => b.Book_Name == book.Book_Name && b.Book_Author == book.Book_Author && b.Book_Status == true).FirstOrDefault();

                    if (check != null)
                    {
                        //Lưu thống tin msg.
                        ViewBag.CreateError = "Tên sách của tác giả này đã tồn tại.";
                    }
                    else
                    {
                        //Gán các trường dữ liệu với thông số cơ bản.
                        book.Category = _context.Categories.Where(c => c.Category_Id == book.Book_Category).FirstOrDefault();
                        book.Book_View = 0;
                        book.Book_Date = DateTime.Now.ToShortDateString();
                        book.Book_Status = true;
                        _context.Add(book);

                        //Cập nhật db.
                        await _context.SaveChangesAsync();

                        //Lưu thông tin msg.
                        TempData["Success"] = "Thêm mới sách thành công.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    //Lưu thông tin msg.
                    ViewBag.CreateError = "Thêm mới thất bại vui lòng kiểm tra lại.";
                    return View(book);
                }
            }
            return View(book);
        }
        #endregion

        #region Cập Nhật
        // GET: Books/Edit/5
        //Điều hướng tới giao diện tính năng.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin dựa trên id.
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            //Lưu thông tin thể loại để hiển thị combobox.
            ViewData["CatId"] = new SelectList(_context.Categories.Where(c => c.Category_Status == true), "Category_Id", "Category_Name", book.Book_Category);

            //Truyền dữ liệu về view để hiển thị.
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Thực hiện tính năng cập nhật dữ liệu.
        public async Task<IActionResult> Edit(int id, [Bind("Book_Id,Book_Name,Book_Author,Book_Content,Book_ShortContent,Book_Category,Book_Img,Book_Date,Book_NXB,Book_View,Book_Status")] Book book)
        {
            if (id != book.Book_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Check dữ liệu trùng.
                var check = _context.Books.Where(b => b.Book_Name == book.Book_Name && b.Book_Author == book.Book_Author && b.Book_Status == true && b.Book_Id != book.Book_Id).FirstOrDefault();

                if (check != null)
                {
                    //Lưu msg.
                    ViewBag.EditError = "Tên sách của tác giả này đã tồn tại.";
                }
                else
                {
                    try
                    {
                        //Lưu các trường thông tin cơ bản.
                        book.Category = _context.Categories.Where(c => c.Category_Id == book.Book_Category).FirstOrDefault();
                        book.Book_Date = DateTime.Now.ToShortDateString();
                        book.Book_Status = true;
                        _context.Update(book);

                        //Cập nhật db.
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookExists(book.Book_Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //Lưu msg.
                    TempData["Success"] = "Cập nhật thông tin sách thành công.";
                    return RedirectToAction(nameof(Index));
                }                
            }
            return View(book);
        }
        #endregion

        #region Xóa
        // GET: Books/Delete/5
        //Điều hướng tới trang giao diện chức năng.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin dựa trên id.
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Book_Id == id);

            //Dựa trên sách lấy thông tin thể loại tương ứng
            var cat = _context.Categories.Where(c => c.Category_Id == book.Book_Category).FirstOrDefault();
            ViewBag.Cat_Name = cat.Category_Name;

            if (book == null)
            {
                return NotFound();
            }

            //Đưa dữ liệu lấy được về view.
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện tính năng xóa sách.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                //Lấy thông tin dựa trên id.
                var book = await _context.Books.FindAsync(id);

                //Xóa tạm thời.
                book.Book_Status = false;
                _context.Update(book);

                //Xóa vĩnh viễn.
                //_context.Books.Remove(book);

                //Cập nhật db.
                await _context.SaveChangesAsync();

                //Lưu thông tin msg.
                TempData["Success"] = "Xóa thông tin sách thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                //Lưu thông tin msg.
                TempData["Fail"] = "Xóa thất bại vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
