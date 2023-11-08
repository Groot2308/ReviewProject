using FluentValidation;
using ProjectReview.Models;

namespace ProjectReview.Validator
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Please enter your user name")
            .Length(6, 15).WithMessage("Name must be between 6 and 15 characters.")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Name invalid");

            RuleFor(model => model.Email)
           .NotEmpty().WithMessage("Please enter your Email")
           .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Email invalid");

            RuleFor(model => model.Password)
           .NotEmpty().WithMessage("Please enter your Password")
           .Length(8, 25).WithMessage("Name must be between 6 and 25 characters.")
           .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])(.{8,})$").WithMessage("Bao gồm ít nhất một chữ cái viết hoa, một chữ cái viết thường, một số và một ký tự đặc biệt và có độ dài tối thiểu là 8");
        }
    }
}
