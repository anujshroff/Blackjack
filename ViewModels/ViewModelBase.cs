using CommunityToolkit.Mvvm.ComponentModel;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// Provides common functionality like INotifyPropertyChanged implementation.
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// Indicates whether the ViewModel is currently performing a busy operation.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// The title of the page/view associated with this ViewModel.
        /// </summary>
        [ObservableProperty]
        private string title = string.Empty;

        /// <summary>
        /// Virtual method that can be overridden to perform initialization logic.
        /// Called when the view is navigated to.
        /// </summary>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
