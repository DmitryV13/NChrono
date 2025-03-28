using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.Response;
using WebApplication1.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISession = WebApplication1.Interfaces.ISession;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUSer _userRepository;
        private readonly ISession _session;

        public UserController(IUSer userRepository, ISession session)
        {
            _userRepository = userRepository;
            _session = session;
        }

        // GET: /User/Index
        public async Task<IActionResult> Index(Guid? editId)
        {
            // Fetch all users asynchronously
            var users =  _userRepository.GetAllUsers();

            UserResponse userToEdit = null;
            if (editId.HasValue)
            {
                userToEdit = await _userRepository.GetOneUserByIdAsync(editId.Value);
            }

            var viewModel = new UserIndexViewModel
            {
                Users = users,
                UserToEdit = userToEdit?.User
            };

            return View(viewModel);
        }

        // POST: /User/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deleteResponse =  _userRepository.DeleteUser(id);

            if (deleteResponse.Status)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while deleting the user.");
            var user = await _userRepository.GetOneUserByIdAsync(id);
            return View("Index", new UserIndexViewModel
            {
                Users = _userRepository.GetAllUsers(),
                UserToEdit = user.User
            });
        }

        // POST: /User/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserIndexViewModel model)
        {


            var updateUserResponse = await _userRepository.UpdateUser(model.UserToEdit);

            if (updateUserResponse.Status)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
            model.Users = _userRepository.GetAllUsers();
            return View("Index", model);
        }
    }
}
