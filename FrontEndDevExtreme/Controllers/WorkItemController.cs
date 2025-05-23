﻿using DevExpress.XtraEditors.Filtering;
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

        
        public async Task<IActionResult> Index([FromQuery] WorkItemFilterModel filter, int pageNumber = 1, int pageSize = 100)
        {
            var response = await _workItemApiService.GetFilteredAsync(filter, pageNumber, pageSize);

            if (!response.Success)
            {
                // Xử lý lỗi, ví dụ:
                ModelState.AddModelError("", response.Message);
                return View(new List<WorkItemViewModel>());
            }

            var workItems = response.Data ?? new List<WorkItemViewModel>();

            var assigned = await _userApiService.GetAllAsync();
            var assigner = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            ViewBag.Assigned = assigned;
            ViewBag.Assigner = assigner;
            ViewBag.Departments = departments;

            foreach (var workItem in workItems)
            {
                workItem.Notes = await _noteApiService.GetNotesByWorkItemIdAsync(workItem.WorkItemID);
                workItem.NoteCount = workItem.Notes.Count();
            }

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)response.TotalCount / pageSize);
            return View(workItems);
        }

        public async Task<PartialViewResult> GetFilteredGrid([FromQuery] WorkItemFilterModel filter, int pageNumber = 1, int pageSize = 100)
        {
            var response = await _workItemApiService.GetFilteredAsync(filter, pageNumber, pageSize);
            var workItems = response.Data ?? new List<WorkItemViewModel>();

            foreach (var workItem in workItems)
            {
                workItem.Notes = await _noteApiService.GetNotesByWorkItemIdAsync(workItem.WorkItemID);
                workItem.NoteCount = workItem.Notes.Count();
            }

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)response.TotalCount / pageSize);

            return PartialView("_WorkItemGridPartial", workItems);
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
        public async Task<IActionResult> Create([FromForm] WorkItemCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
            }

            var result = await _workItemApiService.CreateAsync(model);
            if (result)
            {
                return Json(new { success = true, message = "Tạo công việc thành công" });
            }

            return Json(new { success = false, message = "Tạo công việc thất bại. Vui lòng thử lại." });
        }


        public async Task<IActionResult> CreatePopUp()
        {
            var users = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;

            
            return PartialView("_CreateWorkItemPartial", new WorkItemCreateModel());
        }


        [HttpPost]
        public async Task<IActionResult> CreatePopUp([FromForm] WorkItemCreateModel model)
        {
           
            var departmentIds = Request.Form["DepartmentIDs"].ToList();
            var userIds = Request.Form["UserIDs"].ToList();

           
            model.DepartmentIDs = departmentIds.Select(int.Parse).ToList();
            model.UserIDs = userIds.Select(int.Parse).ToList();

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
            }

            
            var result = await _workItemApiService.CreateAsync(model);
            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Tạo công việc thất bại. Vui lòng thử lại." });
        }







        public async Task<IActionResult> Detail(int id)
        {
            var workItem = await _workItemApiService.GetWorkItemDetailAsync(id);
            if (workItem == null) return NotFound();

            workItem.Notes = await _noteApiService.GetNotesByWorkItemIdAsync(id);
            return View(workItem);
        }

        public async Task<IActionResult> NotePartial(int workItemId)
        {
            var noteModels = await _noteApiService.GetNotesByWorkItemIdAsync(workItemId);

            
            var noteViewModels = noteModels.Select(n => new NoteViewModel
            {
                NoteID = n.NoteID,
                Content = n.Content,
                CreatedDate = n.CreatedDate,
               
            }).ToList();

            return PartialView("_NotePartial", noteViewModels);
        }




        [HttpPost]
        public async Task<IActionResult> AddNote(int workItemId, string content)
        {
            
            if (string.IsNullOrEmpty(content))
            {
                return Json(new { success = false, message = "Nội dung ghi chú không thể để trống." });
            }

            try
            {
                
                var result = await _noteApiService.AddNoteAsync(workItemId, content);

                if (result)
                {
                    
                    var newNote = await _noteApiService.GetNotesByWorkItemIdAsync(workItemId);
                    var lastNote = newNote.LastOrDefault();

                    if (lastNote != null)
                    {
                        return Json(new
                        {
                            success = true,
                            data = new
                            {
                                lastNote.NoteID,
                                lastNote.Content,
                                CreatedDate = lastNote.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                            }
                        });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không thể lấy ghi chú vừa thêm." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi khi thêm ghi chú." });
                }
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = "Đã có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeleteNote(int noteId, int workItemId)
        {
            var result = await _noteApiService.DeleteNoteAsync(noteId, workItemId);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Có lỗi khi xóa ghi chú." });
            }
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
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _workItemApiService.DeleteAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Xóa thành công" });
            }

            TempData["Error"] = "Xóa thất bại";
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var workItem = await _workItemApiService.GetWorkItemDetailAsync(id);
            if (workItem == null)
                return Json(new { success = false, message = "Không tìm thấy công việc." });

            var users = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            var model = new WorkItemEditModel
            {
                WorkItemID = workItem.WorkItemID,
                TaskName = workItem.TaskName,
                Status = 2,
                Progress = 100,
                TaskType = workItem.TaskType,
                IsPinned = workItem.IsPinned,
                StartDate = workItem.StartDate,
                EndDate = workItem.EndDate,
                AssignerID = workItem.AssignerID,
                Priority = workItem.Priority,
                DepartmentIDs = workItem.DepartmentList?
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Select(name => departments.FirstOrDefault(d => d.DepartmentName == name)?.DepartmentID ?? 0)
                    .Where(d => d != 0).ToList() ?? new List<int>(),
                UserIDs = workItem.UserList?
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Select(name => users.FirstOrDefault(u => u.UserName == name)?.UserID ?? 0)
                    .Where(u => u != 0).ToList() ?? new List<int>()
            };

            var result = await _workItemApiService.UpdateAsync(id, model);
            if (result)
            {
                return Json(new { success = true, message = "Đã hoàn tất công việc." });
            }

            return Json(new { success = false, message = "Cập nhật thất bại." });
        }

    }
}
