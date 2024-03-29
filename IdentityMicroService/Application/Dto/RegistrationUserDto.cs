﻿using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Application.Dto;

public class RegistrationUserDto : UserModelForAuthorizationDto
{
    [Required(ErrorMessage = "Confirmation Password is required.")]
    [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
    public string ConfirmPassword { get; set; }
}