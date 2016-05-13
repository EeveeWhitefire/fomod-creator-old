﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FomodInfrastructure;
using FomodInfrastructure.MvvmLibrary.Commands;
using Prism.Regions;
using FomodInfrastructure.Aspect;
using AspectInjector.Broker;
using MahApps.Metro.Controls.Dialogs;
using Module.Editor.ViewModel;
using Module.Welcome.ViewModel;
using System.Diagnostics;

namespace MainApplication
{
    public class ShellViewModel
    {
        private readonly string _defautlTitle;

        #region Services

        private readonly IRegionManager _regionManager;
        private readonly IDialogCoordinator _dialogCoordinator;

        #endregion

        public ShellViewModel(IRegionManager regionManager)
        {
            Title = _defautlTitle = "FOMOD Creator beta v" + GetVersion();
            _regionManager = regionManager;
        }

        #region Commands

        public RelayCommand<object> CloseTabCommand { get; }
        public RelayCommand SaveProjectCommand { get; }

        #endregion

        #region Properties

        object _CurentSelectedItem;

        [Aspect(typeof (AspectINotifyPropertyChanged))]
        public object CurentSelectedItem
        {
            get { return _CurentSelectedItem; }
            set
            {
                _CurentSelectedItem = value;
                SaveProjectCommand.RaiseCanExecuteChanged();
                var b = (CurentSelectedItem as FrameworkElement)?.DataContext;
                if (b != null && b is MainEditorViewModel)
                    Title = $"{(b as MainEditorViewModel).FirstData.ModuleInformation.Name}: {_defautlTitle}";
                else
                    Title = _defautlTitle;
            }
        }

        [Aspect(typeof (AspectINotifyPropertyChanged))]
        public string Title { get; set; }

        #endregion

        public ShellViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator)
        {
            _regionManager = regionManager;
            _dialogCoordinator = dialogCoordinator;
            CloseTabCommand = new RelayCommand<object>(CloseTab);
            SaveProjectCommand = new RelayCommand(SaveProject, CanSaveProject);
        }

        #region Methods

        private async void CloseTab(object p)
        {
            var removeView =
                _regionManager.Regions[Names.MainContentRegion].Views.Cast<FrameworkElement>()
                    .FirstOrDefault(v => v.DataContext == p);
            var result = await CofirmDialog();
            if (result)
                SaveProject();
            _regionManager.Regions[Names.MainContentRegion].Remove(removeView);
        }

        private void SaveProject()
        {
            var vm = (MainEditorViewModel) ((FrameworkElement) CurentSelectedItem).DataContext;
            vm.Save();
        }

        private bool CanSaveProject() => (CurentSelectedItem as FrameworkElement)?.DataContext is MainEditorViewModel;
        private async Task<bool> CofirmDialog() => await _dialogCoordinator.ShowMessageAsync(this, "Закрыть проект", "Сохранить перед закрытием?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative;
        
        private string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        #endregion
    }
}