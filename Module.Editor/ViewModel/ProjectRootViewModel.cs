﻿using System;
using FomodInfrastructure.MvvmLibrary.Commands;
using FomodModel.Base;
using FomodModel.Base.ModuleCofiguration;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using FomodInfrastructure.Interface;

namespace Module.Editor.ViewModel
{
    public class ProjectRootViewModel : BaseViewModel
    {
        #region Services

        private readonly IFileBrowserDialog _fileBrowserDialog;

        #endregion

        private ProjectRoot _data;

        public RelayCommand AddImageCommand { get; }
        public RelayCommand RemoveImageCommand { get; }
        public RelayCommand SetImageCommand { get; }
        public RelayCommand<CompositeDependency> AddCompositeDependencyCommand { get; }
        public RelayCommand<CompositeDependency> RemoveCompositeDependencyCommand { get; }

        public RelayCommand<CompositeDependency> AddFileDependencyCommand { get; }
        public RelayCommand<CompositeDependency> AddFlagDependencyCommand { get; }
        public RelayCommand<FileDependency> RemoveFileDependencyCommand { get; }
        public RelayCommand<FlagDependency> RemoveFlagDependencyCommand { get; }

        public RelayCommand ChkModuleNamePositionCommand { get; }


        public ProjectRootViewModel(IFileBrowserDialog fileBrowserDialog)
        {
            _fileBrowserDialog = fileBrowserDialog;

            AddImageCommand = new RelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
            SetImageCommand = new RelayCommand(SetImage);
            AddCompositeDependencyCommand = new RelayCommand<CompositeDependency>(AddCompositeDependency);
            RemoveCompositeDependencyCommand = new RelayCommand<CompositeDependency>(RemoveCompositeDependency);
            AddFileDependencyCommand = new RelayCommand<CompositeDependency>(AddFileDependency);
            RemoveFileDependencyCommand = new RelayCommand<FileDependency>(RemoveFileDependency);
            AddFlagDependencyCommand = new RelayCommand<CompositeDependency>(AddFlagDependency);
            RemoveFlagDependencyCommand = new RelayCommand<FlagDependency>(RemoveFlagDependency);
            ChkModuleNamePositionCommand = new RelayCommand(ChkModuleNamePosition);
            var notifyPropertyChanged = this as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
                notifyPropertyChanged.PropertyChanged += (obj, args) => _data = args.PropertyName == nameof(Data) ? (ProjectRoot)Data : _data;
        }

        private void ChkModuleNamePosition()
        {
            //_data.ModuleConfiguration.ModuleName.Position = null; 
            System.Diagnostics.Debug.Print("***Helow***");
        }

        private void AddImage()
        {
            _data.ModuleConfiguration.ModuleImage = new HeaderImage();
        }

        private void RemoveImage()
        {
            _data.ModuleConfiguration.ModuleImage = null;
        }

        private void SetImage()
        {
            _fileBrowserDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            _fileBrowserDialog.ShowDialog();
            var fileName = _fileBrowserDialog.SelectedPath;
            _fileBrowserDialog.Reset();
            if (!File.Exists(fileName) || !fileName.StartsWith(_data.FolderPath)) return; //тут я потом сделаю сервис копирования файлов
            fileName = fileName.Replace(_data.FolderPath + Path.DirectorySeparatorChar, string.Empty);
            _data.ModuleConfiguration.ModuleImage.Path = fileName;
        }

        private void AddCompositeDependency(CompositeDependency dependency)
        {
            if (dependency==null)
                _data.ModuleConfiguration.ModuleDependencies = CompositeDependency.Create();
            else
                dependency.Dependencies = CompositeDependency.Create();
        }

        private void RemoveCompositeDependency(CompositeDependency dependency)
        {
            
            if (dependency.Parent == null)
                _data.ModuleConfiguration.ModuleDependencies = null;
            else
                dependency.Parent.Dependencies = null;
        }

        private void AddFileDependency(CompositeDependency dependency)
        {
            var list = dependency.FileDependencies;
            if (list == null) list = new ObservableCollection<FileDependency>();

            list.Add(FileDependency.Create("aa/ds2/fdf.cc"));
        }

        private void AddFlagDependency(CompositeDependency dependency)
        {

            var list = dependency.FlagDependencies;
            if (list == null) list = new ObservableCollection<FlagDependency>();

            list.Add(FlagDependency.Create());
        }

        private void RemoveFileDependency(FileDependency dependency)
        {
            dependency.Parent.FileDependencies.Remove(dependency);
            dependency.Parent = null;
        }
        private void RemoveFlagDependency(FlagDependency dependency)
        {
            dependency.Parent.FlagDependencies.Remove(dependency);
            dependency.Parent = null;
        }
    }
}