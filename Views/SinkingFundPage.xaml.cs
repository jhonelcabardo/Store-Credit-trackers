using System.Collections.ObjectModel;
using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class SinkingFundPage : ContentPage
{
    private ObservableCollection<SinkingFundRecord> _records = new();
    private bool _isSinkingListVisible = false;

    public SinkingFundPage()
    {
        InitializeComponent();
        SinkingFundCollectionView.ItemsSource = _records;
        StatusPicker.SelectedIndex = 1;
        PaymentDatePicker.Date = GetNextSaturday();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRecordsAsync();
    }

    private async Task LoadRecordsAsync()
    {
        var records = await App.Database.GetSinkingFundRecordsAsync();
        _records.Clear();

        foreach (var record in records)
            _records.Add(record);
    }

    private void OnToggleSinkingListClicked(object sender, EventArgs e)
    {
        _isSinkingListVisible = !_isSinkingListVisible;
        SinkingFundCollectionView.IsVisible = _isSinkingListVisible;
        ToggleSinkingListButton.Text = _isSinkingListVisible ? "Hide" : "Show";
    }

    private async void OnAddRecordClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MemberNameEntry.Text))
        {
            await DisplayAlert("Error", "Please enter member name.", "OK");
            return;
        }

        if (!decimal.TryParse(AmountEntry.Text, out decimal amount))
        {
            await DisplayAlert("Error", "Invalid amount.", "OK");
            return;
        }

        var record = new SinkingFundRecord
        {
            MemberName = MemberNameEntry.Text.Trim(),
            Amount = amount,
            PaymentDate = PaymentDatePicker.Date ?? DateTime.Today,
            Status = StatusPicker.SelectedItem?.ToString() ?? "Unpaid"
        };

        await App.Database.AddSinkingFundRecordAsync(record);
        await LoadRecordsAsync();

        MemberNameEntry.Text = string.Empty;
        AmountEntry.Text = "100";
        StatusPicker.SelectedIndex = 1;
        PaymentDatePicker.Date = GetNextSaturday();

        await DisplayAlert("Success", "Sinking fund record added.", "OK");
    }

    private DateTime GetNextSaturday()
    {
        DateTime today = DateTime.Today;
        int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntilSaturday == 0 ? 7 : daysUntilSaturday);
    }
}