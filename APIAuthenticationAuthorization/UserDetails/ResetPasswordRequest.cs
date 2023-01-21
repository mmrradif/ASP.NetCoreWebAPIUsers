namespace APIAuthenticationAuthorization.UserDetails
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;


        [Required]
        [MinLength(6, ErrorMessage = "At Least 6 Characters needed...")]
        public string Password { get; set; } = string.Empty;


        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
