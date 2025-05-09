using FrontEndDevExtreme.Models;
using FrontEndDevExtreme.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEndDevExtreme.Controllers
{
    public class WorkItemController : Controller
    {
        private readonly IWorkItemApiService _workItemApiService;
        private readonly IDepartmentApiService _departmentApiService;
        private readonly IUserApiService _userApiService;
        private readonly INoteApiService _noteApiService;

        public WorkItemController(IWorkItemApiService workItemApiService, IDepartmentApiService departmentApiService, IUserApiService userApiService, INoteApiService noteApiService)
        {
            _workItemApiService = workItemApiService;
            _departmentApiService = departmentApiService;
            _userApiService = userApiService;
            _noteApiService = noteApiService;
        }


        public async Task<IActionResult> Index([FromQuery] WorkItemFilterModel filter)
        {
            var workItems = await _workItemApiService.GetFilteredAsync(filter);
            var assigned = await _userApiService.GetAllAsync();
            var assigner = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync(); 

            ViewBag.Assigned = assigned;
            ViewBag.Assigner = assigner;
            ViewBag.Departments = departments;
            return View(workItems);
        }



        public async Task<IActionResult> Create()
        {
            var users = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(WorkItemCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _workItemApiService.CreateAsync(model);
            if (result)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Không thể tạo công việc mới.");
            return View(model);
        }


        public async Task<IActionResult> Detail(int id)
        {
            var workItem = await _workItemApiService.GetWorkItemDetailAsync(id);
            if (workItem == null) return NotFound();

            workItem.Notes = await _noteApiService.GetNotesByWorkItemIdAsync(id);
            return View(workItem);
        }
       
        public async Task<IActionResult> Edit(int id)
        {
            var workItemDetail = await _workItemApiService.GetWorkItemDetailAsync(id);
            var users = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;

            var model = new WorkItemEditModel
            {
                WorkItemID = workItemDetail.WorkItemID,
                TaskName = workItemDetail.TaskName,
                Status = workItemDetail.Status,
                Progress = workItemDetail.Progress,
                TaskType = workItemDetail.TaskType,
                IsPinned = workItemDetail.IsPinned,
                StartDate = workItemDetail.StartDate,
                EndDate = workItemDetail.EndDate,
                AssignerID = workItemDetail.AssignerID,
                Priority = workItemDetail.Priority,
                DepartmentIDs = workItemDetail.DepartmentList?
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Select(name => departments.FirstOrDefault(d => d.DepartmentName == name)?.DepartmentID ?? 0)
                    .Where(id => id != 0)
                    .ToList() ?? new List<int>(),
                UserIDs = workItemDetail.UserList?
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Select(name => users.FirstOrDefault(u => u.UserName == name)?.UserID ?? 0)
                    .Where(id => id != 0)
                    .ToList() ?? new List<int>()
            };

            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> Edit(WorkItemEditModel model)
        {
            if (!ModelState.IsValid)
            {
                
                ViewBag.Users = await _userApiService.GetAllAsync();
                ViewBag.Departments = await _departmentApiService.GetAllAsync();
                return View(model);
            }

            var result = await _workItemApiService.UpdateAsync(model.WorkItemID, model);
            if (result)
            {
                return RedirectToAction("Index");
            }

            
            ViewBag.Users = await _userApiService.GetAllAsync();
            ViewBag.Departments = await _departmentApiService.GetAllAsync();
            ModelState.AddModelError(string.Empty, "Cập nhật thất bại");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddNote(int workItemId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("", "Nội dung ghi chú không thể để trống.");
                return RedirectToAction("Details", new { id = workItemId });
            }

            
            var result = await _noteApiService.AddNoteAsync(workItemId, content);

            if (result)
            {
                return RedirectToAction("Detail", new { id = workItemId });
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi khi thêm ghi chú.");
                return RedirectToAction("Detail", new { id = workItemId });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeleteNote(int workItemId, int noteId)
        {
           
            await _noteApiService.DeleteNoteAsync(workItemId, noteId);
            return RedirectToAction("Detail", new { id = workItemId });
        }

    }
}
