namespace JobApplicationTrackerV2.Services
{
    public class ThemeService
    {
        private bool _isDarkMode;

        public bool IsDarkMode
    {
        get => _isDarkMode;
            set
            {
      if (_isDarkMode != value)
       {
          _isDarkMode = value;
          NotifyStateChanged();
    }
            }
        }

   public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void ToggleDarkMode()
        {
   IsDarkMode = !IsDarkMode;
        }
    }
}
