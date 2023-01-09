using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace PocketTailor.ViewModels
{
    [ObservableObject]
    public partial class ExtractMenuViewModel
    {
        [ObservableProperty]
        public static string? _currentTargetFilePath = "None";
        [ObservableProperty]
        public static ObservableCollection<string>? _currentTargetFileADS;

        [RelayCommand]
        public void SelectTargetFileBtn()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.ShowDialog();

            CurrentTargetFilePath = ofd.FileName;

            FileInfo target = new FileInfo(CurrentTargetFilePath);
            CurrentTargetFileADS = PocketHandler.EnumerateStreams(target);

            CurrentTargetFileADS.RemoveAt(0);
        }



    }
}
