using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WizzifyWeb.Data;
using WizzifyWeb.Models;

namespace WizzifyWeb.Controllers
{
    public class AccountsController : Controller
    {
        private readonly WizzifyContext _context;

        public AccountsController(WizzifyContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Account.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(string username, string password)
        {
            if (username == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.username == username && m.password == password);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("username,password,emailAddress,firstName,lastName")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                ViewData["username"] = "username";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("username,password,emailAddress,firstName,lastName")] Account account)
        {
            if (id != account.username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.username, account.emailAddress))
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
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.username == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        public async Task<IActionResult> LogIn() {
            return View();
        }

        public async Task<IActionResult> SignIn([Bind("username", "password")] Account account) 
        {
            if (!CheckAccount(account.username, account.password).Result) {
                ViewData["ErrorMessage"] = "Username or Password not valid";
                return View("LogIn");
            }
            ViewData["username"] = account.username;
            ViewData["password"] = account.password;
            return RedirectToAction("Details", new {account.username, account.password});
        }

        public async Task<bool> CheckAccount(string username, string password) {

            if (username == null)
            {
                return false;
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.username == username && m.password == password);
            if (account == null)
            {
                return false;
            }

            return true;
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var account = await _context.Account.FindAsync(id);
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(string id, string email)
        {
            return _context.Account.Any(e => e.username == id) 
                && _context.Account.Any(e => e.emailAddress == email);
        }

        [AcceptVerbs("GET", "POST")]
        public bool VerifyUsername(string username) {
            if (username.Equals("WiZ")) { return true; }
            StringValidator strVal = new StringValidator(4, 32, "\"$&");
            try
            {
                // Attempt validation.
                strVal.Validate(username);
                if (_context.Account.Any(e => e.username == username)) {
                    return false;
                }

                return true;
            }
            catch (ArgumentException exception)
            {
                // Validation failed.
                return false;
            }
        }
        [AcceptVerbs("GET", "POST")]
        public bool VerifyPassword(string password)
        {
            StringValidator strVal = new StringValidator(8, 32, "\"$&");
            try
            {
                // Attempt validation.
                strVal.Validate(password);
                return true;
            }
            catch (ArgumentException exception)
            {
                // Validation failed.
                return false;
            }
        }
        [AcceptVerbs("GET", "POST")]
        public bool VerifyEmail(string email)
        {
            return !_context.Account.Any(e => e.emailAddress == email);
        }
    }
}
