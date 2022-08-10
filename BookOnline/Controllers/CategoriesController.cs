using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookOnline.Models;

namespace BookOnline.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly BookOnlineDatabaseContext _context;

        #region Tính Năng Khác
        //Khởi tạo mặc định lấy dữ liệu từ db.
        public CategoriesController(BookOnlineDatabaseContext context)
        {
            _context = context;
        }

        // GET: Categories
        // Hiển thị trang danh sách các thể loại.
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
                return View(await _context.Categories.Where(c => c.Category_Status == true).ToListAsync());
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

        // GET: Categories/Details/5
        // Xem chi tiết của thể loại.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin theo id.
            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Category_Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //Truyền dữ liệu đã lấy về view.
            return View(category);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Category_Id == id);
        }
        #endregion

        #region Thêm Mới
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
        public async Task<IActionResult> Create([Bind("Category_Id,Category_Name,Category_DataName,Category_Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Check dữ liệu đã tồn tại chưa.
                    var check = _context.Categories.Where(c => (c.Category_Name == category.Category_Name || c.Category_DataName == category.Category_DataName) && c.Category_Status == true).FirstOrDefault();

                    if (check != null)
                    {
                        //Lưu thống tin msg.
                        ViewBag.CreateError = "Tên thể loại hoặc tên hệ thống đã tồn tại.";
                    }
                    else
                    {
                        //Gán các trường dữ liệu với thông số cơ bản.
                        category.Category_Status = true;
                        _context.Add(category);

                        //Cập nhật db.
                        await _context.SaveChangesAsync();

                        //Lưu thống tin msg.
                        TempData["Success"] = "Thêm mới thể loại thành công.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    //Lưu thống tin msg.
                    ViewBag.CreateError = "Thêm mới thất bại vui lòng kiểm tra lại.";
                    return View(category);
                }
            }
            return View(category);
        }
        #endregion

        #region Cập Nhật
        // GET: Categories/Edit/5
        //Điều hướng tới giao diện tính năng.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin dựa trên id.
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            //Truyền dữ liệu về view để hiển thị.
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Thực hiện tính năng cập nhật dữ liệu.
        public async Task<IActionResult> Edit(int id, [Bind("Category_Id,Category_Name,Category_DataName,Category_Status")] Category category)
        {
            if (id != category.Category_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Check dữ liệu trùng.
                var check = _context.Categories.Where(c => (c.Category_Name == category.Category_Name || c.Category_DataName == category.Category_DataName) && c.Category_Status == true && c.Category_Id != category.Category_Id).FirstOrDefault();

                if (check != null)
                {
                    //Lưu msg.
                    ViewBag.EditError = "Tên thể loại hoặc tên hệ thống đã tồn tại.";
                }
                else
                {
                    try
                    {
                        //Lưu các trường thông tin cơ bản.
                        category.Category_Status = true;
                        _context.Update(category);

                        //Cập nhật db.
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.Category_Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    //Lưu msg.
                    TempData["Success"] = "Thêm mới thể loại thành công.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }
        #endregion

        #region Xóa
        // GET: Categories/Delete/5
        //Điều hướng tới trang giao diện chức năng.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy thông tin dựa trên id.
            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Category_Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //Đưa dữ liệu lấy được vào view.
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //Hàm thực hiện tính năng xóa thể loại.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                //Lấy thông tin dựa trên id.
                var category = await _context.Categories.FindAsync(id);

                //Xóa tạm thời để ẩn khỏi ds.
                category.Category_Status = false;
                _context.Update(category);

                //Dùng để xóa vĩnh viễn.
                //_context.Categories.Remove(category);

                //Cập nhật db.
                await _context.SaveChangesAsync();

                //Lưu thông tin msg.
                TempData["Success"] = "Xóa thông tin thể loại thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                //Lưu thông tin msg.
                TempData["Fail"] = "Lỗi không thể xóa thể loại vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
