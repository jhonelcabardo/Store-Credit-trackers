namespace Store_Credit_Tracker.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter a username");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter an email");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match");
                return;
            }

            if (password.Length < 4)
            {
                ShowError("Password must be at least 4 characters");
                return;
            }

            if (App.Database == null)
            {
                ShowError("Database service not available");
                return;
            }

            var success = await App.Database.RegisterUserAsync(username, password, email);

            if (success)
            {
                await DisplayAlert("Success", "Account created successfully! Please login.", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                ShowError("Username already exists. Please choose another username.");
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }
}