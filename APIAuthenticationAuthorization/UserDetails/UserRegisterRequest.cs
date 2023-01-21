namespace APIAuthenticationAuthorization.UserDetails
{
    public class UserRegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }=string.Empty;


        [Required]
        [MinLength(6,ErrorMessage ="At Least 6 Characters needed...")]
        public string Password { get; set; }= string.Empty;


        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }= string.Empty;
    }
}
