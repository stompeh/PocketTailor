using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace PocketTailor.ViewModels
{
    [ObservableObject]
    public partial class CreateMenuViewModel
    {
        [ObservableProperty]
        public static string? _currentTargetFilePath = "None";
        [ObservableProperty]
        public static string? _currentDataFilePath = "None";
        [ObservableProperty]
        public static ObservableCollection<string>? _currentTargetFilePocketsDisplay;
        private static List<string>? CurrentTagetFilePockets;
        [ObservableProperty]
        public static string? _pocketName;
        [ObservableProperty]
        public static bool _deleteDataFileOnWrite = false;
        [ObservableProperty]
        public static string? _selectedPocketLB;
        [ObservableProperty]
        public static int _selectedPocketLBIndex;


        private ObservableCollection<string> CleanDisplayPockets(ObservableCollection<string> dirtyData)
        {
            ObservableCollection<string> cleanData = new ObservableCollection<string>();
            string temp;

            foreach (string dirt in dirtyData)
            {
                temp = dirt.Replace("$DATA", "");
                temp = temp.Trim(':');
                cleanData.Add(temp);
            }

            return cleanData;
        }

        [RelayCommand]
        public void SelectTargetFileBtn()
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            if (ofd.FileName == "")
            {
                CurrentTargetFilePath = "None";
                return;
            }

            CurrentTargetFilePath = ofd.FileName;
            FileInfo target = new FileInfo(CurrentTargetFilePath);
            CurrentTargetFilePocketsDisplay = PocketHandler.EnumerateStreams(target);
            
            CurrentTargetFilePocketsDisplay.RemoveAt(0);
            CurrentTagetFilePockets = CurrentTargetFilePocketsDisplay.ToList();
            CurrentTargetFilePocketsDisplay = CleanDisplayPockets(CurrentTargetFilePocketsDisplay);
        }

        [RelayCommand]
        public void SelectDataFileBtn()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.ShowDialog();

            if (ofd.FileName == "")
            {
                CurrentDataFilePath = "None";
                return;
            }

            CurrentDataFilePath = ofd.FileName;
        }


        [RelayCommand]
        public void ExtractFromTargetSelection()
        {
            if (CurrentTargetFilePath == "None")
            {
                MessageBox.Show("Please choose a target file");
                return;
            }


            if (SelectedPocketLBIndex != -1)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.DefaultExt = "txt";
                sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                sfd.ShowDialog();

                if (sfd.FileName == "")
                {
                    return;
                }

                string contentToWrite = File.ReadAllText(CurrentTargetFilePath + CurrentTagetFilePockets.ElementAt(SelectedPocketLBIndex));
                File.WriteAllText(sfd.FileName, contentToWrite);
            }
            else
            {
                MessageBox.Show("No pocket selected");
            }
        }

        [RelayCommand]
        public void WriteToTarget()
        {
            if (!File.Exists(CurrentDataFilePath))
            {
                MessageBox.Show("Data file not found!");
                return;
            }

            if (!File.Exists(CurrentTargetFilePath))
            {
                MessageBox.Show("Target file not found!");
                return;
            }

            if (PocketName == null)
            {
                MessageBox.Show("Please provide a pocket name.");
                return;
            }

            FileInfo target = new FileInfo(CurrentTargetFilePath);

            foreach (string name in CurrentTargetFilePocketsDisplay)
            {
                if (name == PocketName)
                {
                    var result = MessageBox.Show("Duplicate pocket name found, overwrite data?", "Uh oh", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No) 
                    { return; }
                }
            }

            string contentToWrite = File.ReadAllText(CurrentDataFilePath);
            string pocketName = CurrentTargetFilePath + ":" + PocketName;
            File.WriteAllText(pocketName, contentToWrite);

            FileInfo after = new FileInfo(CurrentTargetFilePath);
            CurrentTargetFilePocketsDisplay = PocketHandler.EnumerateStreams(after);
            CurrentTargetFilePocketsDisplay.RemoveAt(0);
            CurrentTagetFilePockets = CurrentTargetFilePocketsDisplay.ToList();
            CurrentTargetFilePocketsDisplay = CleanDisplayPockets(CurrentTargetFilePocketsDisplay);

            if (DeleteDataFileOnWrite)
            {
                File.Delete(CurrentDataFilePath);
                CurrentDataFilePath = "None";
            }

        }
    }
}
