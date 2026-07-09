
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.Security.Claims;
using TasksManager.Data;
using TasksManager.Models;

public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    // Helper functions
    public static string GetPriorityClass(TaskModel.TaskPriority priority)
    {
        return priority switch
        {
            TaskModel.TaskPriority.High => "text-danger",
            TaskModel.TaskPriority.Medium => "text-warning",
            TaskModel.TaskPriority.Low => "text-success",
            _ => ""
        };
    }

    private bool TaskExists(int? id)
    {
        return _context.TasksDB.Any(e => e.Id == id);
    }

    // GET: TASKS
    public async Task<IActionResult> Index(int? filter, string sortOrder)
    {
        ViewData["SortOrder"] = sortOrder;
        var currentUser = await _userManager.GetUserAsync(User);
        var tasks = from t in _context.TasksDB select t;
        if (User.IsInRole("Leader"))
        {
            tasks = tasks.Where(t => t.Owner == currentUser)
                                            .Include(t => t.AssignedUsers);
        }
        else
        {
            tasks = tasks.Where(t => t.AssignedUsers.Contains(currentUser!))
                                            .Include(t => t.Owner);
        }
        if (filter.HasValue)
        {
            tasks = tasks.Where(t => t.Status == (TaskModel.TaskStatus)filter);
            ViewData["Filter"] = filter;
        }
        switch (sortOrder)
        {
            case "name_desc":
                tasks = tasks.OrderByDescending(t => t.Name);
                break;
            case "status_desc":
                tasks = tasks.OrderByDescending(t => t.Status);
                break;
            case "priority_desc":
                tasks = tasks.OrderByDescending(t => t.Priority);
                break;
            case "duetodate_desc":
                tasks = tasks.OrderByDescending(t => t.DueToDate);
                break;

            case "name":
                tasks = tasks.OrderBy(t => t.Name);
                break;
            case "status":
                tasks = tasks.OrderBy(t => t.Status);
                break;
            case "priority":
                tasks = tasks.OrderBy(t => t.Priority);
                break;
            case "duetodate":
                tasks = tasks.OrderBy(t => t.DueToDate);
                break;
            case "assignmentdate":
                tasks = tasks.OrderBy(t => t.CreationDate);
                break;

            default:
                tasks = tasks.OrderByDescending(t => t.CreationDate);
                break;
        }

        var taskViews = tasks.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Name = t.Name,
            Status = t.Status,
            Description = t.Description,
            Priority = t.Priority,
            DueToDate = t.DueToDate,
            CreationDate = (t.CreationDate == null) ? null : DateOnly.FromDateTime((DateTime)t.CreationDate),
            AssignedUserNames = t.AssignedUsers.Select(u => u.Name).ToList(),
            OwnerName = (t.Owner == null) ? "" : t.Owner.Name
        });

        return View(await taskViews.AsNoTracking().ToListAsync());
    }

    // GET: TASKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _context.TasksDB.Include(t => t.AssignedUsers)
            .Include(t => t.Owner)
            .Include(t => t.Comments)
            .Include(t => t.Comments)
                .ThenInclude(c => c.File)
            .Include(t => t.File)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    public async Task<IActionResult> Download(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var file = await _context.FilesDB.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
            {
                return NotFound();
            }
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _environment.WebRootPath, file.FilePath);
            return PhysicalFile(fullPath, "application/octet-stream", file.OriginalFileName);
        }
        catch
        {
            return BadRequest();
        }
    }

    // POST: TASKS/Cancel/5
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFile(int? id, TaskViewModel model)
    {
        if (id == null)
        {
            return NotFound();
        }

        var file = await _context.FilesDB.FirstOrDefaultAsync(f => f.Id == id);
        if (file != null)
        {
            await _context.TasksDB.Where(t => t.FileId == file.Id).ExecuteUpdateAsync(s => s.SetProperty(t => t.FileId, (int?)null));
            await _context.SaveChangesAsync();
            _context.FilesDB.Remove(file);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Edit), model);
    }

    // GET: TASKS/Create
    public async Task<IActionResult> Create()
    {
        TaskViewModel taskVM = new TaskViewModel() { Name = "", Description = "" };
        taskVM.Users = await _context.UsersDB.AsNoTracking()
            .ToListAsync();
        return View(taskVM);
    }

    // POST: TASKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskViewModel taskVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                TaskModel task = new TaskModel()
                {
                    Name = taskVM.Name,
                    DueToDate = taskVM.DueToDate,
                    Description = taskVM.Description,
                    Status = TaskModel.TaskStatus.InProgress,
                    Priority = taskVM.Priority,
                    OwnerId = currentUser!.Id,
                    CreationDate = DateTime.Now,
                    AssignedUsers = await _context.UsersDB
                                .Where(u => taskVM.AssignedUserIds.Contains(u.Id))
                                .ToListAsync()
                };

                _context.Add(task);
                await _context.SaveChangesAsync();

                if (taskVM.File != null)
                {
                    // Add file validation + malware check

                    IFormFile file = taskVM.File;
                    var directory = FileModel.UploadsPath + task.Id + "/";
                    string filePath = directory + Path.GetRandomFileName();

                    FileModel fm = new FileModel()
                    {
                        OriginalFileName = Path.GetFileName(file.FileName),
                        FilePath = Path.GetRelativePath(_environment.WebRootPath, filePath),
                        TaskId = task.Id
                    };
                    _context.FilesDB.Add(fm);

                    Directory.CreateDirectory(directory);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    await _context.SaveChangesAsync();
                    task.FileId = fm.Id;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        return View(taskVM);
    }

    // GET: TASKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _context.TasksDB.Include(t => t.AssignedUsers)
                                         .Include(t => t.File)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(item => item.Id == id);
        if (task == null)
        {
            return NotFound();
        }
        var editViewModel = new TaskViewModel()
        {
            Users = await _context.UsersDB.ToListAsync(),
            Name = task.Name,
            DueToDate = task.DueToDate,
            Priority = task.Priority,
            Description = task.Description,
        };
        foreach (var user in task.AssignedUsers)
        {
            editViewModel.AssignedUserIds.Add(user.Id);
        }

        if (task.FileId != null)
        {
            editViewModel.FileId = task.FileId;
            editViewModel.OriginalFileName = task.File!.OriginalFileName;
        }

        return View(editViewModel);
    }

    // POST: TASKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, TaskViewModel editViewModel)
    {
        var task = await _context.TasksDB.Include(t => t.AssignedUsers).FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                task.Name = editViewModel.Name;
                task.DueToDate = editViewModel.DueToDate;
                task.Description = editViewModel.Description;
                task.Priority = editViewModel.Priority;
                task.AssignedUsers = await _context.UsersDB
                        .Where(u => editViewModel.AssignedUserIds.Contains(u.Id))
                        .ToListAsync();
                if (editViewModel.File != null)
                {
                    var directory = FileModel.UploadsPath + task.Id + "/";
                    string filePath = directory + Path.GetRandomFileName();

                    Directory.CreateDirectory(directory);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await editViewModel.File.CopyToAsync(stream);
                    }

                    FileModel fm = new FileModel()
                    {
                        OriginalFileName = Path.GetFileName(editViewModel.File.FileName),
                        FilePath = Path.GetRelativePath(_environment.WebRootPath, filePath),
                        TaskId = task.Id,
                    };
                    _context.FilesDB.Add(fm);
                    await _context.SaveChangesAsync();
                    task.FileId = fm.Id;
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(editViewModel);
    }

    // GET: TASKS/Cancel/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _context.TasksDB
            .Include(t => t.AssignedUsers)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // POST: TASKS/Cancel/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var task = await _context.TasksDB.FindAsync(id);
        if (task != null)
        {
            task.Status = TaskModel.TaskStatus.Deleted;
            task.DeletedDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: TASKS/Submit/5
    public async Task<IActionResult> Submit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        try
        {
            var messages = await _context.CommentsDB.Where(c => c.TaskModelId == id)
                .Include(c => c.File)
                .AsNoTracking()
                .ToListAsync();
            return View(new SubmitModel() { Comments = messages, message = "" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            throw;
        }
    }

    // POST: TASKS/Submit/5
    [HttpPost, ActionName("Submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmSubmit(int id, SubmitModel model, string action)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var task = _context.TasksDB.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Member") && action == "Submit")
            {
                task.Status = TaskModel.TaskStatus.ConfirmationPending;
            }
            else if (User.IsInRole("Leader"))
            {
                if (action == "Confirm")
                {
                    task.Status = TaskModel.TaskStatus.Completed;
                    task.CompletedDate = DateTime.Now;
                }
                else if (action == "Decline")
                {
                    task.Status = TaskModel.TaskStatus.InProgress;
                }
            }

            TaskCommentModel comment = new TaskCommentModel()
            {
                TaskModelId = id,
                DateTime = DateTime.Now,
                Message = model.message                
            };
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                comment.UserName = user.Name;
            }
            else
            {
                comment.UserName = "Unknown";
            }
            _context.CommentsDB.Add(comment);
            await _context.SaveChangesAsync();

            if (model.file != null)
            {
                var directory = FileModel.UploadsPath + task.Id + "/";
                string filePath = directory + Path.GetRandomFileName();

                Directory.CreateDirectory(directory);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.file.CopyToAsync(stream);
                }

                FileModel fm = new FileModel()
                {
                    OriginalFileName = Path.GetFileName(model.file.FileName),
                    FilePath = Path.GetRelativePath(_environment.WebRootPath, filePath),
                    TaskId = task.Id,
                    TaskCommentId = comment.Id,
                };
                _context.FilesDB.Add(fm);
                await _context.SaveChangesAsync();
                comment.FileId = fm.Id;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            throw;
        }
    }
}
