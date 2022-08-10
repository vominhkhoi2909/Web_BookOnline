using BookOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookOnlineDatabaseContext _context;

        public HomeController(BookOnlineDatabaseContext context)
        {
            _context = context;
        }

        //Hiển thị trang chủ cho người dùng.
        public async Task<IActionResult> Index()
        {
            if(TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }

            ViewBag.Hot = _context.Books.Where(b => b.Book_Status == true).OrderByDescending(b => b.Book_View).Take(10);
            ViewBag.New = _context.Books.Where(b => b.Book_Status == true).OrderBy(b => b.Book_Id).Reverse().Take(10);
            ViewBag.Categories = _context.Categories.Where(c => c.Category_Status == true).Take(3);

            return View(await _context.Books.Where(c => c.Book_Status == true).ToListAsync());
        }

        //Hiển thị trang danh mục cho người dùng.
        public IActionResult Categories(int? id)
        {
            if (TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }

            ViewBag.Categories = _context.Categories.Where(c => c.Category_Status == true);
            
            if (id != null)
            {
                ViewBag.Selection = _context.Categories.Where(c => c.Category_Status == true && c.Category_Id == id).FirstOrDefault();
                return View(_context.Books.Where(b => b.Book_Status == true && b.Book_Category == id).ToList());
            }
            else
            {
                ViewBag.Selection = null;
                return View(_context.Books.Where(b => b.Book_Status == true).ToList());
            }            
        }

        //Hiển thị trang danh mục với dữ liệu search.
        public IActionResult Search(int? id, FormCollection frm)
        {
            var key = frm["Search"];
            if (TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }

            ViewBag.Categories = _context.Categories.Where(c => c.Category_Status == true);

            if (id != null)
            {
                ViewBag.Selection = _context.Categories.Where(c => c.Category_Status == true && c.Category_Id == id).FirstOrDefault();
                return View(_context.Books.Where(b => b.Book_Status == true && b.Book_Category == id && (b.Book_Name.Contains(key) || b.Book_NXB.Contains(key) || b.Book_Author.Contains(key))).ToList());
            }
            else
            {
                ViewBag.Selection = null;
                return View(_context.Books.Where(b => b.Book_Status == true && (b.Book_Name.Contains(key) || b.Book_NXB.Contains(key) || b.Book_Author.Contains(key))).ToList());
            }
        }

        //Hiển thị trang chi tiết sách.
        public IActionResult Single(int? id)
        {
            if (TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }

            var book = _context.Books.Where(b => b.Book_Id == id).FirstOrDefault();
            book.Book_View += 1;
            _context.Update(book);
            _context.SaveChanges();
            ViewBag.Cat = _context.Categories.Where(c => c.Category_Id == book.Book_Category);

            return View(book);
        }

        //Hàm tải file về máy.
        public FileResult Download(string fileName="*.pdf")
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\KhoiVM1\Downloads\");
            
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        //Hàm xem file trực tuyến.
        public IActionResult ViewDocument(int? id)
        {
            if (TempData["Login"] != null)
            {
                ViewBag.Login = TempData["Login"];
                ViewBag.Type = TempData["Type"];
                TempData["Type"] = ViewBag.Type;
                TempData["Login"] = ViewBag.Login;
            }
            if(ViewBag.Login == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var book = _context.Books.Where(b => b.Book_Id == id).FirstOrDefault();
                string path = "wwwroot/pdf/" + book.Book_Content;
                return File(System.IO.File.ReadAllBytes(path), "application/pdf");
            }
        }
    }
}
