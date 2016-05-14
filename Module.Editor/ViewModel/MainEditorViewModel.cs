﻿using System.Collections.ObjectModel;
using System.Linq;
using AspectInjector.Broker;
using FomodInfrastructure.Aspect;
using FomodInfrastructure.MvvmLibrary.Commands;
using FomodModel.Base;
using FomodModel.Base.ModuleCofiguration;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Interactivity.InteractionRequest;
using System;
using FomodInfrastructure.Interface;

namespace Module.Editor.ViewModel
{
    [Aspect(typeof (AspectINotifyPropertyChanged))]
    public class MainEditorViewModel : BindableBase
    {
        #region Services

        private IRegionManager _regionManager;
        private IRepository<ProjectRoot> _repository;
        private readonly IMemoryService _memoryService;

        #endregion

        public MainEditorViewModel(IMemoryService memoryService)
        {
            _memoryService = memoryService;

            AddStepCommand = new RelayCommand<ProjectRoot>(AddStep);
            AddGroupCommand = new RelayCommand<InstallStep>(AddGroup);
            AddPluginCommand = new RelayCommand<Group>(AddPlugin);
            DeleteDialogCommand = new RelayCommand<object[]>(DeleteDialog);
        }
        
        public void ConfigurateViewModel(IRegionManager regionManager, IRepository<ProjectRoot> repository)
        {
            if (repository == null) throw new NotImplementedException();
            _repository = repository;
            _regionManager = regionManager;
            var root = Data.FirstOrDefault(i => i.FolderPath == repository.CurrentPath);
            if (root == null)
            {
                Data.Add(repository.GetData());
                if (FirstData == null)
                    FirstData = repository.GetData();
            }
            _memoryService.GetMemorySize(Data);
            SelectedNode = FirstData;
        }

        #region Properties

        public bool IsNeedSave
        {
            get
            {
                _memoryService.GetMemorySize(Data);
                return _memoryService.IsMemorySizeChanged;
            }
            set
            {
                if (!value)
                    _memoryService.Reset(Data);
            }
        }

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; } = new InteractionRequest<IConfirmation>();

        public ObservableCollection<ProjectRoot> Data { get; set; } = new ObservableCollection<ProjectRoot>();

        public ProjectRoot FirstData { get; private set; } //TODO Не забыть про очищение свойства при закрытии или удалении проекта, или сделать слабую ссылку

        private object _selectedNode;

        public object SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                if (value == null) return;
                var name = value.GetType().Name;
                var param = new NavigationParameters
                {
                    {name, value}
                };
                _regionManager.Regions["NodeRegion"].RequestNavigate(name + "View", param);
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand<ProjectRoot> AddStepCommand { get; }
        public RelayCommand<InstallStep> AddGroupCommand { get; }
        public RelayCommand<Group> AddPluginCommand { get; }
        public RelayCommand<object[]> DeleteDialogCommand { get; private set; }

        #endregion

        #region Methods

        private void DeleteDialog(object[] obj)
        {
            var item = obj[1];
            ConfirmationRequest.Raise(new Confirmation { Content = "Вы точно хотите удалить " + item.GetType().Name + " узел?", Title = "Удалить узел?" }, c =>
            {
                if (!c.Confirmed) return;
                if (item is Plugin)
                    RemovePlugin(obj);
                else if (item is Group)
                    RemoveGroup(obj);
                else if (item is InstallStep)
                    RemoveStep(obj);
                else
                    throw new NotImplementedException();
            });
        }

        private void AddStep(ProjectRoot p)
        {
            if (p.ModuleConfiguration.InstallSteps == null)
                p.ModuleConfiguration.InstallSteps = new StepList();
            if (p.ModuleConfiguration.InstallSteps.InstallStep == null)
                p.ModuleConfiguration.InstallSteps.InstallStep = new ObservableCollection<InstallStep>();
            p.ModuleConfiguration.InstallSteps.InstallStep.Add(new InstallStep { Name = "New Step" });
        }

        private void AddGroup(InstallStep p)
        {
            if (p.OptionalFileGroups == null)
                p.OptionalFileGroups = new GroupList();
            if (p.OptionalFileGroups.Group == null)
                p.OptionalFileGroups.Group = new ObservableCollection<Group>();
            p.OptionalFileGroups.Group.Add(new Group { Name = "New Group" });
        }

        private void AddPlugin(Group p)
        {
            if (p.Plugins == null)
                p.Plugins = new PluginList();
            if (p.Plugins.Plugin == null)
                p.Plugins.Plugin = new ObservableCollection<Plugin>();
            p.Plugins.Plugin.Add(new Plugin { Name = "New Plugin" });
        }

        private void RemoveStep(object[] p)
        {
            var parent = (ProjectRoot)p[0];
            var child = (InstallStep)p[1];
            parent.ModuleConfiguration.InstallSteps.InstallStep.Remove(child);
        }

        private void RemoveGroup(object[] p)
        {
            var parent = (InstallStep)p[0];
            var child = (Group)p[1];
            parent.OptionalFileGroups.Group.Remove(child);
        }

        private void RemovePlugin(object[] p)
        {
            var parent = (Group)p[0];
            var child = (Plugin)p[1];
            parent.Plugins.Plugin.Remove(child);
        }

        public void Save()
        {
            _repository.SaveData(_repository.CurrentPath);
        }

        public void SaveAs()
        {
            _repository.SaveData();
        }

        #endregion
    }
}