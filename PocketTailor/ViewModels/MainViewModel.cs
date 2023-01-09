using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PocketTailor.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {

        public CreateMenuViewModel createMenuVM { get; set; }
        public ExtractMenuViewModel extractMenuVM { get; set; }

        //private object _currentView { get; set; }


        [ObservableProperty]
        public object _currentView;

        

        [RelayCommand]
        public void ChangeView_Create()
        {
            CurrentView = createMenuVM;
        }

        [RelayCommand]
        public void ChangeView_Extract()
        {
            CurrentView = extractMenuVM;
        }

        [RelayCommand]
        public void MinimizeWindowButton()
        {
            
        }

        [RelayCommand]
        public void MaximizeWindowButton()
        {

        }

        [RelayCommand]
        public void CloseWindowButton()
        {
            // Can't promise other windows aren't open!!
            // TODO: Send parameter giving window name from xaml
            App.Current.Windows[0].Close();
        }

        public MainViewModel()
        {
            createMenuVM = new CreateMenuViewModel();
            extractMenuVM = new ExtractMenuViewModel();
            CurrentView = createMenuVM;
        }

        
    }
}
