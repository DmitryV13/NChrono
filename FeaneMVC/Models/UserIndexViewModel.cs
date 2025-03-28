using System;
using System.Collections.Generic;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class UserIndexViewModel
    {
        public IEnumerable<UserData> Users { get; set; }
        public UserData UserToEdit { get; set; }
    }
}
