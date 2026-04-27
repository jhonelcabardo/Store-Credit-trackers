namespace Store_Credit_Tracker.Views
{
    public partial class LoginPage : ContentPage
    {
        private const string VALID_USERNAME = "admin";
        private const string VALID_PASSWORD = "admin123";

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageLabel.Text = "Please enter username and password";
                MessageLabel.TextColor = Colors.Red;
                return;
            }

            if (username == VALID_USERNAME && password == VALID_PASSWORD)
            {
                App.LoggedInUser = username;

                await DisplayAlert("Success", "Login successful!", "OK");

                Application.Current!.MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MessageLabel.Text = "Invalid username or password";
                MessageLabel.TextColor = Colors.Red;
                PasswordEntry.Text = string.Empty;
            }
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}