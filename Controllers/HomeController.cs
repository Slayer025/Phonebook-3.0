using Microsoft.AspNetCore.Mvc;
using PhonebookApp.Models;
using PhonebookApp.Repositories;

namespace PhonebookApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactRepository _repo;

        private const int PageSize = 10;

        public HomeController(IContactRepository repo)
        {
            _repo = repo;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index(int? page, string searchTerm)
        {
            int pageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;

            var result = await _repo.GetContactsPagedAsync(pageNumber, PageSize, searchTerm);

            ViewBag.SearchTerm = searchTerm;

            return View(result);
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                await _repo.InsertContactAsync(contact);
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var contact = await _repo.GetContactByIdAsync(id.Value);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Home/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                await _repo.UpdateContactAsync(contact);
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // POST: Home/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new { success = false, message = "Invalid contact id." });
            }

            try
            {
                bool success = await _repo.DeleteContactAsync(id.Value);

                if (success)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Contact not found or could not be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
