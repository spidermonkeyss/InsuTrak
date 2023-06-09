﻿using Capstone.DAO;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Capstone.Controllers
{
    [Route("profiles")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileDAO profileDAO;

        public ProfileController(IProfileDAO profileDAO)
        {
            this.profileDAO = profileDAO;
        }

        [Authorize]
        [HttpGet("{profileId}")]
        public IActionResult GetProfile(int profileId)
        {
            Profile profile = profileDAO.GetProfile(profileId);
            if (profile == null)
            {
                return Forbid();
            }

            int userId = int.Parse(this.User.FindFirst("sub").Value);
            if (userId == profile.UserId)
            {
                return Ok(profile);
            }
            else
            {
                return Forbid();
            }    
        }

        [Authorize]
        [HttpGet()]
        public IActionResult GetAllUserProfiles()
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            List<Profile> profiles = profileDAO.GetAllUserProfiles(userId);
            return Ok(profiles);
        }

        [Authorize]
        [HttpPost()]
        public IActionResult AddProfile([FromBody] Profile profile)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            profile.UserId = userId;

            bool result = profileDAO.AddProfile(profile);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "An error occurred and profile was not created." });
            }
        }

        [Authorize]
        [HttpPut()]
        public IActionResult UpdateProfile([FromBody] Profile profile)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            profile.UserId = userId;

            Profile pro = profileDAO.GetProfile(profile.ProfileId);
            if (pro == null)
            {
                return BadRequest(new { message = "Profile does not exist" });
            }

            if (pro.UserId != userId)
            {
                return BadRequest(new { message = "Profile does not belong to user" });
            }

            bool result = profileDAO.UpdateProfile(profile);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "An error occurred and profile was not updated." });
            }
        }

        [Authorize]
        [HttpDelete("{profileId}")]
        public IActionResult DeleteProfile(int profileId)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            Profile profile = profileDAO.GetProfile(profileId);
            if (profile == null)
            {
                return BadRequest(new { message = "Profile does not exist" });
            }

            if (userId != profile.UserId)
            {
                return BadRequest(new { message = "Profile id does not belong to logged in user" });
            }

            bool result = profileDAO.DeleteProfile(profileId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "An error occurred and profile was not deleted." });
            }
        }
    }
}
