using HelpdeskModel.BusinessRules;
using HelpdeskModel.ViewModels;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]

    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _deptService;

        public DepartmentController(IDepartmentService deptService)
        {
            _deptService = deptService;
        }

        // GET: /Department
        public async Task<IActionResult> Index()
        {
            var list = await _deptService.GetAllAsync();
            return View(list);
        }

        // GET: /Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _deptService.CreateAsync(model, 1); // createdById dummy
            if (success) return RedirectToAction("Index", "Department");

            ModelState.AddModelError("", "Failed to create department");
            return View(model);
        }

        // GET: /Department/Edit/
        public async Task<IActionResult> Edit(long id)
        {
            var dept = await _deptService.GetByIdAsync(id);
            if (dept == null) return NotFound();

            var vm = new DepartmentViewModel { Id = dept.Id, Name = dept.Name };
            return View(vm);
        }

        // POST: /Department/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _deptService.UpdateAsync(model, 1);
            if (success) return RedirectToAction("Index", "Department");

            ModelState.AddModelError("", "Failed to update department");
            return View(model);
        }

        // GET: /Department/Delete/
        public async Task<IActionResult> Delete(long id)
        {
            var dept = await _deptService.GetByIdAsync(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: /Department/DeleteConfirmed/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var success = await _deptService.DeleteAsync(id, 1);
            if (success) return RedirectToAction("Index", "Department");

            ModelState.AddModelError("", "Failed to delete department");
            return RedirectToAction(nameof(Index));
        }
    }
}
