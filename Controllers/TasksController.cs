
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TasksManager.Data;
using TasksManager.Models;

public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private class TaskModelWithOwnerName : TaskModel
    {
        public string? OwnerName { get; set; }
    }

    // GET: TASKS
    public async Task<IActionResult> Index(int? SelectedStatus)
    {
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
        if (SelectedStatus != -1 && SelectedStatus != null)
        {
            tasks = tasks.Where(t => t.Status == (TaskModel.TaskStatus)SelectedStatus);
            ViewData["CurrentFilter"] = SelectedStatus;
        }
        return View(await tasks.AsNoTracking().ToListAsync());
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
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // GET: TASKS/Create
    public async Task<IActionResult> Create()
    {
        TaskDTO taskVM = new TaskDTO() { Name = "", Description = "" };
        taskVM.Users = await _context.UsersDB.AsNoTracking()
            .ToListAsync();
        return View(taskVM);
    }

    // POST: TASKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskDTO taskVM)
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
                    Owner = currentUser,
                    OwnerId = currentUser!.Id,
                    AssignedUsers = await _context.UsersDB
                                .Where(u => taskVM.SelectedUserIds.Contains(u.Id))
                                .ToListAsync()
                };
                _context.Add(task);
                await _context.SaveChangesAsync();
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
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(item => item.Id == id);
        if (task == null)
        {
            return NotFound();
        }
        var editViewModel = new TaskDTO()
        {
            Users = await _context.UsersDB.ToListAsync(),
            Name = task.Name,
            DueToDate = task.DueToDate,
            Description = task.Description,
        };
        foreach (var user in task.AssignedUsers)
        {
            editViewModel.SelectedUserIds.Add(user.Id);
        }

        return View(editViewModel);
    }

    // POST: TASKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, TaskDTO editViewModel)
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

                task.AssignedUsers = await _context.UsersDB
                        .Where(u => editViewModel.SelectedUserIds.Contains(u.Id))
                        .ToListAsync();
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
            //_context.TasksDB.Remove(task);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TaskExists(int? id)
    {
        return _context.TasksDB.Any(e => e.Id == id);
    }

    // POST: TASKS/Submit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id)
    {
        try
        {
            var task = _context.TasksDB.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }
            task.Status = TaskModel.TaskStatus.ConfirmationPending;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: TASKS/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string action)
    {
        try
        {
            var task = _context.TasksDB.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }
            if (action == "Approve")
            {
                task.Status = TaskModel.TaskStatus.Completed;
                task.CompletedDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else if (action == "Reject")
            {
                task.Status = TaskModel.TaskStatus.InProgress;
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }
}
